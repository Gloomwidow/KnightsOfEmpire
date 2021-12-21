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
        public int BuildingId { get; set; }
        public int BuildCost { get; set; }
        public Vector2i Position { get; set; }
        public int PlayerId { get; set; }
        public int TextureId { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
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
        }
        public bool EqualPosition(Vector2i position)
        {
            if (Position.Equals(position)) 
            {
                return true;
            }
            return false;
        }
    }
}
