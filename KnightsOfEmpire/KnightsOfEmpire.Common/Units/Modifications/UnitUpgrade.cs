using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications
{
    public class UnitUpgrade
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual void Upgrade(Unit u)
        {

        }
    }
}
