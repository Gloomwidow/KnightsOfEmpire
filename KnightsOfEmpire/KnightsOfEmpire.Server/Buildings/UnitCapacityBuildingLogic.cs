using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.Buildings
{
    public class UnitCapacityBuildingLogic : BuildingLogic
    {
        public int Capacity { get; protected set; }

        public UnitCapacityBuildingLogic(int Capacity)
        {
            this.Capacity = Capacity;
        }

        public override void OnCreate()
        {
            Server.Resources.IncreaseCapacity(BuildingInstance.PlayerId, Capacity);
        }

        public override void OnDestroy()
        {
            Server.Resources.DecreaseCapacity(BuildingInstance.PlayerId, Capacity);
        }
    }
}
