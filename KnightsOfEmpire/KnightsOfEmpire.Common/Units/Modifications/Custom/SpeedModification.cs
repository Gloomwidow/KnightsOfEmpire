using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Custom
{
    public class SpeedModification : UnitUpgrade
    {
        public SpeedModification()
        {
            this.Name = "Speed";
            this.Description = "Speed: +10   Cost: +40";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MovementSpeed += 10;
            u.Stats.TrainCost += 40;
        }
    }

    public class SpeedModification2 : UnitUpgrade
    {
        public SpeedModification2()
        {
            this.Name = "Speed";
            this.Description = "Speed: +50% Cost: +20 Health: -50%";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MovementSpeed *= 1.5f;
            u.Stats.TrainCost += 20;

            u.Stats.MaxHealth /= 2;
            u.Stats.Health /= 2;
        }
    }
}
