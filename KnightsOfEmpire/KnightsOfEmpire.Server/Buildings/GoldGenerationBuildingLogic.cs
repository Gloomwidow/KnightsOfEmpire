using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.Buildings
{
    public class GoldGenerationBuildingLogic : BuildingLogic
    {
        public int GoldGain { get; protected set; }
        public float GenerationTime { get; protected set; }

        public GoldGenerationBuildingLogic(int GoldGain, float GenerationTime)
        {
            this.GoldGain = GoldGain;
            this.GenerationTime = GenerationTime;
        }

        public override void OnUpdate()
        {
            BuildingInstance.Timer += Server.DeltaTime;
            if(BuildingInstance.Timer >= GenerationTime)
            {
                BuildingInstance.Timer %= GenerationTime;
                Server.Resources.AddGold(BuildingInstance.PlayerId, GoldGain);
            }
        }
    }
}
