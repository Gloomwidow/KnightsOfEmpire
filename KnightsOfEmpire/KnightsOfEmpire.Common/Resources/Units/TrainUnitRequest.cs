using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Units
{
    [Serializable]
    public class TrainUnitRequest
    {
        public int UnitTypeId { get; set; }

        // We will pass coordinates of the building in which unit is produces/spawned
        public int BuildingPosX { get; set; }
        public int BuildingPosY { get; set; }
    }
}
