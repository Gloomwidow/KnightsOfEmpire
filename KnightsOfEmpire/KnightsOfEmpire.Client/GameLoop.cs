using KnightsOfEmpire.Definitions.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire
{
    public static class GameLoop
    {
        private static GameState nextGameState = null;
        public static GameState NextGameState 
        {
            get
            {
                return nextGameState;
            }
            set
            {
                nextGameState = value;
            }
        }
        
    }
}
