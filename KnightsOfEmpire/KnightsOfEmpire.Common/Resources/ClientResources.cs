using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Units;

namespace KnightsOfEmpire.Common.Resources
{
    /// <summary>
    /// Class for store client resources e.g. client nickname, client units package and other...
    /// </summary>
    public class ClientResources
    {
        public ClientResources()
        {
            Nickname = string.Empty;
            Map = null;
            CustomUnits = new CustomUnits();
        }

        public string Nickname { get; set; }

        public Map.Map Map { get; set; }

        public CustomUnits CustomUnits { get; set; }
    }
}
