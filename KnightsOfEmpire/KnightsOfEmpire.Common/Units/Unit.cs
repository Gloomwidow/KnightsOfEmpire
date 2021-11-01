using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units
{
    public class Unit
    {
        public Vector2f Position { get; protected set; }
        public Vector2f MoveDirection { get; protected set; }
        public UnitStats Stats { get; protected set; }

        /// <summary>
        /// Texture Id which is used to render unit on map 
        /// </summary>
        public int TextureId { get; set; }

        /// <summary>
        /// Unit stance indicates what unit is doing. It can be used to play certain animations.
        /// </summary>
        public UnitStance Stance { get; set; }

        
    }
}
