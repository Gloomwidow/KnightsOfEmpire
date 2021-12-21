using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Buildings;
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
        public GameGUIState GameGUIState;
        public UnitUpdateState UnitUpdateState;
        public BuildingUpdateState BuildingUpdateState;

        bool isMousePressed = false;

        public MatchGameState()
        {
            GameGUIState = new GameGUIState();
            BuildingUpdateState = new BuildingUpdateState();
            UnitUpdateState = new UnitUpdateState();
            RegisterGameState(new ViewControlState());
            RegisterGameState(new MapRenderState());
            RegisterGameState(new UnitOrdersState());
            RegisterGameState(UnitUpdateState);
            RegisterGameState(BuildingUpdateState);
            RegisterGameState(GameGUIState);
            RegisterGameState(new FogOfWarState());
            
        }

        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            foreach(ReceivedPacket packet in packets)
            {
                if(packet.GetHeader().StartsWith(PacketsHeaders.GameUnitHeaderStart))
                {
                    UnitUpdateState.HandleTCPPacket(packet);
                }
                if (packet.GetHeader().StartsWith(PacketsHeaders.ChangePlayerInfoRequest)) 
                {
                    GameGUIState.HandleTCPPacket(packet);
                }
                if (packet.GetHeader().StartsWith(PacketsHeaders.BuildingHeaderStart)) 
                {
                    BuildingUpdateState.HandleTCPPacket(packet);
                }
            }
        }

        public override void HandleUDPPackets(List<ReceivedPacket> packets)
        {
            UnitUpdateState.HandleUDPPackets(packets);
        }

        public override void Update()
        {
            base.Update();

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
                        if (Client.Resources.Map.CanUnitBeSpawnedOnPos(spawnPos))
                        {
                            TrainUnitRequest request = new TrainUnitRequest
                            {
                                UnitTypeId = 0,
                                BuildingPosX = (int)spawnPos.X,
                                BuildingPosY = (int)spawnPos.Y,
                            };

                            SentPacket packet = new SentPacket(PacketsHeaders.GameUnitTrainRequest);
                            packet.stringBuilder.Append(JsonSerializer.Serialize(request));
                            Client.TCPClient.SendToServer(packet);
                        }
                    }
                }
                else if (!isMousePressed && Mouse.IsButtonPressed(Mouse.Button.Left) && Keyboard.IsKeyPressed(Keyboard.Key.LShift))
                {
                    isMousePressed = true;
                    Vector2i clickPos = Mouse.GetPosition(Client.RenderWindow);
                    if (clickPos.Y < Client.RenderWindow.Size.Y - GameGUIState.MainPanelHeight) // clicked on map
                    {
                        Vector2f spawnPos = Client.RenderWindow.MapPixelToCoords(clickPos);
                        if (Client.Resources.Map.CanUnitBeSpawnedOnPos(spawnPos))
                        {
                            CreateBuildingRequest request = new CreateBuildingRequest
                            {
                                BuildingTypeId = 0,
                                BuildingPosX = (int)spawnPos.X,
                                BuildingPosY = (int)spawnPos.Y,

                            };

                            SentPacket packet = new SentPacket(PacketsHeaders.GameUnitTrainRequest);
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
        }
    }
}
