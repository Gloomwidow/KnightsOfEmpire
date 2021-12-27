using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Server.GameStates.Match;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.Buildings
{
    public class FriendlyHealingBuildingLogic : BuildingLogic
    {
        public int HealthGain { get; protected set; }
        public float HealDistance { get; protected set; }
        public float GenerationTime { get; protected set; }
        public FriendlyHealingBuildingLogic(int HealthGain, float HealDistance, float GenerationTime)
        {
            this.HealthGain = HealthGain;
            this.HealDistance = HealDistance;
            this.GenerationTime = GenerationTime;
        }

        public override void OnUpdate()
        {
            BuildingInstance.Timer += Server.DeltaTime;
            if (BuildingInstance.Timer >= GenerationTime)
            {
                BuildingInstance.Timer %= GenerationTime;
                List<Unit> friendly = GameStateManager.GameState.GetSiblingGameState<UnitUpdateState>().
                    GameUnits[BuildingInstance.PlayerId];
                Vector2f pos = new Vector2f(BuildingInstance.Position.X * Map.TilePixelSize +(Map.TilePixelSize/2),
                    BuildingInstance.Position.Y * Map.TilePixelSize + (Map.TilePixelSize/2));
                for(int i=0;i<friendly.Count;i++)
                {
                    Unit u = friendly[i];
                    
                    if(u.Position.Distance2(pos)<=HealDistance*HealDistance)
                    {
                        u.Stats.Health = Math.Min(u.Stats.MaxHealth, u.Stats.Health + HealthGain);
                    }
                }
            }
        }
    }
}
