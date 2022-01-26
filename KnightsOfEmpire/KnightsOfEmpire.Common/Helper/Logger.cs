using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Helper
{
    public static class Logger
    {
        public const bool VerboseLog = false;

        public static void Log(string msg)
        {
            if (VerboseLog) Console.WriteLine(msg);
        }
    }
}
