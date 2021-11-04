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
        public GameGUIState GameGUIState;


        public override void LoadResources()
        {
            UnitsSelectionState = new UnitsSelectionState();
            ViewControlState = new ViewControlState();
            MapRenderState = new MapRenderState();
            GameGUIState = new GameGUIState();
            MapRenderState.LoadResources();
        }

        public override void Initialize()
        {
            MapRenderState.GameMap = new Map("64x64test.kmap");
            MapRenderState.Initialize();

            ViewControlState.SetCameraBounds(MapRenderState.GetMapBounds());

            ViewControlState.Initialize();
            UnitsSelectionState.Initialize();

            GameGUIState.Initialize();
        }

        public override void Update()
        {
            ViewControlState.ViewBottomBoundGuiHeight = GameGUIState.MainPanelHeight;
            ViewControlState.Update();
            UnitsSelectionState.Update();
            GameGUIState.Update();
        }

        public override void Render()
        {
            MapRenderState.RenderView = ViewControlState.View;
            MapRenderState.Render();
            UnitsSelectionState.Render();
            GameGUIState.Render();
        }

        public override void Dispose()
        {
            ViewControlState.Dispose();
            UnitsSelectionState.Dispose();
            MapRenderState.Dispose();
            GameGUIState.Dispose();
        }
    }
}
