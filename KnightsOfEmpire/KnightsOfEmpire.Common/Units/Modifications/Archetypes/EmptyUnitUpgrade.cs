using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Archetypes
{
    public class EmptyUnitUpgrade : UnitUpgrade
    {
        public EmptyUnitUpgrade()
        {
            this.Name = "No Upgrade";
            this.Description = "You can choose custom upgrade for this unit.";
        }
    }
}
