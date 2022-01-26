using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Custom
{
    public class AttackModification : UnitUpgrade
    {
        public AttackModification()
        {
            this.Name = "Heavy Swing";
            this.Description = "+20 Damage, -50% Attack Speed";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage += 20;
            u.Stats.AttackSpeed *= 0.5f;
        }
    }

    public class AttackModification2 : UnitUpgrade
    {
        public AttackModification2()
        {
            this.Name = "Strength Training";
            this.Description = "+5 Damage, +10 Cost";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage += 5;
            u.Stats.TrainCost += 10;
        }
    }

    public class AttackModification3 : UnitUpgrade
    {
        public AttackModification3()
        {
            this.Name = "Eagle Eye";
            this.Description = "+1 View Distance, +1 Attack Range, +25 Cost";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDistance += Map.Map.TilePixelSize;
            u.Stats.TrainCost += 50;
            u.Stats.VisibilityDistance+=1;
        }
    }

    public class AttackModification4 : UnitUpgrade
    {
        public AttackModification4()
        {
            this.Name = "Blinding Rage";
            this.Description = "+5 Damage, +10% Attack Speed, -2 View Distance";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage += 5;
            u.Stats.AttackSpeed += new UnitStats().AttackSpeed * 0.1f;
            u.Stats.VisibilityDistance -= 2;
        }
    }

    public class AttackModification5 : UnitUpgrade
    {
        public AttackModification5()
        {
            this.Name = "Demolition Expert";
            this.Description = "+20% Building Multiplier, +25 Cost";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.TrainCost += 25;
            u.Stats.BuildingsDamageMultiplier += 0.2f;
        }
    }


}
