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
    public class MapRenderState : GameState
    {
        public View View = new View(new Vector2f(400, 400), new Vector2f(800, 800));

        public Map GameMap;

        public List<Texture> textures;
        public RectangleShape[,] mapRectangles;

        public View RenderView;


        public (int xA, int xB, int yA, int yB) GetMapBounds()
        {
            return (0, GameMap.TileCountX * Map.TilePixelSize, 0 , GameMap.TileCountY * Map.TilePixelSize);
        }

        public override void LoadResources()
        {
            textures = new List<Texture>();
            textures.Add(new Texture(@"./Assets/Textures/grass.png"));
            textures.Add(new Texture(@"./Assets/Textures/water.png"));
        }
        

        public override void Initialize()
        {
            mapRectangles = new RectangleShape[GameMap.TileCountX, GameMap.TileCountY];

            Parallel.For(0, GameMap.TileCountX, (x, stateOuter) =>
            {
                Parallel.For(0, GameMap.TileCountY, (y, stateInner) =>
                {
                    CreateMapRectangle(x, y);
                });
            });

        }

        public void CreateMapRectangle(int x,int y)
        {
            mapRectangles[x, y] = new RectangleShape(new Vector2f(Map.TilePixelSize, Map.TilePixelSize));
            mapRectangles[x, y].Position = new Vector2f(x * Map.TilePixelSize, y * Map.TilePixelSize);
            mapRectangles[x, y].Texture = textures[GameMap.TileTexture[x][y]];
        }


        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(0,0,0));
            Client.RenderWindow.SetView(RenderView);
            for (int i = 0; i < GameMap.TileCountX; i++)
            {
                for (int j = 0; j < GameMap.TileCountY; j++)
                {
                    Client.RenderWindow.Draw(mapRectangles[i, j]);
                }
            }
        }

        public override void Dispose()
        {
            foreach(Texture texture in textures)
            {
                texture.Dispose();
            }
        }
    }
}
