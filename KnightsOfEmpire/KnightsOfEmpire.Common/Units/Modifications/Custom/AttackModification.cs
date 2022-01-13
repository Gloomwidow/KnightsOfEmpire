using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Custom
{
    class AttackModification : UnitUpgrade
    {
        public AttackModification()
        {
            this.Name = "Attack";
            this.Description = "Damage: +100% Cost: +100% Attack Speed: -40%";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage *= 2;
            u.Stats.TrainCost *= 2;
            u.Stats.AttackSpeed *= 0.6f;
        }
    }


}
