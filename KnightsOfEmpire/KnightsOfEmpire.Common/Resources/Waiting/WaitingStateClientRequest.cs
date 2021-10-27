using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Waiting
{
    [Serializable]
    public class WaitingStateClientRequest
    {
        public string Nickname { get; set; }

        public bool IsReady { get; set; }
    }
}
