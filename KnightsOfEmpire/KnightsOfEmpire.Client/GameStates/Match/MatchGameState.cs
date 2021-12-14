using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Units;
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
    public class MatchGameState : GameState
    {
        public ViewControlState ViewControlState;
        public MapRenderState MapRenderState;
        public GameGUIState GameGUIState;
        public UnitUpdateState UnitUpdateState;
        public UnitOrdersState UnitsOrdersState;
        public FogOfWarState FogOfWarState;

        bool isMousePressed = false;


        public override void LoadResources()
        {
            ViewControlState = new ViewControlState();
            MapRenderState = new MapRenderState();
            GameGUIState = new GameGUIState();
            UnitUpdateState = new UnitUpdateState();
            UnitsOrdersState = new UnitOrdersState();
            FogOfWarState = new FogOfWarState();
            MapRenderState.LoadResources();
        }

        public override void Initialize()
        {
            MapRenderState.GameMap = Client.Resources.Map;
            MapRenderState.Initialize();
            UnitUpdateState.Initialize();

            ViewControlState.SetCameraBounds(MapRenderState.GetMapBounds());

            ViewControlState.Initialize();

            GameGUIState.Initialize();
            FogOfWarState.Initialize();

            FogOfWarState.PlayerUnits = UnitUpdateState.GameUnits[Client.Resources.PlayerGameId];
            UnitsOrdersState.GameUnits = UnitUpdateState.GameUnits;
            MapRenderState.VisibilityLevel = FogOfWarState.VisibilityLevel;
            UnitUpdateState.VisibilityLevel = FogOfWarState.VisibilityLevel;
        }

        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            foreach(ReceivedPacket packet in packets)
            {
                if(packet.GetHeader().StartsWith(PacketsHeaders.GameUnitHeaderStart))
                {
                    UnitUpdateState.HandleTCPPacket(packet);
                }
            }
        }

        public override void HandleUDPPackets(List<ReceivedPacket> packets)
        {
            UnitUpdateState.HandleUDPPackets(packets);
        }

        public override void Update()
        {
            ViewControlState.ViewBottomBoundGuiHeight = GameGUIState.MainPanelHeight;
            UnitsOrdersState.MainPanelHeight = GameGUIState.MainPanelHeight;

            //TO-DO: convert this behavior to button press on MainState GUI
            if (Client.RenderWindow.HasFocus())
            {
                if (!isMousePressed && Mouse.IsButtonPressed(Mouse.Button.Left) && Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                {
                    isMousePressed = true;
                    Vector2i clickPos = Mouse.GetPosition(Client.RenderWindow);
                    if (clickPos.Y < Client.RenderWindow.Size.Y - GameGUIState.MainPanelHeight) // clicked on map
                    {
                        Vector2f spawnPos = Client.RenderWindow.MapPixelToCoords(clickPos);
                        if (MapRenderState.GameMap.CanUnitBeSpawnedOnPos(spawnPos))
                        {
                            TrainUnitRequest request = new TrainUnitRequest
                            {
                                UnitTypeId = 0,
                                BuildingPosX = (int)spawnPos.X,
                                BuildingPosY = (int)spawnPos.Y,
                            };

                            SentPacket packet = new SentPacket(PacketsHeaders.GameUnitTrainRequest, -1);
                            packet.stringBuilder.Append(JsonSerializer.Serialize(request));
                            Client.TCPClient.SendToServer(packet);
                        }
                    }
                }
                else if (!Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    isMousePressed = false;
                }
            }


            UnitUpdateState.Update();
            UnitsOrdersState.Update();
            ViewControlState.Update();
            FogOfWarState.Update();
            GameGUIState.Update();
        }

        public override void Render()
        {
            MapRenderState.RenderView = ViewControlState.View;
            MapRenderState.Render();
            UnitUpdateState.Render();
            GameGUIState.Render();
        }

        public override void Dispose()
        {
            ViewControlState.Dispose();
            MapRenderState.Dispose();
            GameGUIState.Dispose();
            UnitUpdateState.Dispose();
            FogOfWarState.Dispose();
        }
    }
}
