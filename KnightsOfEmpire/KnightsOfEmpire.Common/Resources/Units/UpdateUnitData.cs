using KnightsOfEmpire.Common.Units;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Units
{
    [Serializable]
    public class UpdateUnitData
    {
        public int PlayerId { get; set; }

        public char[] UnitId { get; set; }

        public int Health { get; set; }

        public float MoveDirectionX { get; set; }

        public float MoveDirectionY { get; set; }

        public float PositionX { get; set; }

        public float PositionY { get; set; }

        public UnitStance Stance { get; set; }

        public UpdateUnitData() { }

        public UpdateUnitData(Unit unit, int playerID)
        {
            PlayerId = playerID;

            UnitId = unit.ID;
            Health = unit.Stats.Health;
            MoveDirectionX = unit.MoveDirection.X;
            MoveDirectionY = unit.MoveDirection.Y;
            PositionX = unit.Position.X;
            PositionY = unit.Position.Y;
            Stance = unit.Stance;
        }
    }
}
