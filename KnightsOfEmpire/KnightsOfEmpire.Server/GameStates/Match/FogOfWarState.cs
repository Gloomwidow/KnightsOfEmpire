using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Units;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.GameStates.Match
{
    public class FogOfWarState : FogState
    {
        public List<Unit>[] GameUnits;
        public List<Building>[] GameBuildings;

        public override void Initialize()
        {
            map = Server.Resources.Map;
            visibilityLevels = new float[MaxPlayerCount][,];
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                visibilityLevels[i] = new float[map.TileCountX, map.TileCountY];
            }
        }

        public float[,] GetPlayerVisibilityField(int playerID)
        {
            return visibilityLevels[playerID];
        }

        public override void LoadDependencies()
        {
            GameUnits = Parent.GetSiblingGameState<UnitUpdateState>().GameUnits;
            GameBuildings = Parent.GetSiblingGameState<BuildingUpdateState>().GameBuildings;
        }

        public override void Update()
        {
            base.Update();
            Parallel.For(0, MaxPlayerCount, (i, state) =>
            {
                List<(Vector2i, int)> visionStartPosition = GameUnits[i].GroupBy(u => Map.ToTilePos(u.Position)).
                  Select(g => (
                      g.Key,
                      g.OrderByDescending(u => u.Stats.VisibilityDistance)
                       .Select(u => u.Stats.VisibilityDistance)
                       .FirstOrDefault())).ToList();

                visionStartPosition.AddRange(GameBuildings[i].Select(b => (b.Position, BuildingVisibilityDistance)));

                CalculateVision(visionStartPosition, i);
            }
            );
        }
    }
}
