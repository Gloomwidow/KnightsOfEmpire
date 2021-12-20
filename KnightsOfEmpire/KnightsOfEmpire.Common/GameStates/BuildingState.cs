using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Buildings;

namespace KnightsOfEmpire.Common.GameStates
{
    public class BuildingState : GameState
    {
        public List<Building>[] GameBuildings;

        public override void Initialize()
        {
            GameBuildings = new List<Building>[4];
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                GameBuildings[i] = new List<Building>();
            }
        }
    }
}
