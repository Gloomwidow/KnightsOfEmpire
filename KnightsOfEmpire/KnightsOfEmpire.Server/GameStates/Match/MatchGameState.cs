using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;

namespace KnightsOfEmpire.Server.GameStates
{
    public class MatchGameState : GameState
    {
        public override void Initialize()
        {
            // TODO: Fill
            Console.WriteLine("Start MatchGameState on server");
        }

        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            // TODO: Fill
        }

        public override void Update()
        {
            // TODO: Fill
        }
    }
}
