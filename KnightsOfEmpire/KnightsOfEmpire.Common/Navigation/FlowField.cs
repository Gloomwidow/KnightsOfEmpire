using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Map;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameMap = KnightsOfEmpire.Common.Map.Map;

namespace KnightsOfEmpire.Common.Navigation
{
    public class FlowField
    {
        protected byte[,] FlowVectors;
        protected int[,] IntegrationField;
        public int EndPosX { get; protected set; }
        public int EndPosY { get; protected set; }

        public Vector2i EndPos
        {
            get
            {
                return new Vector2i(EndPosX, EndPosY);
            }
        }

        /*
         * 012
         * 3X4
         * 567
         */
        protected static Vector2f[] vectorIds = new Vector2f[]
        {
            new Vector2f(-1,-1).Normalized(),
            new Vector2f(0,-1).Normalized(),
            new Vector2f(1,-1).Normalized(),
            new Vector2f(-1,0).Normalized(),
            new Vector2f(1,0).Normalized(),
            new Vector2f(-1,1).Normalized(),
            new Vector2f(0,1).Normalized(),
            new Vector2f(1,1).Normalized()
        };

        public FlowField(GameMap map, int endPosX, int endPosY)
        {
            EndPosX = endPosX;
            EndPosY = endPosY;

            IntegrationField = new int[map.TileCountY, map.TileCountX];
            FlowVectors = new byte[map.TileCountY, map.TileCountX];
            CalculateIntegrationField(map);
            CalculateFlowVectors(map, 0, map.TileCountX, 0, map.TileCountY);
        }

        public Vector2f GetFlowVector(int x, int y)
        {
            return vectorIds[FlowVectors[x, y]];
        }

        public void UpdateBuildingFlowField(GameMap map, int x, int y)
        {
            CalculateFlowVectors(map, x - 1, x + 1, y - 1, y + 1);
        }

        protected void CalculateIntegrationField(GameMap map)
        {
            for (int x = 0; x < map.TileCountX; x++)
            {
                for (int y = 0; y < map.TileCountY; y++)
                {
                    IntegrationField[y, x] = map.TileCountY * map.TileCountX * map.TileCountY * map.TileCountX;
                }
            }

            IntegrationField[EndPosX, EndPosY] = 0;

            bool[,] IsInOpenList = new bool[map.TileCountX, map.TileCountY];
            IsInOpenList[EndPosX, EndPosY] = true;

            Queue<Vector2i> OpenList = new Queue<Vector2i>();

            OpenList.Enqueue(EndPos);
            while (OpenList.Count > 0)
            {
                Vector2i current = OpenList.Dequeue();
                IsInOpenList[current.X, current.Y] = false;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0) continue;

                        Vector2i neighbor = new Vector2i(current.X + i, current.Y + j);
                        if (!map.IsTileInBounds(neighbor.X, neighbor.Y)) continue;
                        if (!map.IsTileWalkable(neighbor.X, neighbor.Y)) continue;


                        int cost = 10;
                        if (Math.Abs(i) + Math.Abs(j) == 2) cost = 14; // diagonal

                        int currentNeighborCost = IntegrationField[current.X, current.Y] + cost;

                        if (currentNeighborCost < IntegrationField[neighbor.X, neighbor.Y])
                        {
                            if (!IsInOpenList[neighbor.X, neighbor.Y])
                            {
                                IsInOpenList[neighbor.X, neighbor.Y] = true;
                                OpenList.Enqueue(neighbor);
                            }

                            IntegrationField[neighbor.X, neighbor.Y] = currentNeighborCost;
                        }
                    }
                }
            }
        }

        protected void CalculateFlowVectors(GameMap map, int xL, int xR, int yL, int yR)
        {
            for (int x = xL; x < xR; x++)
            {
                for (int y = yL; y < yR; y++)
                {
                    Vector2i current = new Vector2i(x, y);
                    if (!map.IsTileInBounds(x,y)) continue;
                    if (!map.IsTileWalkable(x,y)) continue;
                    int lowestIntegration = map.TileCountY * map.TileCountX * map.TileCountY * map.TileCountX;
                    Vector2i lowestVector = new Vector2i(0, 0);
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (i == 0 && j == 0) continue;
                            Vector2i neighbor = new Vector2i(current.X + i, current.Y + j);
                            if (!map.IsTileInBounds(neighbor.X,neighbor.Y)) continue;
                            if (!map.IsTileWalkable(neighbor.X, neighbor.Y)) continue;

                            if (Math.Abs(i) + Math.Abs(j) == 2)
                            {
                                if (!CanMoveToDiagonalField(map, current, i, j)) continue;
                            }

                            if (lowestIntegration > IntegrationField[neighbor.X, neighbor.Y])
                            {
                                lowestIntegration = IntegrationField[neighbor.X, neighbor.Y];
                                lowestVector = new Vector2i(i, j);
                            }
                        }
                    }
                    FlowVectors[current.X, current.Y] = VectorToId(lowestVector);
                }
            }
        }

        protected byte VectorToId(Vector2i v)
        {
            if (v.X == -1)
            {
                if (v.Y == -1) return 0;
                else if (v.Y == 0) return 3;
                else return 5;
            }
            else if (v.X == 0)
            {
                if (v.Y == -1) return 1;
                else return 6;
            }
            else
            {
                if (v.Y == -1) return 2;
                else if (v.Y == 0) return 4;
                else return 7;
            }
        }

        protected bool CanMoveToDiagonalField(GameMap map, Vector2i pos, int i, int j)
        {
            Vector2i v1, v2;

            if (i == -1) v1 = new Vector2i(pos.X - 1, pos.Y);
            else v1 = new Vector2i(pos.X + 1, pos.Y);

            if (j == -1) v2 = new Vector2i(pos.X, pos.Y - 1);
            else v2 = new Vector2i(pos.X, pos.Y + 1);

            if (map.IsTileInBounds(v1.X, v1.Y))
            {
                if (!map.IsTileWalkable(v1.X, v1.Y)) return false;
            }

            if (map.IsTileInBounds(v2.X, v2.Y))
            {
                if (!map.IsTileWalkable(v2.X, v2.Y)) return false;
            }

            return true;
        }
    }
}
