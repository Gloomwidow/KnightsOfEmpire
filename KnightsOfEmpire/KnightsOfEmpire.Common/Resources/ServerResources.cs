using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnightsOfEmpire.Common.Units;

namespace KnightsOfEmpire.Common.Resources
{
    public class ServerResources
    {
        public Map.Map Map { get; set; }

        public string[] Nicknames { get; set; }

        public CustomUnits[] CustomUnits { get; set; }

        public ServerResources(int maxPlayers)
        {
            Nicknames = new string[maxPlayers];
            CustomUnits = new CustomUnits[maxPlayers];
            Map = null;
        }

    }
}
