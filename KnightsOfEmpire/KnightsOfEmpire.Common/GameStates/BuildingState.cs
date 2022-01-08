using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Units;
using SFML.Graphics;
using SFML.System;

namespace KnightsOfEmpire.Common.GameStates
{
    public class BuildingState : GameState
    {
        public List<Building>[] GameBuildings;
        public static int MaxBuildingSeparateDistance = 5;

        public override void Initialize()
        {
            GameBuildings = new List<Building>[4];
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                GameBuildings[i] = new List<Building>();
            }
        }

        protected float Distance2Building(Vector2f p, Building b)
        {
            float TileSize = Map.Map.TilePixelSize;
            FloatRect f = new FloatRect(new Vector2f(b.Position.X* TileSize, b.Position.Y* TileSize), 
                new Vector2f(TileSize, TileSize));
            return p.Distance2Rect(f);
        }

        public List<Building> GetEnemyBuildingsInRange(Unit unit, float range, float[,] visibilityLevel = null)
        {
            List<Building> targets = new List<Building>();
            Building nearest = null;
            for (int j = 0; j < MaxPlayerCount; j++)
            {
                if (j == unit.PlayerId) continue;
                List<Building> enemies = GameBuildings[j];
                for (int i = 0; i < enemies.Count; i++)
                {
                    Building another = enemies[i];
                    float currDist = Distance2Building(unit.Position, another);
                    if (currDist <= range * range)
                    {
                        if (nearest == null) nearest = another;
                        else
                        {
                            if (visibilityLevel != null)
                            {
                                Vector2i tilePos = another.Position;
                                if (visibilityLevel[tilePos.X, tilePos.Y] <= FogState.VisibilityMinLevel)
                                {
                                    continue;
                                }
                            }
                            if (currDist < Distance2Building(unit.Position, nearest))
                            {
                                targets.Add(nearest);
                                nearest = another;
                            }
                            else targets.Add(another);
                        }
                    }
                }
            }
            if (nearest != null)
            {
                targets.Add(nearest);
            }
            return targets;
        }
    }
}
