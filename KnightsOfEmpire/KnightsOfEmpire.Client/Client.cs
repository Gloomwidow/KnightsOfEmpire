using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Window;
using SFML.Graphics;
using SFML.System;
using KnightsOfEmpire.Common.Networking.TCP;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Definitions.GameStates;

namespace KnightsOfEmpire
{
    public class Client
    {
        /// <summary>
        /// Render Window for rendering graphics with SFML.
        /// </summary>
        public static RenderWindow RenderWindow { get; private set; }

        /// <summary>
        /// Time between each frame render. Use it to synchronize rendering with real-time, so frames per second won't have impact on graphic execution speed.
        /// </summary>
        public static float DeltaTime { get; private set; }

        /// <summary>
        /// Clock for counting Delta-Time.
        /// </summary>
        private static Clock DeltaTimeClock;

        private static Clock MessageToServerClock;

        private static GameState gameState;
        
        public static TCPClient TCPClient { get; protected set; }

        static void Main(string[] args)
        {
            DeltaTimeClock = new Clock();
            MessageToServerClock = new Clock();

            VideoMode mode = new VideoMode(1280, 720);
            RenderWindow = new RenderWindow(mode, "Knights Of Empire");

            TCPClient = new TCPClient("127.0.0.1", 26969);

            TCPClient.Start();

            RenderWindow.Closed += (obj, e) => 
            {
                TCPClient.Stop();
                RenderWindow.Close(); 
            };


            while (RenderWindow.IsOpen)
            {
                DeltaTime = DeltaTimeClock.Restart().AsSeconds();
                RenderWindow.DispatchEvents();
                RenderWindow.Clear();

                // Here should updates and rendering happen

                // Game state change
                if(GameLoop.NextGameState != null) 
                {
                    gameState.Dispose();
                    gameState = GameLoop.NextGameState;
                    GameLoop.NextGameState = null;
                    gameState.LoadResources();
                    gameState.Initialize();
                }
                gameState.Update();
                gameState.Render();


                if (TCPClient.isRunning)
                {
                    var ReceivedPackets = TCPClient.GetReceivedPackets();
                    if (ReceivedPackets.Count > 0) Console.WriteLine($"Packets received so far: {ReceivedPackets.Count}");
                    foreach (ReceivedPacket Packet in ReceivedPackets)
                    {
                        if (Packet.GetContent().IndexOf("4005 4") > -1)
                        {
                            Console.WriteLine("Server disconnected us due to no slots left");
                            TCPClient.Stop();
                            break;
                        }
                    }

                    if (TCPClient.isRunning && MessageToServerClock.ElapsedTime.AsSeconds() >= 2)
                    {
                        SentPacket pingPacket = new SentPacket();
                        pingPacket.stringBuilder.Append("2001 PING");
                        TCPClient.SendToServer(pingPacket);
                        MessageToServerClock.Restart();
                    }
                }

                RenderWindow.Display();
            }
        }
    }
}
