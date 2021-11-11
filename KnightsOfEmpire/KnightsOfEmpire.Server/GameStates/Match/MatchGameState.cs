using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Server.GameStates.Match;

namespace KnightsOfEmpire.Server.GameStates
{
    public class MatchGameState : GameState
    {

        protected UnitUpdateState unitState = new UnitUpdateState();
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
            }
        }

        public override void Update()
        {
            unitState.Update();
        }
    }
}
