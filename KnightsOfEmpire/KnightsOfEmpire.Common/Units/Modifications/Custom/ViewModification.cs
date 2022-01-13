using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Custom
{
    public class ViewModification : UnitUpgrade
    {
        public ViewModification()
        {
            this.Name = "View";
            this.Description = "View Distance: +50% Cost: +20 Health: -50%";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.VisibilityDistance += u.Stats.VisibilityDistance / 2;
            u.Stats.TrainCost += 20;

            u.Stats.MaxHealth /= 2;
            u.Stats.Health /= 2;
        }
    }

    public class ViewModification2 : UnitUpgrade
    {
        public ViewModification2()
        {
            this.Name = "View";
            this.Description = "View Distance: +1 Cost: +10 Damage: -10";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.VisibilityDistance += 1;
            u.Stats.TrainCost += 10;
            u.Stats.AttackDamage -= 10;
        }
    }
}
