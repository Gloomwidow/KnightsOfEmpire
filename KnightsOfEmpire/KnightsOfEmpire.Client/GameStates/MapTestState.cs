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
        public RectangleShape RectangleShape = new RectangleShape(new Vector2f (800, 800));
        public View View = new View(new Vector2f(400, 400), new Vector2f(800, 800));
        
        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(100, 50, 247));
            Client.RenderWindow.SetView(View);
            //View.Center = ((Vector2f)Mouse.GetPosition(Client.RenderWindow));
            if(Mouse.GetPosition(Client.RenderWindow).X >= 0 && Mouse.GetPosition(Client.RenderWindow).X <= 50) 
            {
                if(View.Center.X >= 300) 
                {
                    View.Move(new Vector2f(-0.05f, 0));
                }      
            }
            else if (Mouse.GetPosition(Client.RenderWindow).X >= Client.RenderWindow.Size.X - 50 && Mouse.GetPosition(Client.RenderWindow).X <= Client.RenderWindow.Size.X)
            {
                if (View.Center.X <= 700)
                {
                    View.Move(new Vector2f(0.05f, 0));
                }
            }
            if (Mouse.GetPosition(Client.RenderWindow).Y >= 0 && Mouse.GetPosition(Client.RenderWindow).Y <= 50)
            {
                if (View.Center.Y >= 300)
                {
                    View.Move(new Vector2f(0, -0.05f));
                }
            }
            else if (Mouse.GetPosition(Client.RenderWindow).Y >= Client.RenderWindow.Size.Y - 50 && Mouse.GetPosition(Client.RenderWindow).Y <= Client.RenderWindow.Size.Y)
            {
                if (View.Center.Y <= 700)
                {
                    View.Move(new Vector2f(0, 0.05f));
                }
            }
            //Console.WriteLine(Mouse.GetPosition(Client.RenderWindow).X + Mouse.GetPosition(Client.RenderWindow).Y);
            RectangleShape.Position = new Vector2f(100, 100);
            RectangleShape.FillColor = new Color(Color.Green);
            Client.RenderWindow.Draw(RectangleShape);
        }
    }
}
