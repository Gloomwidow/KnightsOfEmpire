using KnightsOfEmpire.Common.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.GameStates
{
    public class UnitState : GameState
    {
        public const int MaxPlayerCount = 4;

        public List<Unit>[] GameUnits;
        public override void Initialize()
        {
            GameUnits = new List<Unit>[4];
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                GameUnits[i] = new List<Unit>();
            }
        }
    }
}
