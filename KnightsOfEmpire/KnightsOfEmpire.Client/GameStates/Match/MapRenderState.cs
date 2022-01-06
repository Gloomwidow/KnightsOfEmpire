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
using KnightsOfEmpire.GameStates.Match;
using KnightsOfEmpire.Common.Helper;

namespace KnightsOfEmpire.GameStates
{
    public class MapRenderState : GameState
    {
        public View View = new View(new Vector2f(400, 400), new Vector2f(800, 800));

        public Map GameMap;
        public float[,] VisibilityLevel;
        public Texture TileAtlas;
        public int TileAtlasSize;
        public RectangleShape[,] mapRectangles;

        public View RenderView;


        public (int xA, int xB, int yA, int yB) GetMapBounds()
        {
            return (0, GameMap.TileCountX * Map.TilePixelSize, 0 , GameMap.TileCountY * Map.TilePixelSize);
        }

        public override void LoadResources()
        {

            TileAtlas = new Texture(@"./Assets/Textures/tiles.png");
            TileAtlasSize = (int)TileAtlas.Size.X;
        }

        public override void LoadDependencies()
        {
            VisibilityLevel = Parent.GetSiblingGameState<FogOfWarState>().VisibilityLevel;
            RenderView = Parent.GetSiblingGameState<ViewControlState>().View;
        }

        public override void Initialize()
        {
            GameMap = Client.Resources.Map;
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
            mapRectangles[x, y].Texture = TileAtlas;
            mapRectangles[x, y].TextureRect = IdToTextureRect.
                GetRect(GameMap.TileTexture[x][y], new Vector2i(TileAtlasSize, TileAtlasSize), 32);
        }


        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(0,0,0));
            Client.RenderWindow.SetView(RenderView);
            for (int i = 0; i < GameMap.TileCountX; i++)
            {
                for (int j = 0; j < GameMap.TileCountY; j++)
                {
                    float visionCoef = VisibilityLevel[i, j];
                    mapRectangles[i, j].FillColor = new Color((byte)(255 * visionCoef), (byte)(255 * visionCoef), (byte)(255 * visionCoef));
                    Client.RenderWindow.Draw(mapRectangles[i, j]);
                }
            }
        }

        public override void Dispose()
        {
            TileAtlas.Dispose();
        }
    }
}
