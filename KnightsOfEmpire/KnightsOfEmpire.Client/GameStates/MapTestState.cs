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
    public class MapTestState : GameState
    {
        public View View = new View(new Vector2f(400, 400), new Vector2f(800, 800));

        public Vector2i MousePosition;
        public int EdgeViewMoveOffset = 50;
        public int ViewCenterLeftBoundX = 300;
        public int ViewCenterRightBoundX = 700;
        public int ViewCenterTopBoundY = 300;
        public int ViewCenterBottomBoundY = 700;
        public float ViewScrollSpeed = 800;

        public Map map;

        public FlowField[,] flowFields;
        public List<Texture> textures;
        public RectangleShape[,] mapRectangles;


        public override void Initialize()
        {
            map = new Map("64x64test.kmap");
            textures = new List<Texture>();
            textures.Add(new Texture(@"./Assets/Textures/grass.png"));
            textures.Add(new Texture(@"./Assets/Textures/water.png"));
            mapRectangles = new RectangleShape[map.TileCountX, map.TileCountY];


            Parallel.For(0, map.TileCountX, (x, stateOuter) =>
            {
                Parallel.For(0, map.TileCountY, (y, stateInner) =>
                {
                    CreateMapRectangle(x, y);
                });
            });
            Console.WriteLine($"Map loaded!");

            ViewCenterLeftBoundX = 0;
            ViewCenterRightBoundX = map.TileCountX * Map.TilePixelSize;
            ViewCenterTopBoundY = 0;
            ViewCenterBottomBoundY = map.TileCountY * Map.TilePixelSize;
        // WARNING!!! HIGH CPU USAGE
        //flowFields = new FlowField[map.TileCountX, map.TileCountY];
        //var watch = System.Diagnostics.Stopwatch.StartNew();

        //Parallel.For(0, map.TileCountX, (x,stateOuter) =>
        //{
        //    Parallel.For(0, map.TileCountY, (y, stateInner) =>
        //    {
        //        if (!map.IsTileWalkable(x, y)) return;
        //        FlowField field = new FlowField(map, x, y);
        //        flowFields[x, y] = field;
        //    }
        //    );
        //}
        //);
        //watch.Stop();
        //long elapsedMs = watch.ElapsedMilliseconds;
        //Console.WriteLine($"Flowfield batch execution time: {elapsedMs * 1.00 / 1000.00}s");

        //int updateX = 5;
        //int updateY = 5;
        //map.TileTexture[updateX, updateX] = 1;
        //map.TileTypes[updateX, updateX] = TileType.NonWalkable;
        //CreateMapRectangle(updateX, updateY);

        //Parallel.For(0, map.TileCountX, (x, stateOuter) =>
        //{
        //    Parallel.For(0, map.TileCountY, (y, stateInner) =>
        //    {
        //        if (flowFields[x, y] == null) return;
        //        flowFields[x, y].UpdateBuildingFlowField(map, updateX, updateY);
        //    }
        //    );
        //}
        //);
        //watch.Restart();

        //watch.Stop();
        //elapsedMs = watch.ElapsedMilliseconds;
        //Console.WriteLine($"Flowfield batch update execution time: {elapsedMs * 1.00 / 1000.00}s");
    }

        public void CreateMapRectangle(int x,int y)
        {
            mapRectangles[x, y] = new RectangleShape(new Vector2f(Map.TilePixelSize, Map.TilePixelSize));
            mapRectangles[x, y].Position = new Vector2f(x * Map.TilePixelSize, y * Map.TilePixelSize);
            mapRectangles[x, y].Texture = textures[map.TileTexture[x, y]];
        }

        void DrawMap() 
        {
            for (int i = 0; i < map.TileCountX; i++)
            {
                for (int j = 0; j < map.TileCountY; j++)
                {
                    Client.RenderWindow.Draw(mapRectangles[i, j]);
                }
            }
        }

        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(100, 50, 247));
            View.Size = new Vector2f(Client.RenderWindow.Size.X, Client.RenderWindow.Size.Y);
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
            DrawMap();
        }
    }
}
