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

        public const float WallPushBackBias = 0.01f;
        public int TileCountX { get; set; }
        public int TileCountY { get; set; }
        public TileType[][] TileTypes { get; set; }
        public int[][] TileTexture { get; set; }
        public (int x, int y)[] starterPositions { get; set; }

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

        public bool IsTileVisionObstructed(int x, int y)
        {
            if (!IsTileInBounds(x, y)) return true;
            return TileTypes[x][y] == TileType.Wall;
        }

        public bool CanUnitBeSpawnedOnPos(Vector2f pos)
        {
            if (!IsPositionInBounds(pos)) return false;
            int tileX = (int)(pos.X / TilePixelSize);
            int tileY = (int)(pos.Y / TilePixelSize);
            return IsTileWalkable(tileX, tileY);
        }

        public bool HasWallInNeighborhood(Vector2i pos)
        {
            if (!IsTileInBounds(pos.X, pos.Y)) return true;
            for(int i=-1;i<=1;i++)
            {
                for(int j=-1;j<=1;j++)
                {
                    if (!IsTileInBounds(pos.X + i, pos.Y + j)) continue;
                    if (!IsTileWalkable(pos.X + i, pos.Y + j)) return true;
                }
            }
            return false;
        }

        public static Vector2i ToTilePos(Vector2f pos)
        {
            int tileX = (int)(pos.X / TilePixelSize);
            int tileY = (int)(pos.Y / TilePixelSize);
            return new Vector2i(tileX, tileY);
        }

        public bool IsPositionInBounds(Vector2f pos)
        {
            if (pos.X < 0 || pos.X >= TileCountX * TilePixelSize) return false;
            return pos.Y >= 0 && pos.Y < TileCountY * TilePixelSize;
        }
        public bool IsPositionInBounds(float x, float y)
        {
            return IsPositionInBounds(new Vector2f(x, y));
        }

        public bool CanUnitBeSpawnedOnPos(float x, float y)
        {
            return CanUnitBeSpawnedOnPos(new Vector2f(x, y));
        }

        /// <summary>
        /// Snaps unit position to tile's wall. If unit would move on non-walkable tile, it will be stopped at tile's edge.
        /// </summary>
        /// <param name="oldPos"> Previous unit position </param>
        /// <param name="newPos"> Unit's position after movement</param>
        /// <returns> Unit's position after movement with wall compensation</returns>
        ///    
        public Vector2f SnapToWall(Vector2f oldPos, Vector2f newPos)
        {
            Vector2i obstaclePos = ToTilePos(newPos);
            if (IsTileWalkable(obstaclePos.X, obstaclePos.Y)) return newPos;
            Vector2f result = new Vector2f(newPos.X, newPos.Y);
            float leftSide = obstaclePos.X * TilePixelSize;
            float rightSide = (obstaclePos.X + 1) * TilePixelSize;
            float upSide = obstaclePos.Y * TilePixelSize;
            float downSide = (obstaclePos.Y + 1) * TilePixelSize;
            if(oldPos.X<leftSide)
            {
                result.X = leftSide - WallPushBackBias;
                if (oldPos.Y < upSide) result.Y = upSide - WallPushBackBias;
                else if (oldPos.Y > downSide) result.Y = downSide + WallPushBackBias;
            }
            else if(oldPos.X>rightSide)
            {
                result.X = rightSide + WallPushBackBias;
                if (oldPos.Y < upSide) result.Y = upSide - WallPushBackBias;
                else if (oldPos.Y > downSide) result.Y = downSide + WallPushBackBias;
            }
            else
            {
                if (oldPos.Y < upSide) result.Y = upSide - WallPushBackBias;
                else if (oldPos.Y > downSide) result.Y = downSide + WallPushBackBias;
            }
            return result;
        }

        public Vector2f SnapToBounds(Vector2f newPos)
        {
            newPos.X = Math.Max(Math.Min(newPos.X, TileCountX * TilePixelSize - 1), 0);
            newPos.Y = Math.Max(Math.Min(newPos.Y, TileCountY * TilePixelSize - 1), 0);
            return newPos;
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
            int startersCount = Int32.Parse(sizes[2]);
            starterPositions = new (int x,int y)[startersCount];

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

            for(int i=0;i<startersCount;i++)
            {
                string[] starterPos = lines[lines.Length-1-i].Split(' ');
                starterPositions[i] = (int.Parse(starterPos[0]), int.Parse(starterPos[1]));
            }
        }

    }
}
