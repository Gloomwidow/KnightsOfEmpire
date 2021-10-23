using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Networking
{
    public class DataState
    {
        public int ConnectionID;

        public const int BufferSize = 8192;

        public byte[] buffer;
        public StringBuilder sb;

        public DataState(int connectionID)
        {
            ConnectionID = connectionID;
            buffer = new byte[BufferSize];
            sb = new StringBuilder();
        }
    }
}
