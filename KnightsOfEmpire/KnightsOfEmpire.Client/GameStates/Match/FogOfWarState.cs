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
    public class FogOfWarState : FogState
    {
        public List<Unit> PlayerUnits;
        public List<Building> PlayerBuildings;

        public override void Initialize()
        {
            map = Client.Resources.Map;
            visibilityLevels = new float[1][,];
            visibilityLevels[0] = new float[map.TileCountX, map.TileCountY];
        }

        public override void LoadDependencies()
        {
            PlayerUnits = Parent.GetSiblingGameState<UnitUpdateState>().GameUnits[Client.Resources.PlayerGameId];
            PlayerBuildings = Parent.GetSiblingGameState<BuildingUpdateState>().GameBuildings[Client.Resources.PlayerGameId];
        }

        public override void Update()
        {
            base.Update();

            List<(Vector2i, int)> visionStartPosition = PlayerUnits.GroupBy(u => Map.ToTilePos(u.Position)).
                    Select(g =>(
                        g.Key,
                        g.OrderByDescending(u => u.Stats.VisibilityDistance)
                         .Select(u => u.Stats.VisibilityDistance)
                         .FirstOrDefault())).ToList();

            visionStartPosition.AddRange(PlayerBuildings.Select(b => (b.Position, BuildingVisibilityDistance)));

            CalculateVision(visionStartPosition);
        }
    }
}
