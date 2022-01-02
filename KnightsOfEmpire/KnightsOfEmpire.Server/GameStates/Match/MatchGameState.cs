using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Player;
using KnightsOfEmpire.Server.GameStates.Match;

namespace KnightsOfEmpire.Server.GameStates
{
    public class MatchGameState : GameState
    {
        public MatchGameState()
        {
            RegisterGameState(new UnitUpdateState());
            RegisterGameState(new BuildingUpdateState());
            RegisterTCPRedirects(new (string Header, Type T)[]
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

        public override void Update()
        {
            base.Update();
            if (Server.TCPServer.CurrentActiveConnections<=0)
            {
                Console.WriteLine("No one is connected. Resetting to WaitingState");
                GameStateManager.GameState = new WaitingGameState();
            }
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                if (Server.Resources.IsDefeated[i])
                {
                    if(!Server.Resources.IsDefeatDone[i])
                    {
                        GetSiblingGameState<UnitUpdateState>().DeleteAllUnits(i);
                        GetSiblingGameState<BuildingUpdateState>().DestroyAllBuildings(i);
                        Server.Resources.IsDefeatDone[i] = true;
                    }
                }
                if (Server.Resources.HasChanged[i]) 
                {
                    if (!Server.TCPServer.IsClientConnected(i)) continue;
                    ChangePlayerInfoRequest request = new ChangePlayerInfoRequest
                    {
                        PlayerId = i,
                        PlayersLeft = Server.Resources.PlayersLeft,
                        GoldAmount = Server.Resources.GoldAmount[i],
                        IsDefeated = Server.Resources.IsDefeated[i],
                        CurrentUnitsCapacity = Server.Resources.CurrentUnitsCapacity[i],
                        MaxUnitsCapacity = Server.Resources.MaxUnitsCapacity[i]
                    };
                    //This little hack will disable player won screen if only one player comes to game
                    //TO-DO: remove this or make flag to enable and disable testing things like that
                    if (Server.Resources.StartPlayerCount==1)
                    {
                        request.PlayersLeft++;
                    }
                    SentPacket registerPacket = new SentPacket(PacketsHeaders.ChangePlayerInfoRequest, -1);
                    registerPacket.stringBuilder.Append(JsonSerializer.Serialize(request));

                    registerPacket.ClientID = i;
                    Server.TCPServer.SendToClient(registerPacket);

                    Server.Resources.HasChanged[i] = false;
                }
            }
        }
    }
}
