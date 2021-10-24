using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Resources
{
    /// <summary>
    /// Class for store client resources e.g. client nickname, client units package and other...
    /// </summary>
    public class ClientResources
    {
        public ClientResources()
        {
            Nickname = "";
        }

        public string Nickname { get; set; }
    }
}
