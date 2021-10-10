using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Test;

namespace KnightsOfEmpire.Server
{
    public class Server
    {
        static void Main(string[] args)
        {
            Class1 c = new Class1(45);
            Console.WriteLine("Server starting up!");
            Console.ReadKey();
        }
    }
}
