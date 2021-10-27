using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources.Waiting
{
    [Serializable]
    public class WaitingStateServerResponse
    {
        public WaitingMessage Message { get; set; }

        public string[] PlayerNicknames { get; set; }

        public bool[] PlayerReadyStatus { get; set; }
    }
}
