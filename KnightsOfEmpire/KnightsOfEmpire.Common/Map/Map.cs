using KnightsOfEmpire.Common.Networking;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;

namespace KnightsOfEmpire.Common.Map
{
    [Serializable]
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
        public int TileCountX { get; set; }
        public int TileCountY { get; set; }
        public TileType[][] TileTypes { get; set; }
        public int[][] TileTexture { get; set; }

        public bool IsTileInBounds(int x, int y)
        {
            if (x < 0 || x >= TileCountX) return false;
            return y >= 0 && y < TileCountY;
        }

        public bool IsTileWalkable(int x, int y)
        {
            if (!IsTileInBounds(x, y)) return false;
            return TileTypes[x][y] == TileType.Walkable;
        }

        public bool CanUnitBeSpawnedOnPos(Vector2f pos)
        {
            if (!IsPositionInBounds(pos)) return false;
            int tileX = (int)(pos.X / TilePixelSize);
            int tileY = (int)(pos.Y / TilePixelSize);
            Console.WriteLine($"{tileX} {tileY}");
            return IsTileWalkable(tileX, tileY);
        }

        public bool IsPositionInBounds(Vector2f pos)
        {
            if (pos.X < 0 || pos.X >= TileCountX * TilePixelSize) return false;
            return pos.Y >= 0 && pos.Y < TileCountY * TilePixelSize;
        }

        public Map() { }

        public Map(string mapFileName)
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @".\Assets\Maps"));
            path = Path.Combine(path, mapFileName);
            string[] lines = File.ReadAllLines(path);

            string[] sizes = lines[0].Split(' ');
            TileCountX = Int32.Parse(sizes[0]);
            TileCountY = Int32.Parse(sizes[1]);

            TileTypes = new TileType[TileCountX][];
            for (int i = 0; i < TileCountX; i++)
            {
                TileTypes[i] = new TileType[TileCountY];
            }
            TileTexture = new int[TileCountX][];
            for (int i = 0; i < TileCountX; i++)
            {
                TileTexture[i] = new int[TileCountY];
            }

            for (int i = 0; i < TileCountY; i++) 
            {
                string[] tileTypeValues = lines[i + 2].Split(' ');
                string[] tileTextureValues = lines[i + TileCountY + 3].Split(' ');
                for(int j = 0; j < TileCountX; j++) 
                {
                    TileTypes[j][i] = (TileType)Int32.Parse(tileTypeValues[j]);
                    TileTexture[j][i] = Int32.Parse(tileTextureValues[j]);
                }
            }
        }

    }
}
