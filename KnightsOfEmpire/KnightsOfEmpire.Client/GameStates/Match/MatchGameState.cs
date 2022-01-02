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

        bool isMousePressed = false;

        public MatchGameState()
        {
            GameGUIState = new GameGUIState();
            RegisterGameState(new ViewControlState());
            RegisterGameState(new MapRenderState());
            RegisterGameState(new UnitOrdersState());
            RegisterGameState(new UnitUpdateState());
            RegisterGameState(new BuildingUpdateState());
            RegisterGameState(GameGUIState);
            RegisterGameState(new FogOfWarState());
            RegisterTCPRedirects(new (string Header, Type T)[]
                {
                    (PacketsHeaders.GameUnitHeaderStart, typeof(UnitUpdateState)),
                    (PacketsHeaders.ChangePlayerInfoRequest, typeof(GameGUIState)),
                    (PacketsHeaders.BuildingHeaderStart, typeof(BuildingUpdateState))
                }
            );

            RegisterUDPRedirects(new (string Header, Type T)[]
                {
                    (PacketsHeaders.GameUnitHeaderStart, typeof(UnitUpdateState)),
                    (PacketsHeaders.BuildingHeaderStart, typeof(BuildingUpdateState))
                }
            );
        }


        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            RedirectTCPPackets(packets);
        }

        public override void HandleUDPPackets(List<ReceivedPacket> packets)
        {
            RedirectUDPPackets(packets);
        }

        public override void Update()
        {
            base.Update();

            //TO-DO: convert this behavior to button press on MainState GUI
            if (Client.RenderWindow.HasFocus())
            {
                if (!isMousePressed && Mouse.IsButtonPressed(Mouse.Button.Left))
                {
                    int unitTypeId = -1;
                    if (Keyboard.IsKeyPressed(Keyboard.Key.Num1)) unitTypeId = 0;
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Num2)) unitTypeId = 1;
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Num3)) unitTypeId = 2;
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Num4)) unitTypeId = 3;
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Num5)) unitTypeId = 4;
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Num6)) unitTypeId = 5;
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Num7)) unitTypeId = 6;
                    else if (Keyboard.IsKeyPressed(Keyboard.Key.Num8)) unitTypeId = 7;
                    if (unitTypeId == -1) return;
                    isMousePressed = true;
                    Vector2i clickPos = Mouse.GetPosition(Client.RenderWindow);
                    if (clickPos.Y < Client.RenderWindow.Size.Y - GameGUIState.MainPanelHeight) // clicked on map
                    {
                        Vector2f spawnPos = Client.RenderWindow.MapPixelToCoords(clickPos);
                        if (Client.Resources.Map.CanUnitBeSpawnedOnPos(spawnPos))
                        {
                            TrainUnitRequest request = new TrainUnitRequest
                            {
                                UnitTypeId = unitTypeId,
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
