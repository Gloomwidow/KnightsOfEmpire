using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.GameStates.Match
{
    public class MatchGameState : GameState
    {
        public UnitsSelectionState UnitsSelectionState;
        public ViewControlState ViewControlState;
        public MapRenderState MapRenderState;


        public override void LoadResources()
        {
            MapRenderState = new MapRenderState();
            MapRenderState.LoadResources();
        }

        public override void Initialize()
        {
            UnitsSelectionState = new UnitsSelectionState();
            ViewControlState = new ViewControlState();
            MapRenderState.GameMap = new Map("64x64test.kmap");
            MapRenderState.Initialize();

            ViewControlState.SetCameraBounds(MapRenderState.GetMapBounds());

            ViewControlState.Initialize();
            UnitsSelectionState.Initialize();
        }

        public override void Update()
        {
            ViewControlState.Update();
            UnitsSelectionState.Update();
        }

        public override void Render()
        {
            MapRenderState.RenderView = ViewControlState.View;
            MapRenderState.Render();
            UnitsSelectionState.Render(); 
        }

        public override void Dispose()
        {
            ViewControlState.Dispose();
            UnitsSelectionState.Dispose();
            MapRenderState.Dispose();
        }
    }
}
