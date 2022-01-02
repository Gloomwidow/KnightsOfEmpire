using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Archetypes
{
    public class SiegeArchetypeUnitUpgrade : UnitUpgrade
    {
        public SiegeArchetypeUnitUpgrade()
        {
            this.Name = "Siege Type";
            this.Description = "Siege Units deal 25% increased damage to buildings.";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.BuildingsDamageMultiplier += 0.5f;
        }
    }
}
