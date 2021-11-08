using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Units
{
    [Serializable]
    public class UpdateUnitsResponse
    {
        /// <summary>
        /// Convert DateTime to long
        /// </summary>
        public long TimeStamp { get; set; }

        public UpdateUnitData[] UnitData { get; set; }
    }
}
