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
        public static int MaxBuildingSeparateDistance = 5;
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

        protected void DestroyBuilding(int playerId, Vector2i tilePos)
        {
            int deleteIndex = GameBuildings[playerId].FindIndex(x => x.Equals(tilePos));
            if (deleteIndex != -1) GameBuildings[playerId].RemoveAt(deleteIndex);
            else return;

            Server.Resources.NavigationManager.RemoveBuildingFromMap(tilePos.X, tilePos.Y);

            UnregisterBuildingRequest response = new UnregisterBuildingRequest
            {
                DestroyPosX = tilePos.X,
                DestroyPosY = tilePos.Y,
                PlayerId = playerId
            };

            SentPacket sentPacket = new SentPacket(PacketsHeaders.UnregisterBuildingRequest);
            sentPacket.stringBuilder.Append(JsonSerializer.Serialize(response));

            Server.TCPServer.Broadcast(sentPacket);
        }
    }
}
