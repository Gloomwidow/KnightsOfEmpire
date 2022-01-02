using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Archetypes
{
    public class MeleeArchetypeUnitUpgrade : UnitUpgrade
    {
        public MeleeArchetypeUnitUpgrade()
        {
            this.Name = "Melee Type";
            this.Description = "Meele Units have increased max health by 50 points.";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MaxHealth += 50;
            u.Stats.Health += 50;
        }
    }
}
