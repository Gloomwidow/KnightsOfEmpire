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

        protected DateTime lastResponseTime;

        protected int SendInfoDelay = 1;

        public override void Initialize()
        {
            Nicknames = new string[Server.TCPServer.MaxConnections];
            ReadyStatus = new bool[Server.TCPServer.MaxConnections];
            lastResponseTime = DateTime.Now;
        }

        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            foreach (ReceivedPacket packet in packets)
            {
                string received = packet.GetContent();
                if(received.StartsWith("2001 PING"))
                {
                    continue;
                }
                WaitingStateClientRequest request = null;
                try
                {
                    request = JsonSerializer.Deserialize<WaitingStateClientRequest>(received);
                }
                catch(Exception ex)
                {
                    WaitingStateServerResponse ReadErrorResponse = new WaitingStateServerResponse
                    {
                        Message = WaitingMessage.ServerRefuse,
                    };
                    SentPacket readErrorPacket = new SentPacket(packet.ClientID);
                    readErrorPacket.stringBuilder.Append(JsonSerializer.Serialize(ReadErrorResponse));
                    Server.TCPServer.SendToClient(readErrorPacket);
                    Server.TCPServer.DisconnectClient(packet.ClientID);
                }

                if(request != null)
                {
                    Nicknames[packet.ClientID] = request.Nickname;
                    ReadyStatus[packet.ClientID] = request.IsReady;
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
                SentPacket waitingInfoPacket = new SentPacket();

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
    }
}
