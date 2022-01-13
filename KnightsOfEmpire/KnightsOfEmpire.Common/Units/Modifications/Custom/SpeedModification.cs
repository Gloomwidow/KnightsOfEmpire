using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications.Custom
{
    class SpeedModification : UnitUpgrade
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
}
