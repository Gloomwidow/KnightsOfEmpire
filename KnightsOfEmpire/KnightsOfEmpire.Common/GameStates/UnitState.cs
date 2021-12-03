using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.GameStates
{
    public class UnitState : GameState
    {
        public const int MaxPlayerCount = 4;

        public List<Unit>[] GameUnits;
        public override void Initialize()
        {
            GameUnits = new List<Unit>[4];
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                GameUnits[i] = new List<Unit>();
            }
        }

        public List<Unit> GetFriendlyUnitsInRange(Unit unit, float range)
        {
            List<Unit> neighbors = new List<Unit>();
            List<Unit> friendlies = GameUnits[unit.PlayerId];
            for (int i=0;i<friendlies.Count;i++)
            {
                Unit another = friendlies[i];
                if (another.EqualID(unit.ID)) continue;
                if(another.Position.Distance2(unit.Position)<=range*range)
                {
                    neighbors.Add(another);
                }
            }
            return neighbors;
        }

        public List<Unit> GetEnemyUnitsInRange(Unit unit, float range)
        {
            List<Unit> targets = new List<Unit>();
            Unit nearest = null;
            for (int j = 0; j < MaxPlayerCount; j++)
            {
                if (j == unit.PlayerId) continue;
                List<Unit> enemies = GameUnits[j];
                for (int i = 0; i < enemies.Count; i++)
                {
                    Unit another = enemies[i];
                    float currDist = another.Position.Distance2(unit.Position);
                    if (currDist <= range * range)
                    {
                        if (nearest == null) nearest = another;
                        else
                        {
                            if (currDist < nearest.Position.Distance2(unit.Position))
                            {
                                targets.Add(nearest);
                                nearest = another;
                            }
                            else targets.Add(another);
                        }
                    }
                }
            }
            if(nearest!=null)
            {
                targets.Add(nearest);
            }
            return targets;
        }
    }
}
