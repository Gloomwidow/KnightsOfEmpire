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

namespace KnightsOfEmpire.GameStates
{
    public class MapTestState : GameState
    {
        public RectangleShape RectangleShape = new RectangleShape(new Vector2f (800, 800));
        public View View = new View(new Vector2f(400, 400), new Vector2f(800, 800));

        public Vector2i MousePosition;
        public int EdgeViewMoveOffset = 50;
        public int ViewCenterLeftBoundX = 300;
        public int ViewCenterRightBoundX = 700;
        public int ViewCenterTopBoundY = 300;
        public int ViewCenterBottomBoundY = 700;
        public float ViewScrollSpeed = 200;

        public Map map;
        public Texture grass;
        public Texture water;
        public RectangleShape[,] mapRectangles;


        public override void Initialize()
        {
            map = new Map("TestMap.kmap");
            grass = new Texture(@"./Assets/Textures/grass.png");
            water = new Texture(@"./Assets/Textures/water.png");
            mapRectangles = new RectangleShape[map.TileCountY, map.TileCountX];
            for(int i = 0; i < map.TileCountY; i++) 
            {
                for(int j = 0; j < map.TileCountX; j++) 
                {
                    mapRectangles[i,j] = new RectangleShape(new Vector2f(Map.TilePixelSize, Map.TilePixelSize));
                    mapRectangles[i, j].Position = new Vector2f(i * Map.TilePixelSize, j * Map.TilePixelSize);
                    switch (map.TileTexture[i,j]) 
                    {
                        case 1:
                            mapRectangles[i, j].Texture = grass;
                            break;
                        case 2:
                            mapRectangles[i, j].Texture = water;
                            break;
                        default:
                            
                            break;
                    }
                    
                }
            }
        }

        void DrawMap() 
        {
            for (int i = 0; i < map.TileCountY; i++)
            {
                for (int j = 0; j < map.TileCountX; j++)
                {
                    Client.RenderWindow.Draw(mapRectangles[i, j]);
                }
            }
        }

        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(100, 50, 247));
            Client.RenderWindow.SetView(View);
            MousePosition = Mouse.GetPosition(Client.RenderWindow);
            float ViewScrollSpeedPerFrame = ViewScrollSpeed * Client.DeltaTime; 

            if (MousePosition.X >= 0 && MousePosition.X <= EdgeViewMoveOffset) 
            {
                if(View.Center.X >= ViewCenterLeftBoundX)
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
            RectangleShape.Position = new Vector2f(100, 100);
            RectangleShape.FillColor = new Color(Color.Green);
            //Client.RenderWindow.Draw(RectangleShape);
            DrawMap();
        }
    }
}
