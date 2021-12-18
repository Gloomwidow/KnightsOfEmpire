using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Player
{
    [Serializable]
    public class ChangePlayerInfoRequest
    {
        public int PlayerId { get; set; }
        public int GoldAmount { get; set; }

        public bool IsDefeated { get; set; }

        public int CurrentUnitsCapacity { get; set; }

        public int MaxUnitsCapacity { get; set; }
    }
}
