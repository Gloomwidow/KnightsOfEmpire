using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Archetypes
{
    public class RangedArchetypeUnitUpgrade : UnitUpgrade
    {
        public RangedArchetypeUnitUpgrade()
        {
            this.Name = "Ranged Type";
            this.Description = "Ranged Units have increased their range by 4 tiles.";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDistance += Map.Map.TilePixelSize * 4;
        }
    }
}
