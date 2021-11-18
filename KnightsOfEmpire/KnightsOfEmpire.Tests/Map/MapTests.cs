using KnightsOfEmpire.Common.Map;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KnightsOfEmpire.Tests
{
    [TestClass]
    public class MapTests
    {
        public Map GenerateMap(int x, int y)
        {
            Map map = new Map();
            map.TileCountX = 10;
            map.TileCountY = 10;
            map.TileTypes = new TileType[map.TileCountX][];
            for (int i = 0; i < map.TileCountY; i++)
            {
                map.TileTypes[i] = new TileType[y];
            }

            return map;
        }

        [TestMethod]
        public void IsTileWalkableReturnsTrueWhenTileIsWalkable()
        {
            Map map = GenerateMap(10, 10);
            map.TileTypes[5][5] = TileType.Walkable;
            Assert.IsTrue(map.IsTileWalkable(5, 5));
        }

        [TestMethod]
        public void IsTileWalkableReturnsFalseWhenTileIsNotWalkable()
        {
            Map map = GenerateMap(10, 10);
            map.TileTypes[5][5] = TileType.Wall;
            Assert.IsFalse(map.IsTileWalkable(5, 5));
        }

        [TestMethod]
        public void IsTileWalkableReturnsFalseWhenTileIsOutOfBounds()
        {
            Map map = GenerateMap(10, 10);
            Assert.IsFalse(map.IsTileWalkable(-124, 569));
        }

        [TestMethod]
        public void CanUnitBeSpawnedOnPosReturnsFalseWhenPosIsOutOfBounds()
        {
            Map map = GenerateMap(10, 10);
            Assert.IsFalse(map.CanUnitBeSpawnedOnPos(-123213, 524324324));
        }

        [TestMethod]
        public void CanUnitBeSpawnedOnPosReturnsTrueWhenTileIsWalkable()
        {
            Map map = GenerateMap(10, 10);
            map.TileTypes[2][2] = TileType.Walkable;
            Assert.IsTrue(map.CanUnitBeSpawnedOnPos((2*Map.TilePixelSize)+10, (2 * Map.TilePixelSize)+10));
        }

        [TestMethod]
        public void CanUnitBeSpawnedOnPosReturnsFalseWhenTileIsNonWalkable()
        {
            Map map = GenerateMap(10, 10);
            map.TileTypes[5][5] = TileType.NonWalkable;
            Assert.IsFalse(map.CanUnitBeSpawnedOnPos((5 * Map.TilePixelSize) + 5, (5 * Map.TilePixelSize) + 5));
        }
    }
}
