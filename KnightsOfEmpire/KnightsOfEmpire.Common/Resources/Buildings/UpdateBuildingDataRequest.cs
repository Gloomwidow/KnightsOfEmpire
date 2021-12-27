using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Buildings
{
    [Serializable]
    public class UpdateBuildingDataRequest : UDPBaseRequest
    {
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int PlayerId { get; set; }
        public int Health { get; set; }
    }
}
