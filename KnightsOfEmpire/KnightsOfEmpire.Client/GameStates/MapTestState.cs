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

namespace KnightsOfEmpire.GameStates
{
    public class MapTestState : GameState
    {
        public RectangleShape RectangleShape = new RectangleShape(new Vector2f (1180, 620));
        public View View = new View(new Vector2f(50, 50), new Vector2f(100, 100));
        
        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(100, 50, 247));
            Client.RenderWindow.SetView(View);
            RectangleShape.Position = new Vector2f(50, 50);
            RectangleShape.FillColor = new Color(Color.Green);
            Client.RenderWindow.Draw(RectangleShape);
        }
    }
}
