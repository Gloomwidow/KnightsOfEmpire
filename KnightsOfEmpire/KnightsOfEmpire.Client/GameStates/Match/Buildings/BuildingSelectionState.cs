using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.GameStates.Match.Buildings
{
    public class BuildingSelectionState : GameState
    {
        public Building SelectedBuilding = null;
        public List<Building>[] GameBuildings;
        public int MainPanelHeight = 0;
        Vector2f pressPosition;
        bool alreadyPressed = false;

        public override void LoadDependencies()
        {
            GameBuildings = GameStateManager.GameState.GetSiblingGameState<BuildingUpdateState>().GameBuildings;
        }

        public override void Update()
        {
            MainPanelHeight = GameStateManager.GameState.GetSiblingGameState<GameGUIState>().MainPanelHeight;
            Vector2i clickPosition = Mouse.GetPosition(Client.RenderWindow);
            Vector2f clickMapPosition = Client.RenderWindow.MapPixelToCoords(clickPosition);
            if (Mouse.IsButtonPressed(Mouse.Button.Left) && !alreadyPressed)
            {
                if (clickPosition.Y < Client.RenderWindow.Size.Y - MainPanelHeight) 
                {
                    pressPosition = clickMapPosition;
                    alreadyPressed = true;
                    SelectedBuilding = null;
                    GameStateManager.GameState.GetSiblingGameState<GameGUIState>().ResetUnitButtons();
                }
            }
            else if (!Mouse.IsButtonPressed(Mouse.Button.Left) && alreadyPressed) 
            {
                if(clickMapPosition == pressPosition) 
                {
                    for (int i = 0; i < GameBuildings[Client.Resources.PlayerGameId].Count; i++)
                    {
                        Building building = GameBuildings[Client.Resources.PlayerGameId][i];
                        Vector2i position = new Vector2i(building.Position.X * Map.TilePixelSize, building.Position.Y * Map.TilePixelSize);
                        IntRect buildingRect = new IntRect(position, new Vector2i(Map.TilePixelSize, Map.TilePixelSize));
                        if (buildingRect.Contains((int)clickMapPosition.X, (int)clickMapPosition.Y)) 
                        {
                            SelectedBuilding = GameBuildings[Client.Resources.PlayerGameId][i];
                            GameStateManager.GameState.GetSiblingGameState<GameGUIState>().SetUnitButtons();
                        } 
                    }
                }
                alreadyPressed = false;
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape)) 
            {
                SelectedBuilding = null;
                GameStateManager.GameState.GetSiblingGameState<GameGUIState>().ResetUnitButtons();
            }
        }
        
    }
}
