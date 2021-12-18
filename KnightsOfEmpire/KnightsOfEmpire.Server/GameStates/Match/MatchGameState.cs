using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Player;
using KnightsOfEmpire.Server.GameStates.Match;

namespace KnightsOfEmpire.Server.GameStates
{
    public class MatchGameState : GameState
    {
        protected UnitUpdateState unitState = new UnitUpdateState();
        protected BuildingUpdateState buildingState = new BuildingUpdateState();
        public override void Initialize()
        {
            Console.WriteLine("Start MatchGameState on server");
            unitState.Initialize();
        }

        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            foreach(ReceivedPacket packet in packets)
            {
                if (packet.GetHeader().StartsWith(PacketsHeaders.GameUnitHeaderStart))
                {
                    unitState.HandleTCPPacket(packet);
                }
                if (packet.GetHeader().StartsWith(PacketsHeaders.BuildingHeaderStart)) 
                {
                    buildingState.HandleTCPPacket(packet);
                }
            }
        }

        public override void Update()
        {
            if(Server.TCPServer.CurrentActiveConnections<=0)
            {
                Console.WriteLine("No one is connected. Resetting to WaitingState");
                GameStateManager.GameState = new WaitingGameState();
            }
            unitState.Update();
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                if (Server.Resources.HasChanged[i]) 
                {
                    ChangePlayerInfoRequest request = new ChangePlayerInfoRequest
                    {
                        PlayerId = i,
                        GoldAmount = Server.Resources.GoldAmount[i],
                        IsDefeated = Server.Resources.IsDefeated[i],
                        CurrentUnitsCapacity = Server.Resources.CurrentUnitsCapacity[i],
                        MaxUnitsCapacity = Server.Resources.MaxUnitsCapacity[i]
                    };
                    SentPacket registerPacket = new SentPacket(PacketsHeaders.ChangePlayerInfoRequest, -1);
                    registerPacket.stringBuilder.Append(JsonSerializer.Serialize(request));

                    registerPacket.ClientID = i;
                    Server.TCPServer.SendToClient(registerPacket);

                    Server.Resources.HasChanged[i] = false;
                }
            }
        }

        public override void Dispose()
        {
            
        }
    }
}
