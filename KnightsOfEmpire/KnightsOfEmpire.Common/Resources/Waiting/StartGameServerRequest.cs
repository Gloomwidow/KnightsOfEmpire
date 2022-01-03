using KnightsOfEmpire.Common.Helper;
using KnightsOfEmpire.Common.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Waiting
{
    public class StartGameServerRequest
    {
        public bool StartGame { get; set; }

        public int GameId { get; set; }
        public CustomUnits[] CustomUnits { get; set; }

        public StartGameServerRequest()
        {
            StartGame = true;
            CustomUnits = new CustomUnits[Constants.MaxPlayers];
        }
    }
}
