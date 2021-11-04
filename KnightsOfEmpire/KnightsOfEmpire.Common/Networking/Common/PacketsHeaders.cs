using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Networking
{
    public static class PacketsHeaders
    {
        public const string PING = "2001";

        public const string WaitingStateClientRequest = "3001";
        public const string WaitingStateServerResponse = "3002";

        public const string MapClientRequest = "3101";
        public const string MapServerResponse = "3102";

        public const string CustomUnitsClientRequest = "3201";
        public const string CustomUnitsServerResponse = "3202";

        public const string StartGameServerRequest = "3300";
    }
}
