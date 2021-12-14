using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using TGUI;

using KnightsOfEmpire.Common.Networking.TCP;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Resources;

using KnightsOfEmpire.GameStates;
using KnightsOfEmpire.Common.Networking.UDP;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.GameStates.Match;

namespace KnightsOfEmpire
{
    public class Client
    {
        /// <summary>
        /// Render Window for rendering graphics with SFML.
        /// </summary>
        public static RenderWindow RenderWindow { get; private set; }
        

        /// <summary>
        /// GUI for interact with user.
        /// </summary>
        public static Gui Gui { get; private set; }

        /// <summary>
        /// Time between each frame render. Use it to synchronize rendering with real-time, so frames per second won't have impact on graphic execution speed.
        /// </summary>
        public static float DeltaTime { get; private set; }

        /// <summary>
        /// Class to storage client resource e.g. nickname, units set and other...
        /// </summary>
        public static ClientResources Resources { get; private set; }

        /// <summary>
        /// Clock for counting Delta-Time.
        /// </summary>
        private static Clock DeltaTimeClock;

        private static Clock MessageToServerClock;

        public static TCPClient TCPClient { get; set; }

        public static UDPClient UDPClient { get; set; }


        static void Main(string[] args)
        {
            DeltaTimeClock = new Clock();
            MessageToServerClock = new Clock();

            VideoMode mode = new VideoMode(1280, 720);
            RenderWindow = new RenderWindow(mode, "Knights Of Empire");

            Gui = new Gui(RenderWindow);

            RenderWindow.Resized += (sender, e) =>
            {
                Gui.View = new View(new FloatRect(new Vector2f(0, 0), new Vector2f(RenderWindow.Size.X, RenderWindow.Size.Y)));
            };


            RenderWindow.Closed += (obj, e) => 
            {
                if(TCPClient != null)
                {
                    TCPClient.Stop();
                }
                if(UDPClient != null)
                {
                    UDPClient.Stop();
                }
                RenderWindow.Close(); 
            };

            // Add client resource
            Resources = new ClientResources();
            // Default nickname
            Random rand = new Random();
            Resources.Nickname = "Test"+rand.Next(10000).ToString();

            // Add first GameState
            GameStateManager.GameState = new MainState();

            while (RenderWindow.IsOpen)
            {
                DeltaTime = DeltaTimeClock.Restart().AsSeconds();

                RenderWindow.DispatchEvents();
                RenderWindow.Clear();
               

                //UDPClient works as a additional connection client for TCPClient
                //Since UDP is connectionless, we won't know, if it successfully connected to the server.
                //TCP can tell that, so if TCPClient is down, this code will also disable UDPClient
                if (TCPClient != null && UDPClient != null)
                {
                    if (!TCPClient.isRunning && UDPClient.isRunning)
                    {
                        UDPClient.Stop();
                    }
                }
                
                GameStateManager.UpdateState();


                if (TCPClient != null && TCPClient.isRunning)
                {
                    if (MessageToServerClock.ElapsedTime.AsSeconds() >= 5)
                    {
                        SentPacket pingPacket = new SentPacket(PacketsHeaders.PING);
                        pingPacket.stringBuilder.Append("PING");
                        TCPClient.SendToServer(pingPacket);
                        MessageToServerClock.Restart();
                    }
                }

                if (GameStateManager.GameState != null)
                {
                    if (TCPClient != null && TCPClient.isRunning)
                    {
                        GameStateManager.GameState.HandleTCPPackets(TCPClient.GetReceivedPackets());
                    }
                    if (UDPClient != null && UDPClient.isRunning)
                    {
                        GameStateManager.GameState.HandleUDPPackets(UDPClient.GetReceivedPackets());
                    }
                    GameStateManager.GameState.Update();
                    GameStateManager.GameState.Render();
                }


                RenderWindow.Display();
            }
        }
    }
}
