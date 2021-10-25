using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units
{
    /// <summary>
    /// Holds every unit's stat, along with some read-only values that can be calculated basing on them.
    /// </summary>
    public class UnitStats
    {
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public float HealthPercentage
        {
            get
            {
                return (float)(Health * 1.00f / MaxHealth * 1.00f);
            }
        }
        public float MovementSpeed { get; set; }
        public int AttackDamage { get; set; }
        public float AttackSpeed { get; set; }
        public float AttackDistance { get; set; }

        //TO-DO: Add any other parameters that will be needed for troops or upgrades system.
    }
}
