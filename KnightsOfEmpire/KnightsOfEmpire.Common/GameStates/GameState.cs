using KnightsOfEmpire.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.GameStates
{
    public abstract class GameState
    {
        public virtual void LoadResources() { }
        public virtual void Initialize() { }
        public virtual void HandleTCPPackets(List<ReceivedPacket> packets) { }
        public virtual void HandleUDPPackets(List<ReceivedPacket> packets) { }
        public virtual void Update() { }
        public virtual void Render() { }
        public virtual void Dispose() { }

    }
}
