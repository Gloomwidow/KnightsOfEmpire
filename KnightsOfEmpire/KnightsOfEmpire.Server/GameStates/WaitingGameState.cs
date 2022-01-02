﻿using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources;
using KnightsOfEmpire.Common.Resources.Waiting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Units.Modifications;

namespace KnightsOfEmpire.Server.GameStates
{
    public class WaitingGameState : GameState
    {
        protected string[] Nicknames;
        protected bool[] ReadyStatus;
        protected bool[] IsNicknameChecked;
        protected bool[] IsMapSend;
        protected bool[] IsUnitsReceived;

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


        public override void Update()
        {
            var TCPServer = Server.TCPServer;

            for (int i = 0; i < TCPServer.MaxConnections; i++)
            {
                if (!TCPServer.IsClientConnected(i))
                {
                    Nicknames[i] = string.Empty;
                    ReadyStatus[i] = false;
                    IsNicknameChecked[i] = false;
                    IsMapSend[i] = false;
                    IsUnitsReceived[i] = false;
                }
            }

            if ((DateTime.Now-lastResponseTime).TotalSeconds>=SendInfoDelay)
            {
                WaitingStateServerResponse WaitingRoomStatus = new WaitingStateServerResponse
                {
                    Message = WaitingMessage.ServerOk,
                    PlayerNicknames = Nicknames,
                    PlayerReadyStatus = ReadyStatus
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

                            Console.WriteLine("Server: Map was send to client " + i.ToString());
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
            if(Server.TCPServer.CurrentActiveConnections < 1)
            {
                return false;
            }

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
            mapPacket.stringBuilder.Append(JsonSerializer.Serialize(Server.Resources.Map));
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

            packet.stringBuilder.Append(JsonSerializer.Serialize(data));
            Server.TCPServer.SendToClient(packet);
        }

        // Handle request
        

        private void HandleWaitingClientRequest(ReceivedPacket packet)
        {
            string received = packet.GetContent();
            WaitingStateClientRequest request = null;
            try
            {
                request = JsonSerializer.Deserialize<WaitingStateClientRequest>(received);
            }
            catch (Exception ex)
            {
                WaitingStateServerResponse ReadErrorResponse = new WaitingStateServerResponse
                {
                    Message = WaitingMessage.ServerRefuse,
                };
                SentPacket readErrorPacket = new SentPacket(PacketsHeaders.WaitingStateServerResponse, packet.ClientID);
                readErrorPacket.stringBuilder.Append(JsonSerializer.Serialize(ReadErrorResponse));
                Server.TCPServer.SendToClient(readErrorPacket);
                Server.TCPServer.DisconnectClient(packet.ClientID);
            }

            if (request != null)
            {
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
                }
            }
        }


        private void HandleMapClientRequest(ReceivedPacket packet)
        {
            string received = packet.GetContent();
            MapClientRequest request = null;
            try
            {
                request = JsonSerializer.Deserialize<MapClientRequest>(received);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                IsMapSend[packet.ClientID] = false;
            }

            if(request != null)
            {
                IsMapSend[packet.ClientID] = request.MapReceived;
            }
            else
            {
                IsMapSend[packet.ClientID] = false;
            }
        }

        private void HandleCustomUnitsClientRequest(ReceivedPacket packet)
        {
            CustomUnits request = packet.GetDeserializedClassOrDefault<CustomUnits>();
            if (request == null)
            {
                IsUnitsReceived[packet.ClientID] = false;
                SendCustomUnitsResponse(packet.ClientID, false);
                return;
            }
            if (!UnitUpgradeManager.IsCustomUnitsValid(request))
            {
                IsUnitsReceived[packet.ClientID] = false;
                SendCustomUnitsResponse(packet.ClientID, false);
                return;
            }
            IsUnitsReceived[packet.ClientID] = true;
            Server.Resources.GameCustomUnits[packet.ClientID] = request;
            Console.WriteLine("Custom units saved succesfully");
            SendCustomUnitsResponse(packet.ClientID, true);
        }
    }
}
