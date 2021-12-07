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

        public float AttackProgress { get; set; }

        public UnitStance Stance { get; set; }

        public UpdateUnitData() { }

        public UpdateUnitData(Unit unit, int playerID)
        {
            PlayerId = playerID;

            UnitId = unit.ID;
            Health = unit.Stats.Health;
            MoveDirectionX = (float)Math.Round(unit.MoveDirection.X, 3, MidpointRounding.AwayFromZero);
            MoveDirectionY = (float)Math.Round(unit.MoveDirection.Y, 3, MidpointRounding.AwayFromZero);
            PositionX = (float)Math.Round(unit.Position.X, 3, MidpointRounding.AwayFromZero);
            PositionY = (float)Math.Round(unit.Position.Y, 3, MidpointRounding.AwayFromZero);
            AttackProgress = (float)Math.Round(unit.AttackProgress, 2, MidpointRounding.AwayFromZero);
            Stance = unit.Stance;
        }
    }
}
