using KnightsOfEmpire.Common.Units;
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

        public float RotationAngle { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public UnitStance Stance { get; set; }
    }
}
