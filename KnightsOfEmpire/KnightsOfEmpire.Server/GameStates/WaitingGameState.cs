using KnightsOfEmpire.Common.GameStates;
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

namespace KnightsOfEmpire.Server.GameStates
{
    public class WaitingGameState : GameState
    {
        protected string[] Nicknames;
        protected bool[] ReadyStatus;
        protected bool[] IsNicknameChecked;

        protected DateTime lastResponseTime;

        protected int SendInfoDelay = 1;

        public override void Initialize()
        {
            Nicknames = new string[Server.TCPServer.MaxConnections];
            ReadyStatus = new bool[Server.TCPServer.MaxConnections];
            IsNicknameChecked = new bool[Server.TCPServer.MaxConnections];
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
                        HandleWaitnigClientRequest(packet);
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
                SentPacket waitingInfoPacket = new SentPacket(PacketsHeaders.WaitingStateServerResponse);

                waitingInfoPacket.stringBuilder.Append(JsonSerializer.Serialize(WaitingRoomStatus));

                for (int i = 0; i < TCPServer.MaxConnections; i++)
                {
                    if (TCPServer.IsClientConnected(i))
                    {
                        waitingInfoPacket.ClientID = i;
                        TCPServer.SendToClient(waitingInfoPacket);
                    }
                }
                lastResponseTime = DateTime.Now;
            }
        }


        private void HandleWaitnigClientRequest(ReceivedPacket packet)
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
    }
}
