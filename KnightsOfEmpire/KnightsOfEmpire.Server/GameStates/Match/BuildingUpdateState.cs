using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Buildings;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Server.Buildings;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.GameStates.Match
{
    public class BuildingUpdateState : BuildingState
    { 
        public List<Unit>[] GameUnits;
        public override void Initialize()
        {
            base.Initialize();
            
        }

        public override void LoadDependencies()
        {
            GameUnits = Parent.GetSiblingGameState<UnitUpdateState>().GameUnits;
            BuildPlayerStartBases();
        }

        public void BuildPlayerStartBases()
        {
            (int x, int y)[] StartPoses = Server.Resources.Map.starterPositions;
            Random rand = new Random();
            (int x, int y)[] ShuffledStartPoses = StartPoses.OrderBy(x => rand.Next()).ToArray();
            Console.WriteLine("Positions");
            foreach ((int x, int y) sp in ShuffledStartPoses)
            {
                Console.WriteLine($"{sp.x} {sp.y}");
            }
            int pos = 0;
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                if (!Server.TCPServer.IsClientConnected(i)) continue;
                CreateBuildingRequest request = new CreateBuildingRequest
                {
                    BuildingPosX = ShuffledStartPoses[pos].x,
                    BuildingPosY = ShuffledStartPoses[pos].y,
                    BuildingTypeId = BuildingManager.MainBuildingId
                };
                CreateBuilding(i, request);
                pos++;
            }
        }

        public override void Update()
        {
            base.Update();
            for(int i=0;i<MaxPlayerCount;i++)
            {
                for(int j=0;j<GameBuildings[i].Count;j++)
                {
                    Building b = GameBuildings[i][j];
                    if (b.HealthPercentage <= 0)
                    {
                        DestroyBuilding(i, j);
                        j--;
                        continue;
                    }
                    BuildingLogicManager.ProcessUpdate(b);
                }
            }
            SendDataUpdates();
        }

        public void SendDataUpdates()
        {
            foreach (List<Building> buildings in GameBuildings)
            {
                foreach (Building b in buildings)
                {
                    UpdateBuildingDataRequest request = new UpdateBuildingDataRequest
                    {
                        TimeStamp = DateTime.Now.Ticks,
                        PosX = b.Position.X,
                        PosY = b.Position.Y,
                        PlayerId = b.PlayerId,
                        Health = b.Health
                    };
                    SentPacket packet = new SentPacket(PacketsHeaders.UpdateBuildingDataRequest);
                    packet.stringBuilder.Append(JsonSerializer.Serialize(request));

                    for (int i = 0; i < Server.TCPServer.MaxConnections; i++)
                    {
                        if (Server.TCPServer.IsClientConnected(i))
                        {
                            packet.ClientID = i;
                            Server.UDPServer.SentTo(packet, Server.TCPServer.GetClientAddress(i), i);
                        }
                    }
                }
            }
        }


        public override void HandleTCPPacket(ReceivedPacket packet)
        {
            switch (packet.GetHeader())
            {
                case PacketsHeaders.CreateBuildingRequest:
                    CreateBuilding(packet);
                    break;
            }
        }
        protected void CreateBuilding(ReceivedPacket packet)
        {
            CreateBuildingRequest request = packet.GetDeserializedClassOrDefault<CreateBuildingRequest>();
            if (request == null)
            {
                Console.WriteLine($"build: Request malformed!");
                return;
            }
            Vector2i tileBuildPos = Map.ToTilePos(new Vector2f(request.BuildingPosX, request.BuildingPosY));
            request.BuildingPosX = tileBuildPos.X;
            request.BuildingPosY = tileBuildPos.Y;
            CreateBuilding(packet.ClientID, request);
        }

        protected void CreateBuilding(int clientID, CreateBuildingRequest request)
        {
            if (Server.Resources.IsDefeated[clientID]) return;
            Vector2i tileBuildPos = new Vector2i(request.BuildingPosX, request.BuildingPosY);
            Map gameMap = Server.Resources.Map;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!gameMap.IsTileInBounds(tileBuildPos.X + i, tileBuildPos.Y + j)) continue;
                    if (!gameMap.IsTileWalkable(tileBuildPos.X + i, tileBuildPos.Y + j))
                    {
                        Console.WriteLine($"{tileBuildPos}: Tile or neighborhood is blocking!");
                        return;
                    }
                }
            }

            foreach (List<Unit> units in GameUnits)
            {
                foreach (Unit unit in units)
                {
                    if (Map.ToTilePos(unit.Position).Equals(tileBuildPos))
                    {
                        Console.WriteLine($"{tileBuildPos}: unit is blocking!");
                        return;
                    }
                }
            }

            if (GameBuildings[clientID].Count > 0)
            {
                bool inRange = false;
                foreach (Building b in GameBuildings[clientID])
                {
                    int dist = Math.Max(Math.Abs(b.Position.X - tileBuildPos.X), Math.Abs(b.Position.Y - tileBuildPos.Y));
                    if (dist < MaxBuildingSeparateDistance)
                    {
                        inRange = true;
                        break;
                    }
                }
                if (!inRange)
                {
                    Console.WriteLine($"{tileBuildPos}: No friendly buildings in range!");
                    return;
                }
            }

            Building creation = BuildingManager.GetBuilding(request.BuildingTypeId);
            if (creation == null)
            {
                Console.WriteLine($"{tileBuildPos}: No building of type {request.BuildingTypeId} exists!");
                return;
            }

            if (!Server.Resources.UseGold(clientID, creation.BuildCost))
            {
                Console.WriteLine($"{tileBuildPos}: Not enough gold {creation.BuildCost}!");
                return;
            }

            creation.Position = tileBuildPos;
            creation.PlayerId = clientID;
            BuildingLogicManager.ProcessOnCreate(creation);
            GameBuildings[creation.PlayerId].Add(creation);

            Server.Resources.NavigationManager.AddBuildingOnMap(tileBuildPos.X, tileBuildPos.Y);

            RegisterBuildingRequest response = new RegisterBuildingRequest
            {
                BuildingPosX = tileBuildPos.X,
                BuildingPosY = tileBuildPos.Y,
                BuildingTypeId = creation.BuildingId,
                PlayerId = creation.PlayerId
            };

            SentPacket sentPacket = new SentPacket(PacketsHeaders.RegisterBuildingRequest);
            sentPacket.stringBuilder.Append(JsonSerializer.Serialize(response));
            Console.WriteLine($"{tileBuildPos}: Build successful!");
            Server.TCPServer.Broadcast(sentPacket);
        }

        public void DestroyAllBuildings(int playerId)
        {
            for(int i=0;i<GameBuildings[playerId].Count;i++)
            {
                DestroyBuilding(playerId, i);
                i--;
            }
        }

        protected void DestroyBuilding(int playerId, int index)
        {
            Building b = GameBuildings[playerId][index];

            Server.Resources.NavigationManager.RemoveBuildingFromMap(b.Position.X, b.Position.Y);

            UnregisterBuildingRequest response = new UnregisterBuildingRequest
            {
                DestroyPosX = b.Position.X,
                DestroyPosY = b.Position.Y,
                PlayerId = playerId
            };

            BuildingLogicManager.ProcessOnDestroy(b);

            SentPacket sentPacket = new SentPacket(PacketsHeaders.UnregisterBuildingRequest);
            sentPacket.stringBuilder.Append(JsonSerializer.Serialize(response));

            Server.TCPServer.Broadcast(sentPacket);
            GameBuildings[playerId].RemoveAt(index);
        }
    }
}
