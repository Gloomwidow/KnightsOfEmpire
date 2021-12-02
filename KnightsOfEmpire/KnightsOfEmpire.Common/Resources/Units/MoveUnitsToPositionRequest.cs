using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Units
{
    [Serializable]
    public class MoveUnitsToPositionRequest
    {
        // map coordinates of goal
        public int GoalX { get; set; }
        public int GoalY { get; set; }

        public bool IgnoreAttack { get; set; }

        // IDs of units that are in group
        public string[] UnitIDs;
    }
}
