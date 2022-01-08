using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Units.Groups;
using SFML.System;
using System;
using System.Collections.Generic;
using KnightsOfEmpire.Common.Resources.Units;
using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.Units.Enum;

namespace KnightsOfEmpire.Common.Units
{
    public class Unit
    {
        public UnitGroup UnitGroup;

        public UnitGroup PreviousCompletedGroup;

        public bool IsGroupCompleted { get; set; }

        public bool IsInAttackRange { get; set; }

        public bool IsFacingLeft = false;

        public int UnitTypeId { get; set; } 

        public AttackTarget AttackTarget = AttackTarget.None;

        public const int UnitSize = 32;
        public const int UnitAvoidanceDistance = (int)(UnitSize*1.44f); // UnitSize*sqrt(2)
        public const int UnitGroupVisionDistance =  (int)(1.2f*UnitAvoidanceDistance);
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
            AttackProgress = data.AttackProgress;
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
            if (Stance == UnitStance.Attacking && UnitGroup==null) TargetDirection = new Vector2f(0.0f, 0.0f);
            if (TargetDirection.Length2() <= float.Epsilon) MoveDirection = new Vector2f(0.0f, 0.0f);
            MoveDirection = MoveDirection.LerpTimeStep(TargetDirection, UnitDirectionRotationSpeed, DeltaTime);
            if (MoveDirection.Length2() >= float.Epsilon)
            {
                Position += MoveDirection * DeltaTime;
                Stance = UnitStance.Moving;
            }
            else Stance = UnitStance.Idle;
        }

        public void Attack(float DeltaTime, List<Unit> targetUnits, List<Building> targetBuildings)
        {
            if (Stance != UnitStance.Idle) return;
            if(AttackTarget==AttackTarget.None)
            {
                if (targetUnits.Count > 0) AttackTarget = AttackTarget.Unit;
                else if (targetBuildings.Count > 0) AttackTarget = AttackTarget.Building;
            }

            if (AttackTarget == AttackTarget.None) return;

            if ((AttackTarget == AttackTarget.Unit && targetUnits.Count <= 0) ||
            (AttackTarget == AttackTarget.Building && targetBuildings.Count <= 0))
            {
                ResetAttack();
                return;
            }

            Stance = UnitStance.Attacking;
            AttackProgress += Stats.AttackSpeed * DeltaTime;
            if (!HasAttacked && AttackProgress >= Stats.AttackExecuteTime)
            {
                if (AttackTarget == AttackTarget.Unit)
                {
                    Unit u = targetUnits[targetUnits.Count - 1];
                    u.Stats.Health -= Stats.AttackDamage;
                }
                else
                {
                    Building b = targetBuildings[targetBuildings.Count - 1];
                    b.Health -= (int)(Stats.AttackDamage*Stats.BuildingsDamageMultiplier);    
                }
                HasAttacked = true;
            }
            if (AttackProgress >= 1.0f)
            {
                ResetAttack();
            }
        }  
        
        protected void ResetAttack()
        {
            Stance = UnitStance.Idle;
            AttackProgress = 0.0f;
            AttackTarget = AttackTarget.None;
            HasAttacked = false;
        }
    }
}
