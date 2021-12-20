using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.Buildings
{
    public class MainBuildingLogic : GoldGenerationBuildingLogic
    {
        public MainBuildingLogic(int Gold, float GoldTime) : base(Gold, GoldTime)
        {

        }


        public override void OnDestroy()
        {
            Server.Resources.IsDefeated[BuildingInstance.PlayerId] = true;
        }
    }
}
