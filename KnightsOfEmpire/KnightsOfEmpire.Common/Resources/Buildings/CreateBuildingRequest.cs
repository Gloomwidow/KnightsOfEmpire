using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Buildings
{
    [Serializable]
    public class CreateBuildingRequest
    {
        public int BuildingTypeId { get; set; }
        public int BuildingPosX { get; set; }
        public int BuildingPosY { get; set; }
    }
}
