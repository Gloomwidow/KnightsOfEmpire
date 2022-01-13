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
            this.Name = "Attack";
            this.Description = "Damage: +100% Cost: +200% Attack Speed: -40%";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage *= 2;
            u.Stats.TrainCost *= 3;
            u.Stats.AttackSpeed *= 0.6f;
        }
    }

    public class AttackModification2 : UnitUpgrade
    {
        public AttackModification2()
        {
            this.Name = "Attack";
            this.Description = "Damage: +10 Cost: +50";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage += 10;
            u.Stats.TrainCost += 50;
        }
    }

    public class AttackModification3 : UnitUpgrade
    {
        public AttackModification3()
        {
            this.Name = "Attack";
            this.Description = "Damage: +20 Cost: +40 MovmentSpeed: -50%";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage += 20;
            u.Stats.TrainCost += 50;
            u.Stats.MovementSpeed *= 0.5f;
        }
    }

    public class AttackModification4 : UnitUpgrade
    {
        public AttackModification4()
        {
            this.Name = "Attack";
            this.Description = "Damage: -50% Cost: +40 AttackDistance: +50%";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage /= 2;
            u.Stats.TrainCost += 40;
            u.Stats.AttackDistance *= 1.5f;
        }
    }

    public class AttackModification5 : UnitUpgrade
    {
        public AttackModification5()
        {
            this.Name = "Attack Buldings";
            this.Description = "Damage: +10 Cost: +100\nBuildingsDamageMultiplier +20%";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.AttackDamage += 10;
            u.Stats.TrainCost += 100;
            u.Stats.BuildingsDamageMultiplier *= 1.2f;
        }
    }


}
