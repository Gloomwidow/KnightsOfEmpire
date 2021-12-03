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
    public class FlowFieldManager
    {
        private GameMap gameMap;
        private FlowField[,] flowFields;

        public FlowFieldManager(GameMap map)
        {
            gameMap = map;
            flowFields = new FlowField[gameMap.TileCountX, gameMap.TileCountY];
        }

        public Vector2f GetFlowVector(Vector2f currentPosition, Vector2i destination)
        {
            Vector2i pos = new Vector2i((int)(currentPosition.X / GameMap.TilePixelSize), (int)(currentPosition.Y / GameMap.TilePixelSize));
            return GetFlowVector(pos, destination);
        }

        public Vector2f GetFlowVector(Vector2i currentPosition, Vector2i destination)
        {
            if (flowFields[destination.X,destination.Y]==null)
            {
                if (!gameMap.IsTileWalkable(destination.X, destination.Y)) return new Vector2f(0,0);
                var watch = System.Diagnostics.Stopwatch.StartNew();
                FlowField field = new FlowField(gameMap, destination.X, destination.Y);
                flowFields[destination.X, destination.Y] = field;
                watch.Stop();
                long elapsedMs = watch.ElapsedMilliseconds;
                Console.WriteLine($"Single Flowfield execution time: {elapsedMs * 1.00 / 1000.00}s");
            }
            return flowFields[destination.X, destination.Y].GetFlowVector(currentPosition.X, currentPosition.Y);
        }

        public void AddBuildingOnMap(int buildX, int buildY)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            gameMap.TileTypes[buildX][buildY] = TileType.NonWalkable;
            Parallel.For(0, gameMap.TileCountX, (x, stateOuter) =>
            {
                Parallel.For(0, gameMap.TileCountY, (y, stateInner) =>
                {
                    if (flowFields[x, y] == null) return;
                    flowFields[x, y].UpdateBuildingFlowField(gameMap, buildX, buildY);
                }
                );
            }
            );
            watch.Stop();
            long elapsedMs = watch.ElapsedMilliseconds;
            Console.WriteLine($"Update Flowfields execution time: {elapsedMs * 1.00 / 1000.00}s");
        }
    }
}
