using KnightsOfEmpire.Common.Networking;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Map
{
    public class Map
    {
        /* 
         * Map Structure
         * X,Y - size of map
         * Grid map of tile types
         * 0 - walkable
         * 1 - non-walkable
         * 2 - wall
         * X - start point spawn
         * 
         * e.x.
         * 01010
         * 00000
         * 00002
         * 22222
         * 
         * then texture id for each tile
         * 12,5,0,2
         * 0,0,0,0
         * 5,5,5,5
         * 7,7,7,7
         */

        public static int TilePixelSize = 64;
        public int TileCountX { get; protected set; }
        public int TileCountY { get; protected set; }
        public TileType[,] TileTypes { get; protected set; }
        public int[,] TileTexture { get; protected set; }

        public bool IsTileInBounds(int x, int y)
        {
            if (x < 0 || x >= TileCountX) return false;
            return y >= 0 && y < TileCountY;
        }

        public bool IsTileWalkalble(int x, int y)
        {
            if (!IsTileInBounds(x, y)) return false;
            return TileTypes[x, y] == TileType.Walkable;
        }

        public Map(string mapFileName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @".\Assets\Maps"));
            path = Path.Combine(path, mapFileName);
            string[] lines = File.ReadAllLines(path);

            string[] sizes = lines[0].Split(' ');
            TileCountY = Int32.Parse(sizes[0]);
            TileCountX = Int32.Parse(sizes[1]);

            TileTypes = new TileType[TileCountY,TileCountX];
            TileTexture = new int[TileCountY,TileCountX];

            for (int i = 0; i < TileCountY; i++) 
            {
                string[] tileTypeValues = lines[i + 2].Split(' ');
                string[] tileTextureValues = lines[i + TileCountY + 3].Split(' ');
                for(int j = 0; j < TileCountX; j++) 
                {
                    TileTypes[i, j] = (TileType)Int32.Parse(tileTypeValues[j]);
                    TileTexture[i, j] = Int32.Parse(tileTextureValues[j]);
                }
            }
        }

        //TO-DO: constructor for loading map from packet sent by server
        public Map(ReceivedPacket mapPacket)
        {

        }

    }
}
