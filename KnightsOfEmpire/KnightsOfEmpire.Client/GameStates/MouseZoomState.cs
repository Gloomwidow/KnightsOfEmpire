using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using TGUI;

using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Networking.TCP;
using KnightsOfEmpire.Common.Networking.UDP;
using System.Net;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Navigation;
using System.Runtime.InteropServices;

namespace KnightsOfEmpire.GameStates
{
    public class MouseZoomState : GameState
    {
        public static float gameZoom = 1;
        private float minimumZoom = 0.5f;
        private float maximumZoom = 5.0f;
        private float gameZoopSpeed = 25.0f;

        void OnMouseScroll(object sender, EventArgs e)
        {
            RenderWindow window = (RenderWindow)sender;
            MouseWheelScrollEventArgs mouseEvent = (MouseWheelScrollEventArgs)e;
            if ((mouseEvent.Delta < 0 && gameZoom >= maximumZoom) || (mouseEvent.Delta > 0 && gameZoom <= minimumZoom)) return;
            gameZoom -= gameZoopSpeed * mouseEvent.Delta * Client.DeltaTime;
            gameZoom = Math.Max(minimumZoom, Math.Min(maximumZoom, gameZoom));
        }

        public override void Initialize()
        {
            Client.RenderWindow.MouseWheelScrolled += new EventHandler<MouseWheelScrollEventArgs>(OnMouseScroll);
        }
    }
}
