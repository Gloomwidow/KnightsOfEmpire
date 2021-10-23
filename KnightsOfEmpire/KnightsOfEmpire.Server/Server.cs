using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Networking.TCP;
using KnightsOfEmpire.Common.Networking.UDP;

namespace KnightsOfEmpire.Server
{
    public class Server
    {
        protected static bool isRunning = true;
        public static TCPServer TCPServer { get; protected set; }

        public static UDPServer UDPServer { get; protected set; }

        private static Task TestUDPBroadcast;
        static void Main(string[] args)
        {
            
            Console.WriteLine("Server starting up!");

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            TCPServer = new TCPServer("127.0.0.1", 26969, 4, 30);

            TCPServer.Start();

            UDPServer = new UDPServer("127.0.0.1", 26969);

            UDPServer.Start();

            if(!TCPServer.isRunning)
            {
                UDPServer.Stop();
                return;
            }

            TestUDPBroadcast = new Task(() =>
            {
                while(isRunning)
                {
                    Thread.Sleep(100);
                    
                    for (int i = 0; i < TCPServer.MaxConnections; i++)
                    {
                        var address = TCPServer.GetClientAddress(i);
                        if (address != null)
                        {
                            SentPacket packet = new SentPacket();
                            packet.stringBuilder.Append($"{DateTime.Now.ToString()} UDP IS COOL {i}");
                            UDPServer.SentTo(packet, address, i);
                        }
                    }
                    
                }
            });

            TestUDPBroadcast.Start();
            while (isRunning)
            {
                
            }

            UDPServer.Stop();
            TCPServer.Stop();
            Console.WriteLine("Server has stopped running!");
            Console.ReadKey();
        }


        // This method will close TCPServer correctly before console will exit
        protected static void OnExit(object sender, EventArgs e)
        {
            isRunning = false;
            TestUDPBroadcast.Wait();
            UDPServer.Stop();
            TCPServer.Stop();
        }
    }
}
