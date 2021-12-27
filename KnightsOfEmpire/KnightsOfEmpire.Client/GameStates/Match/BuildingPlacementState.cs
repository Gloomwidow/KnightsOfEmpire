using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Helper;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Buildings;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnightsOfEmpire.GameStates.Match
{
    public class BuildingPlacementState : GameState
    {
        protected int buildingIdToPlace = -1;
        public int BuildingIdToPlace
        {
            get
            {
                return buildingIdToPlace;
            }
            set
            {
                buildingIdToPlace = value;
                checkedEligibility = false;
            }
        }

        protected List<Building> friendlyBuildings;

        protected bool checkedEligibility = false;
        protected bool isMousePressed = false;
        protected Texture BuildingsAtlas;
        protected int AtlasSizeX, AtlasSizeY;
        protected Vector2f spawnPos = new Vector2f(0, 0);

        public override void LoadDependencies()
        {
            BuildingsAtlas = GameStateManager.GameState.GetSiblingGameState<BuildingUpdateState>().BuildingsAtlas;
            AtlasSizeX = (int)BuildingsAtlas.Size.X;
            AtlasSizeX = (int)BuildingsAtlas.Size.Y;
            friendlyBuildings = GameStateManager.GameState.GetSiblingGameState<BuildingUpdateState>().
                GameBuildings[Client.Resources.PlayerGameId];
        }

        public bool IsInFriendlyDistance(Vector2i tileBuildPos)
        {
            if (friendlyBuildings.Count > 0)
            {
                bool inRange = false;
                foreach (Building b in friendlyBuildings)
                {
                    int dist = Math.Max(Math.Abs(b.Position.X - tileBuildPos.X), Math.Abs(b.Position.Y - tileBuildPos.Y));
                    if (dist < BuildingState.MaxBuildingSeparateDistance)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        public override void Update()
        {
            if(!checkedEligibility)
            {
                if(BuildingIdToPlace!=-1)
                {
                    Building b = BuildingManager.GetBuilding(BuildingIdToPlace);
                    if (b.BuildCost > Client.Resources.GoldAmount) BuildingIdToPlace = -1;
                }
                checkedEligibility = true;
            }
            int panelHeight = GameStateManager.GameState.GetSiblingGameState<GameGUIState>().MainPanelHeight;
            if (Client.RenderWindow.HasFocus())
            {
                Vector2i clickPos = Mouse.GetPosition(Client.RenderWindow);
                spawnPos = Client.RenderWindow.MapPixelToCoords(clickPos);
                if (!isMousePressed && Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    isMousePressed = true;
                    if (clickPos.Y < Client.RenderWindow.Size.Y - panelHeight) // clicked on map
                    {
                        if (Client.Resources.Map.CanUnitBeSpawnedOnPos(spawnPos)
                            && !Client.Resources.Map.HasWallInNeighborhood(Map.ToTilePos(spawnPos))
                            && IsInFriendlyDistance(Map.ToTilePos(spawnPos))
                            && BuildingIdToPlace!=-1)
                        {
                            CreateBuildingRequest request = new CreateBuildingRequest
                            {
                                BuildingTypeId = BuildingIdToPlace,
                                BuildingPosX = (int)spawnPos.X,
                                BuildingPosY = (int)spawnPos.Y,

                            };

                            SentPacket packet = new SentPacket(PacketsHeaders.CreateBuildingRequest);
                            packet.stringBuilder.Append(JsonSerializer.Serialize(request));
                            Client.TCPClient.SendToServer(packet);
                            BuildingIdToPlace = -1;
                        }
                    }
                }
                else if (!Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    isMousePressed = false;
                }
            }
            if (Keyboard.IsKeyPressed(Keyboard.Key.Escape))
            {
                BuildingIdToPlace = -1;
            }
        }

        public override void Render()
        { 
            base.Render();
            Map map = Client.Resources.Map;

            if (BuildingIdToPlace == -1) return;

            if(map.IsPositionInBounds(spawnPos))
            {
                RectangleShape buildingShape = new RectangleShape();
                Vector2i spawnPosTile = Map.ToTilePos(spawnPos);
                buildingShape.Size = new Vector2f(Map.TilePixelSize, Map.TilePixelSize);
                float PositionX = spawnPosTile.X * Map.TilePixelSize;
                float PositionY = spawnPosTile.Y * Map.TilePixelSize;
                buildingShape.Position = new Vector2f(PositionX, PositionY);
                buildingShape.Texture = BuildingsAtlas;
                buildingShape.TextureRect = IdToTextureRect.GetRect(
                    BuildingManager.GetTextureId(BuildingIdToPlace),
                    AtlasSizeX, AtlasSizeY);
                if (map.CanUnitBeSpawnedOnPos(spawnPos) && 
                    !map.HasWallInNeighborhood(spawnPosTile)
                    && IsInFriendlyDistance(spawnPosTile))
                {
                    buildingShape.FillColor = new Color(255, 255, 255, 128);  
                }
                else
                {
                    buildingShape.FillColor = new Color(255, 0, 0, 128);
                }
                Client.RenderWindow.Draw(buildingShape);
            }
        }
    }
}
