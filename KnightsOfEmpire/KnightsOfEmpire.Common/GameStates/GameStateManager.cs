using KnightsOfEmpire.Common.GameStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.GameStates
{
    public static class GameStateManager
    {
        private static GameState nextGameState = null;
        private static GameState currentGameState = null;
        public static GameState GameState
        {
            get
            {
                return currentGameState;
            }
            set
            {
                nextGameState = value;
            }
        }

        public static void UpdateState()
        {
            if(nextGameState!=null)
            {
                if(currentGameState != null)
                {
                    currentGameState.Dispose();
                }
                currentGameState = nextGameState;
                nextGameState = null;
                currentGameState.LoadResources();
                currentGameState.Initialize();
            }
        }
    }
}
