using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Networking.TCP;
using KnightsOfEmpire.Common.Networking.UDP;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Server.GameStates;

namespace KnightsOfEmpire.Server
{
    public class Server
    {
        protected static bool isRunning = true;
        public static TCPServer TCPServer { get; protected set; }

        public static UDPServer UDPServer { get; protected set; }

        /// <summary>
        /// Time for server to do one gamestate loop
        /// </summary>
        private static float TickRate = 1.0f / 60.0f;

        private static Stopwatch TickTimer;

        static void Main(string[] args)
        {
            UnitIdManager.SetupIds(2);
            Console.WriteLine("Server starting up!");

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);
            TickTimer = new Stopwatch();

            TCPServer = new TCPServer("127.0.0.1", 26969, 4, 30);

            TCPServer.Start();

            UDPServer = new UDPServer("127.0.0.1", 26969);

            UDPServer.Start();

            if(!TCPServer.isRunning)
            {
                UDPServer.Stop();
                return;
            }

            GameStateManager.GameState = new WaitingGameState();

            while (isRunning)
            {
                TickTimer.Restart();
                GameStateManager.UpdateState();
                if (GameStateManager.GameState != null)
                {
                    if (TCPServer.isRunning)
                    {
                        GameStateManager.GameState.HandleTCPPackets(TCPServer.GetReceivedPackets());
                    }
                    GameStateManager.GameState.Update();
                }
                TickTimer.Stop();

                float elapsedTime = TickTimer.ElapsedMilliseconds / 1000.0f;
                // The server will wait for the next update in case of doing it much faster
                if(elapsedTime<=TickRate)
                {
                    float Difference = TickRate - elapsedTime;
                    Thread.Sleep((int)(Difference * 1000));
                }
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
            UDPServer.Stop();
            TCPServer.Stop();
        }
    }
}
