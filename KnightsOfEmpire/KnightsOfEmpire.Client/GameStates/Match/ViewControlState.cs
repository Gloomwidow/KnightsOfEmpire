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
    public class ViewControlState : GameState
    {
        public View View { get; protected set; }

        private float gameZoom = 1;
        private float minimumZoom = 0.5f;
        private float maximumZoom = 5.0f;
        private float gameZoopSpeed = 25.0f;

        public Vector2i MousePosition;
        public int EdgeViewMoveOffset = 50;
        public int ViewCenterLeftBoundX = 300;
        public int ViewCenterRightBoundX = 700;
        public int ViewCenterTopBoundY = 300;
        public int ViewCenterBottomBoundY = 700;
        public float ViewScrollSpeed = 800;

        protected EventHandler<MouseWheelScrollEventArgs> mouseScrollZoomHandler;

        void OnMouseScroll(object sender, EventArgs e)
        {
            MouseWheelScrollEventArgs mouseEvent = (MouseWheelScrollEventArgs)e;
            if ((mouseEvent.Delta < 0 && gameZoom >= maximumZoom) || (mouseEvent.Delta > 0 && gameZoom <= minimumZoom)) return;
            gameZoom -= gameZoopSpeed * mouseEvent.Delta * Client.DeltaTime;
            gameZoom = Math.Max(minimumZoom, Math.Min(maximumZoom, gameZoom));
        }

        public void SetCameraBounds((int xA, int xB, int yA, int yB) bounds)
        {
            ViewCenterLeftBoundX = bounds.xA;
            ViewCenterRightBoundX = bounds.xB;
            ViewCenterTopBoundY = bounds.yA;
            ViewCenterBottomBoundY = bounds.yB;
        }

        public override void Initialize()
        {
            View = new View(new Vector2f(400, 400), new Vector2f(800, 800));
            mouseScrollZoomHandler = new EventHandler<MouseWheelScrollEventArgs>(OnMouseScroll);
            Client.RenderWindow.MouseWheelScrolled += mouseScrollZoomHandler;
        }

        public override void Update()
        {
            View.Size = new Vector2f(Client.RenderWindow.Size.X, Client.RenderWindow.Size.Y);
            View.Zoom(gameZoom);
            MousePosition = Mouse.GetPosition(Client.RenderWindow);
            float ViewScrollSpeedPerFrame = ViewScrollSpeed * Client.DeltaTime;

            if (MousePosition.X >= 0 && MousePosition.X <= EdgeViewMoveOffset)
            {
                if (View.Center.X >= ViewCenterLeftBoundX)
                {
                    View.Move(new Vector2f(-ViewScrollSpeedPerFrame, 0));
                }
            }
            else if (MousePosition.X >= (Client.RenderWindow.Size.X - EdgeViewMoveOffset) && MousePosition.X <= Client.RenderWindow.Size.X)
            {
                if (View.Center.X <= ViewCenterRightBoundX)
                {
                    View.Move(new Vector2f(ViewScrollSpeedPerFrame, 0));
                }
            }
            if (MousePosition.Y >= 0 && MousePosition.Y <= EdgeViewMoveOffset)
            {
                if (View.Center.Y >= ViewCenterTopBoundY)
                {
                    View.Move(new Vector2f(0, -ViewScrollSpeedPerFrame));
                }
            }
            else if (MousePosition.Y >= (Client.RenderWindow.Size.Y - EdgeViewMoveOffset) && MousePosition.Y <= Client.RenderWindow.Size.Y)
            {
                if (View.Center.Y <= ViewCenterBottomBoundY)
                {
                    View.Move(new Vector2f(0, ViewScrollSpeedPerFrame));
                }
            }
        }

        public override void Dispose()
        {
            Client.RenderWindow.MouseWheelScrolled -= mouseScrollZoomHandler;
        }
    }
}
