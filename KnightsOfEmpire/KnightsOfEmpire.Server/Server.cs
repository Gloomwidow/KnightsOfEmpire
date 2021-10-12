using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Networking.TCP;

namespace KnightsOfEmpire.Server
{
    public class Server
    {
        protected static bool isRunning = true;
        public static TCPServer TCPServer { get; protected set; }
        static void Main(string[] args)
        {
            
            Console.WriteLine("Server starting up!");

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            TCPServer = new TCPServer("127.0.0.1", 26969, 4, 30);

            TCPServer.Start();

            if(!TCPServer.isRunning)
            {
                return;
            }

            while (isRunning)
            {
                var key = Console.ReadKey();
                if(key.Key==ConsoleKey.Escape)
                {
                    isRunning = false;
                }
            }

            TCPServer.Stop();
            Console.WriteLine("Server has stopped running!");
            Console.ReadKey();
        }


        // This method will close TCPServer correctly before console will exit
        protected static void OnExit(object sender, EventArgs e)
        {
            TCPServer.Stop();
        }
    }
}
