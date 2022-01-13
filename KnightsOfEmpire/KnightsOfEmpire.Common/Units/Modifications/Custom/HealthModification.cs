using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Custom
{
    public class HealthModification : UnitUpgrade
    {
        public HealthModification()
        {
            this.Name = "Health";
            this.Description = "Health: +10   Cost: +30";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MaxHealth += 10;
            u.Stats.Health += 10;
            u.Stats.TrainCost += 30;
        }
    }

    public class HealthModification2 : UnitUpgrade
    {
        public HealthModification2()
        {
            this.Name = "Health";
            this.Description = "Health: +50% Cost: +50 MovmentSpeed: -50%";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MaxHealth += u.Stats.MaxHealth/2;
            u.Stats.Health += u.Stats.Health / 2;
            u.Stats.TrainCost += 50;
            u.Stats.MovementSpeed *= 0.5f;
        }
    }
}
