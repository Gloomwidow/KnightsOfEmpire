using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Units.Groups;
using SFML.System;
using System;
using System.Collections.Generic;


using KnightsOfEmpire.Common.Resources.Units;

namespace KnightsOfEmpire.Common.Units
{
    public class Unit
    {

        public UnitGroup UnitGroup;

        public bool IsGroupCompleted { get; set; }

        public const int UnitSize = 32;
        public const int UnitAvoidanceDistance = (int)(UnitSize*1.44f); // UnitSize*sqrt(2)
        public const int UnitFlockVisionDistance =  2*UnitAvoidanceDistance;
        public const float UnitDirectionRotationSpeed = 3.0f;

        public const float UnitAvoidanceWeight = 0.2f;
        public const float UnitMovementWeight = 0.8f;


        /// <summary>
        /// Unique 2 chars to identify units
        /// </summary>
        public int PlayerId { get; set; }
        public char[] ID { get; set; }
        public Vector2f PreviousPosition { get; set; }
        public Vector2f Position { get; set; }
        public Vector2f MoveDirection { get; set; }

        protected Vector2f TargetDirection { get; set; }
        public UnitStats Stats { get; set; }
        public bool IsSelected { get; set; }




        public float AttackProgress { get; protected set; }

        protected bool HasAttacked { get; set; }

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

        public void UpdateData(UpdateUnitData data)
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
            int avoidCount = 0;
            if(neighbors.Count>0)
            {
                foreach(Unit unit in neighbors)
                {
                    avoidCount++;
                    avoidanceBehaviour += (Position - unit.Position);
                }
                avoidanceBehaviour.X /= avoidCount;
                avoidanceBehaviour.Y /= avoidCount;
            }
           
            Vector2f MovementVector = flowDirection.Normalized() * Stats.MovementSpeed;
            Vector2f AvoidanceVector = avoidanceBehaviour * Stats.MovementSpeed * UnitAvoidanceWeight;

            TargetDirection = MovementVector + AvoidanceVector;
            if(TargetDirection.Length2()>Stats.MovementSpeed*Stats.MovementSpeed)
            {
                TargetDirection = TargetDirection.Normalized() * Stats.MovementSpeed;
            }
        }   

        public void Move(float DeltaTime)
        {
            PreviousPosition = new Vector2f(Position.X, Position.Y);
            if (TargetDirection.Length2() == 0.0f) MoveDirection = new Vector2f(0.0f, 0.0f);
            else
            {
                MoveDirection = MoveDirection.LerpTimeStep(TargetDirection, UnitDirectionRotationSpeed, DeltaTime);
            }
            Position += MoveDirection * DeltaTime;
            if (MoveDirection.Length2() != 0.0) Stance = UnitStance.Moving;
            else Stance = UnitStance.Idle;
        }
        
        public void Attack(float DeltaTime, List<Unit> targets)
        {
            if (targets.Count <= 0)
            {
                AttackProgress = 0.0f;
                HasAttacked = false;
                return;
            }

            Stance = UnitStance.Attacking;
            AttackProgress += Stats.AttackSpeed * DeltaTime;
            if(!HasAttacked && AttackProgress>=Stats.AttackExecuteTime)
            {
                Unit u = targets[targets.Count - 1];
                u.Stats.Health -= Stats.AttackDamage;
                HasAttacked = true;
            }
            if(AttackProgress>=1.0f)
            {
                AttackProgress %= 1.0f;
                HasAttacked = false;
            }  
        }
        
    }
}
