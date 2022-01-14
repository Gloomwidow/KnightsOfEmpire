using KnightsOfEmpire.Common.Helper;
using KnightsOfEmpire.Common.Units.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications
{
    [Serializable]
    public class CustomUnit
    {
        public string Name { get; set; }

        public UnitType UnitType { get; set; }

        public int TextureId { get; set; }

        // This must not include unit archetype upgrade!!!
        public int[] UpgradeList { get; set; }

        public CustomUnit(UnitType UnitType, int TextureId, string name = "")
        {
            this.Name = name;
            this.UnitType = UnitType;
            this.TextureId = TextureId;
            UpgradeList = new int[Constants.MaxUpgradesPerUnit];
        }
    }
}
