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
        public string Name { get; set; }
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
        public string Description { get; set; }
        public virtual void OnCreate() { }
        public virtual void Update() { }
        public virtual void OnDestroy() { }
    }
}
