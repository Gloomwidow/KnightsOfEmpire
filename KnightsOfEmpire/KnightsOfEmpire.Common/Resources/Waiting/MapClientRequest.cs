using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Waiting
{
    [Serializable]
    public class MapClientRequest
    {
        public bool MapReceived { get; set; }

        public MapClientRequest()
        {
            MapReceived = false;
        }
    }
}
