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

namespace KnightsOfEmpire.GameStates.Match
{
    public class FogOfWarState : GameState
    {
        public float[,] VisibilityLevel;

        public List<Unit> PlayerUnits;
        public List<Building> PlayerBuildings;

        private Map map;
        // TO-DO - make that units will have unique VisibilityDistance
        public int VisibilityDistance = 5; // in tiles
        public int VisibilityFallofStart = 2; // in tiles
        public static float VisibilityMinLevel = 0.5f;

        public override void Initialize()
        {
            map = Client.Resources.Map;
            VisibilityLevel = new float[map.TileCountX, map.TileCountY];
        }

        public override void LoadDependencies()
        {
            PlayerUnits = Parent.GetSiblingGameState<UnitUpdateState>().GameUnits[Client.Resources.PlayerGameId];
            PlayerBuildings = Parent.GetSiblingGameState<BuildingUpdateState>().GameBuildings[Client.Resources.PlayerGameId];
        }

        public override void Update()
        {
            for(int x=0;x<map.TileCountX;x++)
            {
                for(int y=0;y<map.TileCountY;y++)
                {
                    VisibilityLevel[x, y] = VisibilityMinLevel;
                }
            }
            //Selects unique positions for visibility calculation
            //TO-DO: when units will have unique Visibility Distance, select unit with longest view distance
            List<Vector2i> visionStartPosition = PlayerUnits.GroupBy(u => Map.ToTilePos(u.Position)).Select(u => Map.ToTilePos(u.First().Position)).ToList();
            visionStartPosition.AddRange(PlayerBuildings.Select(b => b.Position));

            foreach(Vector2i checkPos in visionStartPosition)
            {
                VisibilityLevel[checkPos.X, checkPos.Y] = 1.0f;

                PropagateForward(checkPos.X + 1, checkPos.Y, 1, 0, 1.0f, 1);
                PropagateForward(checkPos.X, checkPos.Y + 1, 0, 1, 1.0f, 1);
                PropagateForward(checkPos.X - 1, checkPos.Y, -1, 0, 1.0f, 1);
                PropagateForward(checkPos.X, checkPos.Y - 1, 0, -1, 1.0f, 1);

                PropagateDiagonal(checkPos.X + 1, checkPos.Y+1, 1, 1, 1.0f, 1, false, false);
                PropagateDiagonal(checkPos.X-1, checkPos.Y + 1, -1, 1, 1.0f, 1, false, false);
                PropagateDiagonal(checkPos.X - 1, checkPos.Y-1, -1, -1, 1.0f, 1, false, false);
                PropagateDiagonal(checkPos.X+1, checkPos.Y - 1, 1, -1, 1.0f, 1, false, false);
            }
        }


        protected void PropagateForward(int posX, int posY, int dirX, int dirY, float currentVisibility, int distance)
        {
            if (!map.IsTileInBounds(posX, posY)) return;
            VisibilityLevel[posX, posY] = Math.Max(VisibilityLevel[posX, posY], currentVisibility);
            if (map.IsTileVisionObstructed(posX, posY)) return;

            if (distance >= VisibilityDistance) return;
            if(distance>VisibilityFallofStart)
            {
                float visibilityCoef = (1.0f - VisibilityMinLevel) / (VisibilityDistance - VisibilityFallofStart)*1.00f;
                currentVisibility -= visibilityCoef;
            }
            PropagateForward(posX + dirX, posY + dirY, dirX, dirY, currentVisibility, distance + 1);
        }

        protected void PropagateDiagonal(int posX, int posY, int dirX, int dirY, float currentVisibility, int distance, bool blockX, bool blockY)
        {
            if (!map.IsTileInBounds(posX, posY)) return;
            VisibilityLevel[posX, posY] = Math.Max(VisibilityLevel[posX, posY], currentVisibility);
            if (map.IsTileVisionObstructed(posX, posY)) return;

            if (distance >= VisibilityDistance) return;
            if (distance > VisibilityFallofStart)
            {
                float visibilityCoef = (1.0f - VisibilityMinLevel) / (VisibilityDistance - VisibilityFallofStart) * 1.00f;
                currentVisibility -= visibilityCoef;
            } 

            if(!blockX) PropagateForward(posX + dirX, posY, dirX, 0, currentVisibility, distance + 1);
            if(!blockY) PropagateForward(posX, posY + dirY, 0, dirY, currentVisibility, distance + 1);

            if (map.IsTileVisionObstructed(posX + dirX, posY)) blockX = true;
            if (map.IsTileVisionObstructed(posX, posY + dirY)) blockY = true;

            PropagateDiagonal(posX + dirX, posY + dirY, dirX, dirY, currentVisibility, distance + 1, blockX, blockY);
        }
    }
}
