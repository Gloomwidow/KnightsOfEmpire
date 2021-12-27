using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Units
{
    [Serializable]
    public class UpdateUnitsResponse : UDPBaseRequest
    {
        public UpdateUnitData[] UnitData { get; set; }
    }
}
