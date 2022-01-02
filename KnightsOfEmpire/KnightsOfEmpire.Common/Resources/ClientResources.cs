using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Helper;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Common.Units.Modifications;

namespace KnightsOfEmpire.Common.Resources
{
    /// <summary>
    /// Class for store client resources e.g. client nickname, client units package and other...
    /// </summary>
    public class ClientResources
    {
        public int PlayerGameId { get; set; }
        public ClientResources()
        {
            Nickname = string.Empty;
            Map = null;
            PlayerCustomUnits = UnitUpgradeManager.LoadCustomUnitsFromFile();
            GameCustomUnits = new CustomUnits[Constants.MaxPlayers];
        }

        public string Nickname { get; set; }

        public Map.Map Map { get; set; }

        public CustomUnits PlayerCustomUnits { get; set; }
        public CustomUnits[] GameCustomUnits { get; set; }

        public int GoldAmount = 0;
        public int UnitCapacity = 0;
        public int MaxUnitCapacity = 0;
        public int PlayersLeft = 0;
    }
}
