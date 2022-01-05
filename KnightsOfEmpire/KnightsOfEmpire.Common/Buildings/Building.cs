using KnightsOfEmpire.Common.Resources;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Buildings
{
    public class Building
    {

        public float Timer = 0;
        public int BuildingId { get; set; }
        public int BuildCost { get; set; }
        public Vector2i Position { get; set; }
        public int PlayerId { get; set; }
        public int TextureId { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int TrainType { get; set; } = -1;
        public float HealthPercentage
        {
            get
            {
                return (float)(Health * 1.00f / MaxHealth * 1.00f);
            }
        }

        public Building() { }

        public Building(Building original)
        {
            MaxHealth = original.MaxHealth;
            Health = MaxHealth;
            BuildCost = original.BuildCost;
            TextureId = original.TextureId;
            TrainType = original.TrainType;
        }
    }
}
