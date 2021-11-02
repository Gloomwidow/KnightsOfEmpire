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
    public class UnitsSelectionState : GameState
    {
        public RectangleShape selectionRectangle;

        public override void Initialize() 
        {

        }

        public override void Update()
        {
            Vector2i clickPos = Mouse.GetPosition(Client.RenderWindow);
            Vector2f worldPos = Client.RenderWindow.MapPixelToCoords(clickPos);
            if (Mouse.IsButtonPressed(Mouse.Button.Left) && selectionRectangle == null)
            {
                selectionRectangle = new RectangleShape();
                selectionRectangle.Position = worldPos;
                selectionRectangle.FillColor = new Color(0, 0, 255, 50);
            }
            else if (Mouse.IsButtonPressed(Mouse.Button.Left) && selectionRectangle != null)
            {
                selectionRectangle.Size = (worldPos - selectionRectangle.Position);
            }
            else if (!Mouse.IsButtonPressed(Mouse.Button.Left) && selectionRectangle != null)
            {
                ChangleSelectionRectangleCoords(worldPos);
                // TO-DO select units in rectangle 
                selectionRectangle = null;
            }
        }
        void ChangleSelectionRectangleCoords(Vector2f endPosition)
        {
            selectionRectangle.Size = new Vector2f(Math.Abs(endPosition.X - selectionRectangle.Position.X), Math.Abs(endPosition.Y - selectionRectangle.Position.Y));
            selectionRectangle.Position = new Vector2f(Math.Min(endPosition.X, selectionRectangle.Position.X), Math.Min(endPosition.Y, selectionRectangle.Position.Y));
        }

        public override void Render() 
        {
            if (selectionRectangle != null)
            {
                Client.RenderWindow.Draw(selectionRectangle);
            }
        }
    }
}
