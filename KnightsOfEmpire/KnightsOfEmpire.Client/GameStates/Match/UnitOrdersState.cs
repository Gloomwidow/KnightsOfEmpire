using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Units;
using KnightsOfEmpire.Common.Units;
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
    public class UnitOrdersState : GameState
    {
        public List<Unit>[] GameUnits;
        bool isRightMouseClicked = false;
        public int MainPanelHeight = 0;

        public override void Update()
        {
            if (!Client.RenderWindow.HasFocus()) return;

            bool clicked = Mouse.IsButtonPressed(Mouse.Button.Right);

            if (clicked && !isRightMouseClicked)
            {
                isRightMouseClicked = true;
                Vector2i clickPos = Mouse.GetPosition(Client.RenderWindow);
                if (clickPos.Y < Client.RenderWindow.Size.Y - MainPanelHeight) // clicked on map
                {
                    Vector2f spawnPos = Client.RenderWindow.MapPixelToCoords(clickPos);
                    if (Client.Resources.Map.CanUnitBeSpawnedOnPos(spawnPos))
                    {
                        MoveUnitsToPositionRequest request = new MoveUnitsToPositionRequest
                        {
                            IgnoreAttack = true,
                            GoalX = (int)spawnPos.X,
                            GoalY = (int)spawnPos.Y
                        };
                        List<string> candidates = new List<string>();
                        foreach (Unit u in GameUnits[Client.Resources.PlayerGameId])
                        {
                            if (u.IsSelected) candidates.Add(new string(u.ID));
                        }
                        if(candidates.Count>0)
                        {
                            request.UnitIDs = candidates.ToArray();
                            SentPacket packet = new SentPacket(PacketsHeaders.GameUnitsMoveToTileRequest, -1);
                            packet.stringBuilder.Append(JsonSerializer.Serialize(request));
                            Client.TCPClient.SendToServer(packet);
                            Console.WriteLine(packet.stringBuilder.ToString());
                            Console.WriteLine($"Moving units:{request.UnitIDs}");
                        }
                    }
                }
            }
            else if (!clicked)
            {
                isRightMouseClicked = false;
            }
        }
    }
}
