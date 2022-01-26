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
            this.Name = "Athletics Training";
            this.Description = "+10% Speed, +15 Cost";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.MovementSpeed += new UnitStats().MovementSpeed*0.1f;
            u.Stats.TrainCost += 15;
        }
    }

    public class SpeedModification2 : UnitUpgrade
    {
        public SpeedModification2()
        {
            this.Name = "Armor Lightening";
            this.Description = "+20% Movement Speed, -10% Health, +20 Cost";
        }

        public override void Upgrade(Unit u)
        {
            u.Stats.TrainCost += 20;
            u.Stats.MovementSpeed += new UnitStats().MovementSpeed*0.2f;
            u.Stats.MaxHealth -= (int)(new UnitStats().MaxHealth*0.1f);
            u.Stats.Health -= (int)(new UnitStats().MaxHealth * 0.1f);
        }
    }
}
