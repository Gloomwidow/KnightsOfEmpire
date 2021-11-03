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
        public bool SendMap { get; set; }
        public bool MapRecived { get; set; }

        public MapClientRequest()
        {
            SendMap = false;
            MapRecived = false;
        }
    }
}
