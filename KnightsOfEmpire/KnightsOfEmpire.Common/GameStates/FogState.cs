using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Units;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace KnightsOfEmpire.Common.GameStates
{ 
    public class FogState : GameState
    {
        protected float[][,] visibilityLevels;
        protected Map.Map map;

        public int BuildingVisibilityDistance = 5; // in tiles
        public static float VisibilityMinLevel = 0.5f;

        public float[,] VisibilityLevel
        {
            get
            {
                return visibilityLevels[0];
            }
        }

        public override void Update()
        {
            base.Update();
            for (int i = 0; i < visibilityLevels.Length; i++)
            {
                for (int x = 0; x < map.TileCountX; x++)
                {
                    for (int y = 0; y < map.TileCountY; y++)
                    {
                        visibilityLevels[i][x, y] = VisibilityMinLevel;
                    }
                }
            }
        }

        protected void CalculateVision(List<(Vector2i pos, int dist)> visionStartPosition, int playerID = 0)
        {
            foreach ((Vector2i pos, int dist) checkPos in visionStartPosition)
            {
                VisibilityLevel[checkPos.pos.X, checkPos.pos.Y] = 1.0f;
                PropagateDiagonal(checkPos.pos.X, checkPos.pos.Y, 1, 1, 1.0f, 0, checkPos.dist, false, false, playerID);
                PropagateDiagonal(checkPos.pos.X, checkPos.pos.Y, -1, 1, 1.0f, 0, checkPos.dist, false, false, playerID);
                PropagateDiagonal(checkPos.pos.X, checkPos.pos.Y, -1, -1, 1.0f, 0, checkPos.dist, false, false, playerID);
                PropagateDiagonal(checkPos.pos.X, checkPos.pos.Y, 1, -1, 1.0f, 0, checkPos.dist, false, false, playerID);
            }
        }

        protected void PropagateForward(int posX, int posY, int dirX, int dirY, float currentVisibility, int distance, int maxDistance, int fieldID = 0)
        {
            if (!map.IsTileInBounds(posX, posY)) return;
            visibilityLevels[fieldID][posX, posY] = Math.Max(visibilityLevels[fieldID][posX, posY], currentVisibility);
            if (map.IsTileVisionObstructed(posX, posY) && distance!=0) return;
            int VisibilityFallofStart = maxDistance / 2;
            if (distance >= maxDistance) return;
            if (distance > VisibilityFallofStart)
            {
                float visibilityCoef = (1.0f - VisibilityMinLevel) / (maxDistance - VisibilityFallofStart) * 1.00f;
                currentVisibility -= visibilityCoef;
            }
            PropagateForward(posX + dirX, posY + dirY, dirX, dirY, currentVisibility, distance + 1, maxDistance, fieldID);
        }

        protected void PropagateDiagonal(int posX, int posY, int dirX, int dirY, float currentVisibility, int distance, int maxDistance, bool blockX, bool blockY, int fieldID = 0)
        {
            if (!map.IsTileInBounds(posX, posY)) return;
            visibilityLevels[fieldID][posX, posY] = Math.Max(visibilityLevels[fieldID][posX, posY], currentVisibility);
            if (map.IsTileVisionObstructed(posX, posY) && distance != 0) return;
            int VisibilityFallofStart = maxDistance / 2;
            if (distance >= maxDistance) return;
            if (distance > VisibilityFallofStart)
            {
                float visibilityCoef = (1.0f - VisibilityMinLevel) / (maxDistance - VisibilityFallofStart) * 1.00f;
                currentVisibility -= visibilityCoef;
            }

            if (!blockX) PropagateForward(posX + dirX, posY, dirX, 0, currentVisibility, distance + 1, maxDistance, fieldID);
            if (!blockY) PropagateForward(posX, posY + dirY, 0, dirY, currentVisibility, distance + 1, maxDistance, fieldID);

            if (map.IsTileVisionObstructed(posX + dirX, posY)) blockX = true;
            if (map.IsTileVisionObstructed(posX, posY + dirY)) blockY = true;

            PropagateDiagonal(posX + dirX, posY + dirY, dirX, dirY, currentVisibility, distance + 1, maxDistance, blockX, blockY, fieldID);
        }
    }
}
