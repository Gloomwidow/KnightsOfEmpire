using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Units.Groups;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KnightsOfEmpire.Common.Resources.Units;

namespace KnightsOfEmpire.Common.Units
{
    public class Unit
    {

        public UnitGroup UnitGroup;

        public const int UnitSize = 32;
        public const int UnitAvoidanceDistance = UnitSize + 16;


        /// <summary>
        /// Unique 2 chars to identify units
        /// </summary>
        public int PlayerId { get; set; }
        public char[] ID { get; set; }
        public Vector2f PreviousPosition { get; set; }
        public Vector2f Position { get; set; }
        public Vector2f MoveDirection { get; set; }
        public UnitStats Stats { get; set; }
        public bool IsSelected { get; set; }

        /// <summary>
        /// Texture Id which is used to render unit on map 
        /// </summary>
        public int TextureId { get; set; }
        /// <summary>
        /// Unit stance indicates what unit is doing. It can be used to play certain animations.
        /// </summary>
        public UnitStance Stance { get; set; }

        public bool EqualID(char[] otherID)
        {
            if (ID.Length != otherID.Length) return false;
            for(int i=0;i<ID.Length;i++)
            {
                if (ID[i] != otherID[i]) return false;
            }
            return true;
        }

        public void UppdateData(UpdateUnitData data)
        {
            if(!EqualID(data.UnitId))
            {
                throw new ArgumentException("Error unit data");
            }

            Position = new Vector2f(data.PositionX, data.PositionY);
            MoveDirection = new Vector2f(data.MoveDirectionX, data.MoveDirectionY);
            Stats.Health = data.Health;
            Stance = data.Stance;
        }
        

        public void Update(Vector2f flowDirection, List<Unit> neighbors)
        {
            Vector2f avoidanceBehaviour = new Vector2f(0, 0);
            if(neighbors.Count>0)
            {
                foreach(Unit unit in neighbors)
                {
                    avoidanceBehaviour += (Position - unit.Position);
                }
                avoidanceBehaviour.X /= neighbors.Count;
                avoidanceBehaviour.Y /= neighbors.Count;
            }
            MoveDirection = (flowDirection + avoidanceBehaviour).Normalized() * Stats.MovementSpeed;
        }   

        public void Move(float DeltaTime)
        {
            PreviousPosition = new Vector2f(Position.X, Position.Y);
            Position += MoveDirection * DeltaTime;
            if (MoveDirection.Length() != 0.0) Stance = UnitStance.Moving;
            else Stance = UnitStance.Idle;
        }
        
        public void Attack(List<Unit> targets)
        {
            // TO-DO: Implement Attack
        }
        
    }
}
