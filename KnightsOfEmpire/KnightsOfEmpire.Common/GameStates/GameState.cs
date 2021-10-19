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
        public abstract void LoadResources();
        public abstract void Initialize();
        public abstract void HandlePackets(List<ReceivedPacket> packets);
        public abstract void Update();
        public abstract void Render();
        public abstract void Dispose();

    }
}
