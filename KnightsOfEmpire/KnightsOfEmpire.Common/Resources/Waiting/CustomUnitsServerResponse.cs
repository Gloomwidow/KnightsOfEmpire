using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Waiting
{
    [Serializable]
    public class CustomUnitsServerResponse
    {
        public bool IsUnitsReceived { get; set; }

        public CustomUnitsServerResponse()
        {
            IsUnitsReceived = false;
        }
    }
}
