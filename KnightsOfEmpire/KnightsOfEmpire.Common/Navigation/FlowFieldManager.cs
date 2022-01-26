using KnightsOfEmpire.Common.Helper;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Units;
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
        public const int MaxAllowedUnitsPerTile = 3;
        private GameMap gameMap;
        private FlowField[,] flowFields;
        private int[,,] unitDensityMap;

        public FlowFieldManager(GameMap map)
        {
            gameMap = map;
            flowFields = new FlowField[gameMap.TileCountX, gameMap.TileCountY];
            unitDensityMap = new int[Constants.MaxPlayers, gameMap.TileCountX, gameMap.TileCountY];
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
                Logger.Log($"Single Flowfield execution time: {elapsedMs * 1.00 / 1000.00}s");
            }
            return flowFields[destination.X, destination.Y].GetFlowVector(currentPosition.X, currentPosition.Y);
        }

        public bool IsNextPositionOccupied(int PlayerId, Vector2f currentPosition, Vector2i destination)
        {
            Vector2i pos = Map.Map.ToTilePos(currentPosition);
            Vector2i dirTile = flowFields[destination.X, destination.Y].GetFlowTileDir(pos.X, pos.Y);
            return unitDensityMap[PlayerId, pos.X + dirTile.X, pos.Y + dirTile.Y] >= MaxAllowedUnitsPerTile;
        }

        public void AddBuildingOnMap(int buildX, int buildY)
        {
            gameMap.TileTypes[buildX][buildY] = TileType.Building;
            UpdateBuildingFlowField(buildX, buildY);
        }

        public void RemoveBuildingFromMap(int buildX, int buildY)
        {
            gameMap.TileTypes[buildX][buildY] = TileType.Walkable;
            UpdateBuildingFlowField(buildX, buildY);
        }

        public void MoveUnitOnDensityMap(Unit u)
        {
            Vector2i previousPosition = Map.Map.ToTilePos(u.PreviousPosition);
            Vector2i nextPosition = Map.Map.ToTilePos(u.Position);
            unitDensityMap[u.PlayerId, previousPosition.X, previousPosition.Y]--;
            unitDensityMap[u.PlayerId, nextPosition.X, nextPosition.Y]++;
        }

        public void AddUnitOnDensityMap(Unit u)
        {
            Vector2i position = Map.Map.ToTilePos(u.Position);
            unitDensityMap[u.PlayerId, position.X, position.Y]++;
        }

        public void RemoveUnitOnDensityMap(Unit u)
        {
            Vector2i position = Map.Map.ToTilePos(u.Position);
            unitDensityMap[u.PlayerId, position.X, position.Y]--;
        }

        protected void UpdateBuildingFlowField(int buildX, int buildY)
        {
            Parallel.For(0, gameMap.TileCountX, (x, stateOuter) =>
            {
                Parallel.For(0, gameMap.TileCountY, (y, stateInner) =>
                {
                    if (flowFields[x, y] == null) return;
                    flowFields[x, y].UpdateBuildingFlowField(gameMap, buildX, buildY);
                }
                );
            });
        }
    }
}
