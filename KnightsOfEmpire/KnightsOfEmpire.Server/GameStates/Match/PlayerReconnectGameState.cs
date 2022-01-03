using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Buildings;
using KnightsOfEmpire.Common.Resources.Units;
using KnightsOfEmpire.Common.Resources.Waiting;
using KnightsOfEmpire.Common.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.GameStates.Match
{
    public class PlayerReconnectState : GameState
    {

        public bool[] DisconnectedRecently;

        public PlayerReconnectState()
        {
            DisconnectedRecently = new bool[MaxPlayerCount];
        }

        public override void HandleTCPPacket(ReceivedPacket packet)
        {
            string header = packet.GetHeader();
            switch (header)
            {
                case PacketsHeaders.WaitingStateClientRequest:
                    HandleReconnectingClient(packet);
                    break;
                case PacketsHeaders.GameReadyPing:
                    HandleGameReady(packet);
                    break;
            }
        }

        public override void Update()
        {
            base.Update();
            for(int i=0;i<MaxPlayerCount;i++)
            {
                if(!DisconnectedRecently[i])
                {
                    DisconnectedRecently[i] = !Server.TCPServer.IsClientConnected(i);
                }
            }
        }

        public void HandleReconnectingClient(ReceivedPacket packet)
        {
            Console.WriteLine("Got request to reconnect!");
            WaitingStateClientRequest request = null;
            request = packet.GetDeserializedClassOrDefault<WaitingStateClientRequest>();

            if (request == null)
            {
                Console.WriteLine("Error reading packet!");
                SendRefusal(packet.ClientID);
                return;
            }
            int position = -1;
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                if (Server.Resources.Nicknames[i].Equals(request.Nickname))
                {
                    position = i;
                    break;
                }
            }

            if (position == -1)
            {
                Console.WriteLine($"Nickname {request.Nickname} not found...");
                SendRefusal(packet.ClientID);
                return;
            }

            if (!DisconnectedRecently[position])
            {
                Console.WriteLine($"Nickname {request.Nickname} already connected");
                SendRefusal(packet.ClientID);
                return;
            }

            Console.WriteLine($"Nickname {request.Nickname} found on pos {position}");

            if (position != packet.ClientID)
            {
                Console.WriteLine($"Swapping needed!");
                Server.TCPServer.SwapConnection(packet.ClientID, position);
            }

            packet.ClientID = position;

            //send player id 
            WaitingStateServerResponse response = new WaitingStateServerResponse
            {
                Message = WaitingMessage.ServerOk,
                PlayerGameId = packet.ClientID,
                PlayerReadyStatus = new bool[MaxPlayerCount],
                PlayerNicknames = new string[MaxPlayerCount]
            };
            SentPacket idPacket = new SentPacket(PacketsHeaders.WaitingStateServerResponse, packet.ClientID);
            idPacket.stringBuilder.Append(JsonSerializer.Serialize(response));
            Server.TCPServer.SendToClient(idPacket);

            //send map packet
            SentPacket mapPacket = new SentPacket(PacketsHeaders.MapServerResponse, packet.ClientID);
            mapPacket.stringBuilder.Append(JsonSerializer.Serialize(Server.Resources.Map));
            Server.TCPServer.SendToClient(mapPacket);

            //start game on client
            SentPacket startGamePacket = new SentPacket(PacketsHeaders.StartGameServerRequest, packet.ClientID);
            StartGameServerRequest data = new StartGameServerRequest();
            data.CustomUnits = Server.Resources.GameCustomUnits;
            data.StartGame = true;

            startGamePacket.stringBuilder.Append(JsonSerializer.Serialize(data));
            Server.TCPServer.SendToClient(startGamePacket);           
        }

        public void HandleGameReady(ReceivedPacket packet)
        {
            if (!DisconnectedRecently[packet.ClientID]) return;

            DisconnectedRecently[packet.ClientID] = false;
            //send buildings to client 
            List<Building>[] buildings = parent.GetSiblingGameState<BuildingUpdateState>().GameBuildings;

            foreach (List<Building> list in buildings)
            {
                foreach (Building b in list)
                {
                    RegisterBuildingRequest buildingRequest = new RegisterBuildingRequest
                    {
                        BuildingPosX = b.Position.X,
                        BuildingPosY = b.Position.Y,
                        BuildingTypeId = b.BuildingId,
                        PlayerId = b.PlayerId
                    };

                    SentPacket buildingPacket = new SentPacket(PacketsHeaders.RegisterBuildingRequest, packet.ClientID);
                    buildingPacket.stringBuilder.Append(JsonSerializer.Serialize(buildingRequest));
                    Server.TCPServer.SendToClient(buildingPacket);
                }
            }

            //send units to player
            List<Unit>[] units = parent.GetSiblingGameState<UnitUpdateState>().GameUnits;

            foreach (List<Unit> list in units)
            {
                foreach (Unit u in list)
                {
                    RegisterUnitRequest unitRequest = new RegisterUnitRequest
                    {
                        ID = u.ID,
                        PlayerId = u.PlayerId,
                        UnitTypeId = u.UnitTypeId,
                        StartPositionX = u.Position.X,
                        StartPositionY = u.Position.Y
                    };

                    SentPacket unitPacket = new SentPacket(PacketsHeaders.GameUnitRegisterRequest, packet.ClientID);
                    unitPacket.stringBuilder.Append(JsonSerializer.Serialize(unitRequest));
                    Server.TCPServer.SendToClient(unitPacket);
                }
            }
        }

        protected void SendRefusal(int clientID)
        {
            WaitingStateServerResponse ReadErrorResponse = new WaitingStateServerResponse
            {
                Message = WaitingMessage.ServerRefuse,
            };
            SentPacket readErrorPacket = new SentPacket(PacketsHeaders.WaitingStateServerResponse, clientID);
            readErrorPacket.stringBuilder.Append(JsonSerializer.Serialize(ReadErrorResponse));
            Server.TCPServer.SendToClient(readErrorPacket);
            Server.TCPServer.DisconnectClient(clientID);
        }
    }
}
