using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Archetypes
{
    public class CavalryArchetypeUnitUpgrade : UnitUpgrade
    {
        public CavalryArchetypeUnitUpgrade()
        {
            this.Name = "Cavalry Type";
            this.Description = "Cavalry Units have their speed increased by 50%.";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MovementSpeed*=1.5f;
        }
    }
}
