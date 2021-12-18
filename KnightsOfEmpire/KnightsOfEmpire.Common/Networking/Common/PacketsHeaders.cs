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


        // Everything for units will have header starting with 50
        public const string GameUnitHeaderStart = "50";

        public const string GameUnitRegisterRequest = "5010";
        public const string GameUnitUnregisterRequest = "5011";

        public const string GameUnitUpdateRequest = "5050";

        public const string GameUnitsMoveToTileRequest = "5051";

        public const string GameUnitTrainRequest = "5055";

        // Everything for building will have header starting with 60
        public const string BuildingHeaderStart = "60";

        public const string BuildingCreateRequest = "6010";

        // Update player info such as gold, capacity etc.
        public const string ChangePlayerInfoRequest = "7010";
    }
}
