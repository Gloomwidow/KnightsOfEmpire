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
            this.Name = "Scouts";
            this.Description = "+1 View Distance, +10% Movement Speed, +50 cost";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.VisibilityDistance += 1;
            u.Stats.TrainCost += 50;
            u.Stats.MovementSpeed += new UnitStats().MovementSpeed * 0.1f;
        }
    }

    public class ViewModification2 : UnitUpgrade
    {
        public ViewModification2()
        {
            this.Name = "Spies";
            this.Description = "+2 View Distance, -10% Health, -10% Attack";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.VisibilityDistance += 2;
            u.Stats.MaxHealth -= (int)(new UnitStats().MaxHealth * 0.1f);
            u.Stats.Health -= (int)(new UnitStats().MaxHealth * 0.1f);
            u.Stats.AttackDamage -= (int)(new UnitStats().AttackDamage * 0.1f);
        }
    }
}
