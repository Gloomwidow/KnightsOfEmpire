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

        public float AttackExecuteTime { get; set; }
        public float AttackDistance { get; set; }

        public float BuildingsDamageMultiplier { get; set; }

        public int TrainCost { get; set; }

        public int VisibilityDistance { get; set; } // In map tile

        public UnitStats()
        {
            Health = 100;
            MaxHealth = 100;
            MovementSpeed = 64;
            AttackDamage = 25;
            AttackSpeed = 0.5f;
            AttackExecuteTime = 1f; // It is not update
            AttackDistance = 64;
            BuildingsDamageMultiplier = 1.0f;
            TrainCost = 25;
            VisibilityDistance = 5;
        }

        public void Clamp()
        {
            Health = Math.Max(1, Health);
            MaxHealth = Math.Max(1, MaxHealth);
            MovementSpeed = Math.Max(1, MovementSpeed);
            AttackDamage = Math.Max(1, AttackDamage);
            AttackSpeed = Math.Max(0.01f, AttackSpeed);
            AttackDistance = Math.Max(Unit.UnitSize + 1, AttackDistance);
            BuildingsDamageMultiplier = Math.Max(0.01f, BuildingsDamageMultiplier);
            TrainCost = Math.Max(1, TrainCost);
            VisibilityDistance = Math.Max(1, VisibilityDistance);
        }

        //TO-DO: Add any other parameters that will be needed for troops or upgrades system.
    }
}
