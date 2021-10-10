using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Test;
using SFML.Window;
using SFML.Graphics;
using SFML.System;

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

        static void Main(string[] args)
        {
            Class1 c = new Class1(45);
            DeltaTimeClock = new Clock();

            VideoMode mode = new VideoMode(1280, 720);
            RenderWindow = new RenderWindow(mode, "Knights Of Empire");

            RenderWindow.Closed += (obj, e) => { RenderWindow.Close(); };


            while (RenderWindow.IsOpen)
            {
                DeltaTime = DeltaTimeClock.Restart().AsSeconds();
                RenderWindow.DispatchEvents();
                RenderWindow.Clear();

                // Here should updates and rendering happen

                RenderWindow.Display();
            }
        }
    }
}
