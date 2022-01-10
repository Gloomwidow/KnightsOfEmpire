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

        protected List<ReceivedPacket> packets = null;

        bool isMousePressed = false;

        public MatchGameState(List<ReceivedPacket> packets = null)
        {
            this.packets = packets;
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

        public override void LoadDependencies()
        {
            base.LoadDependencies();
            if(packets != null)
            {
                HandleTCPPackets(packets);
            }
            SentPacket gameReadyPacket = new SentPacket(PacketsHeaders.GameReadyPing);
            Client.TCPClient.SendToServer(gameReadyPacket);
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
            if(!Client.TCPClient.isRunning)
            {
                GameStateManager.GameState = new MainState($"An error occured with connection: {Client.TCPClient.LastError}");
            }
        }
    }
}
