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
using KnightsOfEmpire.Common.GameStates;

using KnightsOfEmpire.GameStates;

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

        public static TCPClient TCPClient { get; set; }

        static void Main(string[] args)
        {
            DeltaTimeClock = new Clock();
            MessageToServerClock = new Clock();

            VideoMode mode = new VideoMode(1280, 720);
            RenderWindow = new RenderWindow(mode, "Knights Of Empire");

            RenderWindow.Closed += (obj, e) => 
            {
                if(TCPClient != null)
                {
                    TCPClient.Stop();
                }
                RenderWindow.Close(); 
            };

            // Add first GameState
            GameStateManager.GameState = new ConnectState();

            while (RenderWindow.IsOpen)
            {
                DeltaTime = DeltaTimeClock.Restart().AsSeconds();

                RenderWindow.DispatchEvents();
                RenderWindow.Clear();

                
                GameStateManager.UpdateState();


                if (TCPClient != null && TCPClient.isRunning)
                {
                    if (MessageToServerClock.ElapsedTime.AsSeconds() >= 2)
                    {
                        SentPacket pingPacket = new SentPacket();
                        pingPacket.stringBuilder.Append("2001 PING");
                        TCPClient.SendToServer(pingPacket);
                        MessageToServerClock.Restart();
                    }
                }

                if (GameStateManager.GameState != null)
                {
                    if (TCPClient != null && TCPClient.isRunning)
                    {
                        GameStateManager.GameState.HandlePackets(TCPClient.GetReceivedPackets());
                    }
                    GameStateManager.GameState.Update();
                    GameStateManager.GameState.Render();
                }


                RenderWindow.Display();
            }
        }
    }
}
