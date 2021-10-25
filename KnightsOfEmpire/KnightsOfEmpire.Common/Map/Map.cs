using KnightsOfEmpire.Common.Networking;
using System;
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

        //TO-DO: constructor for loading map from file
        public Map(string mapFileName)
        {

        }

        //TO-DO: constructor for loading map from packet sent by server
        public Map(ReceivedPacket mapPacket)
        {

        }

    }
}
