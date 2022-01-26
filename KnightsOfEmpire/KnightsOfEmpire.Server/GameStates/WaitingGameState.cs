using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Helper;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Waiting;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Common.Units.Modifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace KnightsOfEmpire.Server.GameStates
{
    public class WaitingGameState : GameState
    {
        protected string[] Nicknames;
        protected bool[] ReadyStatus;
        protected int[] SelectedMap;
        protected bool[] IsNicknameChecked;
        protected bool[] IsMapSend;
        protected bool[] IsUnitsReceived;

        protected bool DataChanged = true;

        protected DateTime lastResponseTime;

        protected int SendInfoDelay = 1;

        public override void Initialize()
        {
            Server.Resources.Reset();
            Nicknames = new string[Server.TCPServer.MaxConnections];
            ReadyStatus = new bool[Server.TCPServer.MaxConnections];
            IsNicknameChecked = new bool[Server.TCPServer.MaxConnections];
            IsMapSend = new bool[Server.TCPServer.MaxConnections];
            IsUnitsReceived = new bool[Server.TCPServer.MaxConnections];
            SelectedMap = new int[Server.TCPServer.MaxConnections];
            lastResponseTime = DateTime.Now;
        }

        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            foreach (ReceivedPacket packet in packets)
            {
                string header = packet.GetHeader();
                switch(header)
                {
                    case PacketsHeaders.PING:
                        break;

                    case PacketsHeaders.WaitingStateClientRequest:
                        HandleWaitingClientRequest(packet);
                        break;

                    case PacketsHeaders.MapClientRequest:
                        HandleMapClientRequest(packet);
                        break;

                    case PacketsHeaders.CustomUnitsClientRequest:
                        HandleCustomUnitsClientRequest(packet);
                        break;
                }
            }
        }

        private int PickMap()
        {
            int[] mapVotes = new int[Server.Resources.MapsPool.Length];
            int mostVotes = 0;
            for(int i=0;i<MaxPlayerCount;i++)
            {
                if(Server.TCPServer.IsClientConnected(i)) mapVotes[SelectedMap[i]]++;
            }
            foreach(int votes in mapVotes)
            {
                if (mostVotes < votes) mostVotes = votes;
            }
            List<int> votedMaps = new List<int>();
            for(int i=0;i< mapVotes.Length;i++)
            {
                if (mapVotes[i] == mostVotes) votedMaps.Add(i);
            }
            if (votedMaps.Count == 1) return votedMaps[0];
            else
            {
                Random rand = new Random();
                int select = rand.Next(0, votedMaps.Count);
                return votedMaps[select];
            }
        }


        public override void Update()
        {
            var TCPServer = Server.TCPServer;

            for (int i = 0; i < TCPServer.MaxConnections; i++)
            {
                if (!TCPServer.IsClientConnected(i))
                {
                    Nicknames[i] = string.Empty;
                    ReadyStatus[i] = false;
                    SelectedMap[i] = 0;
                    IsNicknameChecked[i] = false;
                    IsMapSend[i] = false;
                    IsUnitsReceived[i] = false;
                }
            }

            if (DataChanged)
            {
                DataChanged = false;
                WaitingStateServerResponse WaitingRoomStatus = new WaitingStateServerResponse
                {
                    Message = WaitingMessage.ServerOk,
                    PlayerNicknames = Nicknames,
                    PlayerReadyStatus = ReadyStatus,
                    PlayerSelectedMap = SelectedMap
                };

                for (int i = 0; i < TCPServer.MaxConnections; i++)
                {
                    if (TCPServer.IsClientConnected(i))
                    {
                        SentPacket waitingInfoPacket = new SentPacket(PacketsHeaders.WaitingStateServerResponse);
                        WaitingRoomStatus.PlayerGameId = i;
                        waitingInfoPacket.stringBuilder.Append(JsonSerializer.Serialize(WaitingRoomStatus));
                        waitingInfoPacket.ClientID = i;
                        TCPServer.SendToClient(waitingInfoPacket);

                        //Send map packet
                        if(IsMapSend[i] == false)
                        {
                            SendMapPacket(i);
                            Logger.Log("Server: Map was send to client " + i.ToString());
                        }

                        if(IsUnitsReceived[i] == false)
                        {
                            SendCustomUnitsResponse(i, false);
                        }
                    }
                }
                lastResponseTime = DateTime.Now;
            }

            // Check start game conditions
            if(CheckStartGameCondition())
            {
                Server.Resources.Nicknames = Nicknames;

                for (int i = 0; i < Server.TCPServer.MaxConnections; i++)
                {
                    if (Server.TCPServer.IsClientConnected(i))
                    {
                        SendStartGameRequest(i);
                    }
                }
                Server.Resources.PlayersLeft = Server.TCPServer.CurrentActiveConnections;
                Server.Resources.StartPlayerCount = Server.TCPServer.CurrentActiveConnections;
                GameStateManager.GameState = new MatchGameState();
            }

        }

        bool CheckStartGameCondition()
        {
            if(Server.TCPServer.CurrentActiveConnections < 1) return false;

            for(int i = 0; i < Server.TCPServer.MaxConnections; i++)
            {
                if(Server.TCPServer.IsClientConnected(i))
                {
                    if(!IsNicknameChecked[i] || !IsMapSend[i] || !IsUnitsReceived[i] || !ReadyStatus[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Send packets

        private void SendMapPacket(int clientID)
        {
            SentPacket mapPacket = new SentPacket(PacketsHeaders.MapServerResponse, clientID);
            mapPacket.stringBuilder.Append(JsonSerializer.Serialize(Server.Resources.MapsPool.ToArray()));
            Server.TCPServer.SendToClient(mapPacket);
        }

        private void SendCustomUnitsResponse(int clientID, bool isAccepted)
        {
            SentPacket packet = new SentPacket(PacketsHeaders.CustomUnitsServerResponse, clientID);
            CustomUnitsServerResponse data = new CustomUnitsServerResponse();
            data.IsUnitsReceived = isAccepted;

            packet.stringBuilder.Append(JsonSerializer.Serialize(data));
            Server.TCPServer.SendToClient(packet);
        }

        private void SendStartGameRequest(int clientID)
        {
            SentPacket packet = new SentPacket(PacketsHeaders.StartGameServerRequest, clientID);
            StartGameServerRequest data = new StartGameServerRequest();
            data.StartGame = true;
            for(int i=0;i<MaxPlayerCount;i++)
            {
                data.CustomUnits[i] = Server.Resources.GameCustomUnits[i];
            }
            data.MapID = PickMap();
            Server.Resources.CurrentMapId = data.MapID;
            Server.Resources.Map = new Common.Map.Map(Server.Resources.MapsPool[data.MapID].MapName);
            packet.stringBuilder.Append(JsonSerializer.Serialize(data));
            Server.TCPServer.SendToClient(packet);
        }

        private void HandleWaitingClientRequest(ReceivedPacket packet)
        {
            WaitingStateClientRequest request =
                packet.GetDeserializedClassOrDefault<WaitingStateClientRequest>();
            DataChanged = true;
            if (request==null)
            {
                WaitingStateServerResponse ReadErrorResponse = new WaitingStateServerResponse
                {
                    Message = WaitingMessage.ServerRefuse,
                };
                SentPacket readErrorPacket = new SentPacket(PacketsHeaders.WaitingStateServerResponse, packet.ClientID);
                readErrorPacket.stringBuilder.Append(JsonSerializer.Serialize(ReadErrorResponse));
                Server.TCPServer.SendToClient(readErrorPacket);
                Server.TCPServer.DisconnectClient(packet.ClientID);
                return;
            }
            // Check nicknames one times
            if (!IsNicknameChecked[packet.ClientID])
            {
                IsNicknameChecked[packet.ClientID] = true;
                foreach (string nickname in Nicknames)
                {
                    if (request.Nickname == nickname)
                    {
                        IsNicknameChecked[packet.ClientID] = false;

                        WaitingStateServerResponse ReadErrorResponse = new WaitingStateServerResponse
                        {
                            Message = WaitingMessage.ServerChangeNick,
                        };
                        SentPacket readErrorPacket = new SentPacket(PacketsHeaders.WaitingStateServerResponse, packet.ClientID);
                        readErrorPacket.stringBuilder.Append(JsonSerializer.Serialize(ReadErrorResponse));
                        Server.TCPServer.SendToClient(readErrorPacket);
                        Server.TCPServer.DisconnectClient(packet.ClientID);

                        break;
                    }
                }
            }
            if (IsNicknameChecked[packet.ClientID])
            {
                Nicknames[packet.ClientID] = request.Nickname;
                ReadyStatus[packet.ClientID] = request.IsReady;
                SelectedMap[packet.ClientID] = request.SelectedMap;
            }
        }


        private void HandleMapClientRequest(ReceivedPacket packet)
        {
            MapClientRequest request = packet.GetDeserializedClassOrDefault<MapClientRequest>();
            if(request==null)
            {
                DataChanged = true;
                IsMapSend[packet.ClientID] = false;
                return;
            }
            IsMapSend[packet.ClientID] = request.MapReceived;
        }

        private void HandleCustomUnitsClientRequest(ReceivedPacket packet)
        {
            CustomUnits request = packet.GetDeserializedClassOrDefault<CustomUnits>();
            if (request == null)
            {
                DataChanged = true;
                IsUnitsReceived[packet.ClientID] = false;
                SendCustomUnitsResponse(packet.ClientID, false);
                return;
            }
            if (!UnitUpgradeManager.IsCustomUnitsValid(request))
            {
                DataChanged = true;
                IsUnitsReceived[packet.ClientID] = false;
                SendCustomUnitsResponse(packet.ClientID, false);
                return;
            }
            IsUnitsReceived[packet.ClientID] = true;
            Server.Resources.GameCustomUnits[packet.ClientID] = request;
            Logger.Log("Custom units saved succesfully");
            SendCustomUnitsResponse(packet.ClientID, true);
        }
    }
}
