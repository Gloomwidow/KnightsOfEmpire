using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using TGUI;

using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Networking.TCP;
using KnightsOfEmpire.Common.Resources;
using KnightsOfEmpire.Common.Resources.Waiting;

using KnightsOfEmpire.GameStates.Match;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Helper;

namespace KnightsOfEmpire.GameStates
{
    class WaitingState : GameState
    {
        // State to manage GameState

        protected Map[] ServerMaps;
        protected Texture[] MapPreviews;
        private enum State { First, Main, Update, StartGame}
        private State state = State.First;

        // Players list
        private bool clientReady = false;
        private string[] playersNicknames = null;
        private bool[] playersReadyStatus = null;
        private int[] selectedMapId = null;
        private int currentPickedMap = 0;

        // Panel for waiting to server first response
        private Panel connectPanel;

        // Panel for wait state
        private Panel waitingPanel;

        private Picture mapPreview;

        private const int maxPlayers = 4;
        private ScrollablePanel playerListPanel;
        private const string numLabelStr = "Num";
        private const string nicknameLabelStr = "Nick";
        private const string readyLabelStr = "Ready";
        private const string selectedMapLabelStr = "MapSelect";

        private Button readyButton;
        private Button notReadyButton;

        protected ComboBox mapPicker;

        /// <summary>
        /// Initialize GUI
        /// </summary>
        public override void Initialize()
        {
            InitializeWaitingPanel();
            waitingPanel.Visible = true;
            Client.Gui.Add(waitingPanel);

            for(int i = 0; i<maxPlayers; i++)
            {
                Panel panel = CreatePlayerPanel(i + 1, "", "", false, maxPlayers);
                panel.Position = new Vector2f(10, i * 40);
                panel.Visible = false;
                playerListPanel.Add(panel, "Player" + i.ToString());
            }

            InitializeConnectPanel();
            connectPanel.Visible = true;
            Client.Gui.Add(connectPanel);

        }

        /// <summary>
        /// Handle TCP request from server
        /// </summary>
        /// <param name="packets">List of recived packets</param>
        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            foreach (ReceivedPacket packet in packets)
            {
                switch(packet.GetHeader())
                {
                    case PacketsHeaders.PING:
                        break;

                    case PacketsHeaders.WaitingStateServerResponse:
                        HandleWaitingStateServerResponse(packet);
                        break;

                    case PacketsHeaders.MapServerResponse:
                        HandleMapServerResponse(packet);
                        break;

                    case PacketsHeaders.CustomUnitsServerResponse:
                        HandleCustomUnitsServerResponse(packet);
                        break;

                    case PacketsHeaders.StartGameServerRequest:
                        HandleStartGameServerRequest(packet);
                        break;
                }
            }
        }

        /// <summary>
        /// Uppdate state
        /// </summary>
        public override void Update()
        {
            if (Client.TCPClient == null || !Client.TCPClient.isRunning)
            {
                GameStateManager.GameState = new MainState($"An error occured with connection: {Client.TCPClient.LastError}");
            }
            switch (state)
            {
                case State.First:
                    {
                        connectPanel.Visible = true;
                    }
                    break;
                case State.Main:
                    {
                        connectPanel.Visible = false;
                    }
                    break;
                case State.Update:
                    {
                        int playerNumber = 0;
                        for (int i=0; i<playersNicknames.Length; i++)
                        {
                            if(playersNicknames[i] != string.Empty)
                            {
                                playerNumber++;

                                if(playersNicknames[i] == Client.Resources.Nickname)
                                {
                                    // Uppdate buttons
                                    if(playersReadyStatus[i])
                                    {
                                        readyButton.Enabled = false;
                                        notReadyButton.Enabled = true;
                                    }
                                    else
                                    {
                                        readyButton.Enabled = true;
                                        notReadyButton.Enabled = false;
                                    }
                                }
                            }
                        }

                        int j = 0;
                        for (int i = 0; i < playersNicknames.Length; i++)
                        {
                            if (playersNicknames[i] != string.Empty)
                            {
                                Panel panel = (Panel)playerListPanel.Get("Player" + j.ToString());
                                ((Label)panel.Get(nicknameLabelStr)).Text = playersNicknames[i];
                                ((Label)panel.Get(readyLabelStr)).Text = playersReadyStatus[i] ? "Yes" : "No";
                                ((Label)panel.Get(selectedMapLabelStr)).Text = ServerMaps!=null?ServerMaps[selectedMapId!=null?selectedMapId[i]:0].MapName:"";
                                panel.Visible = true;
                                j++;
                            }
                        }
                        for (; j < maxPlayers; j++)
                        {
                            Panel panel = (Panel)playerListPanel.Get("Player" + j.ToString());
                            panel.Visible = false;
                        }

                        state = State.Main;
                    }
                    break;
            }
        }

        /// <summary>
        /// Draw GUI
        /// </summary>
        public override void Render()
        {
            Client.Gui.Draw();
        }

        /// <summary>
        /// Remove all widgets from GUI
        /// </summary>
        public override void Dispose()
        {
            Client.Gui.RemoveAllWidgets();
        }

        private void InitializeWaitingPanel()
        {
            Vector2u windowSize = Client.RenderWindow.Size;

            waitingPanel = new Panel();
            waitingPanel.Position = new Vector2f(0, 0);
            waitingPanel.Size = new Vector2f(1280, 720);

            Label label = new Label();
            label.Text = "Knights of Empire";
            label.Position = new Vector2f(0, 110);
            label.Size = new Vector2f(1280, 80);
            label.TextSize = 50;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.IgnoreMouseEvents = true;
            waitingPanel.Add(label);

            label = new Label();
            label.Text = "Wait for player";
            label.Position = new Vector2f(0, 170);
            label.Size = new Vector2f(1280, 32);
            label.TextSize = 20;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.IgnoreMouseEvents = true;
            waitingPanel.Add(label);

            label = new Label();
            label.Text = "Map preview";
            label.Position = new Vector2f(60, 210);
            label.Size = new Vector2f(440, 32);
            label.TextSize = 24;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.IgnoreMouseEvents = true;
            waitingPanel.Add(label);

            Panel panel = new Panel();
            panel.Position = new Vector2f(110, 300);
            panel.Size = new Vector2f(340, 280);
            panel.Renderer.BackgroundColor = new Color(247, 247, 247);
            waitingPanel.Add(panel);

            mapPreview = new Picture();
            mapPreview.Position = new Vector2f(110, 300);
            mapPreview.Size = new Vector2f(340, 340);
            waitingPanel.Add(mapPreview);

            mapPicker = new ComboBox();
            mapPicker.Position = new Vector2f(110, 240);
            mapPicker.Size = new Vector2f(340, 40);
            mapPicker.ItemSelected += UpdateMapSelection;
            waitingPanel.Add(mapPicker);
            


            label = new Label();
            label.Text = "Player list";
            label.Position = new Vector2f(440, 250);
            label.Size = new Vector2f(840, 32);
            label.TextSize = 24;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.IgnoreMouseEvents = true;
            waitingPanel.Add(label);

            Panel panelLabel = new Panel();
            panelLabel.Position = new Vector2f(560, 300);
            panelLabel.Size = new Vector2f(600, 35);
            panelLabel.Renderer.BackgroundColor = new Color(247, 247, 247);

            label = new Label();
            label.Text = "Nickname";
            label.Position = new Vector2f(0, 10);
            label.Size = new Vector2f(320, 20);
            label.TextSize = 14;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panelLabel.Add(label);

            label = new Label();
            label.Text = "Selected Map";
            label.Position = new Vector2f(180, 10);
            label.Size = new Vector2f(320, 20);
            label.TextSize = 14;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panelLabel.Add(label);

            label = new Label();
            label.Text = "Is ready";
            label.Position = new Vector2f(400, 10);
            label.Size = new Vector2f(190, 20);
            label.TextSize = 14;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panelLabel.Add(label);

            waitingPanel.Add(panelLabel);

            playerListPanel = new ScrollablePanel();
            playerListPanel.Position = new Vector2f(560, 335);
            playerListPanel.Size = new Vector2f(600, 160);
            playerListPanel.Renderer.BackgroundColor = new Color(247, 247, 247);

            waitingPanel.Add(playerListPanel);

            readyButton = new Button();
            readyButton.Text = "Ready";
            readyButton.Position = new Vector2f(685, 520);
            readyButton.Size = new Vector2f(150, 40);
            readyButton.TextSize = 18;
            readyButton.Clicked += ReadyButton;
            waitingPanel.Add(readyButton);

            notReadyButton = new Button();
            notReadyButton.Text = "Not ready";
            notReadyButton.Position = new Vector2f(885, 520);
            notReadyButton.Size = new Vector2f(150, 40);
            notReadyButton.TextSize = 18;
            notReadyButton.Enabled = false;
            notReadyButton.Clicked += NotReadyButton;
            waitingPanel.Add(notReadyButton);

            Button button = new Button();
            button.Text = "Disconnect";
            button.Position = new Vector2f(1095, 650);
            button.Size = new Vector2f(150, 40);
            button.TextSize = 18;
            button.Clicked += DisconnectButton;
            waitingPanel.Add(button);

        }

        private void ReadyButton(object sender, EventArgs e)
        {
            clientReady = true;
            SendInfoPacket();
        }

        private void NotReadyButton(object sender, EventArgs e)
        {
            clientReady = false;
            SendInfoPacket();
        }


        public void UpdateMapSelection(object sender, EventArgs e)
        {
            currentPickedMap = mapPicker.GetSelectedItemIndex();
            mapPreview.Renderer.Texture = MapPreviews[currentPickedMap];
            SendInfoPacket();
        }

        private void DisconnectButton(object sender, EventArgs e)
        {
            if(Client.TCPClient != null)
            {
                Client.TCPClient.Stop();
            }
            GameStateManager.GameState = new MainState("Disconnected from server");
        }

        private Panel CreatePlayerPanel(int number, string nickname, string selectedMap, bool isReady, int total = 4)
        {
            Panel panel = new Panel();
            panel.Size = new Vector2f(total > 4 ? 565 : 580, 30);

            Label label = new Label();
            label.Text = number.ToString();
            label.Position = new Vector2f(0, 0);
            label.Size = new Vector2f(70, 30);
            label.TextSize = 18;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label, numLabelStr);

            label = new Label();
            label.Text = nickname;
            label.Position = new Vector2f(100, 0);
            label.Size = new Vector2f(320, 30);
            label.TextSize = 18;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label, nicknameLabelStr);

            label = new Label();
            label.Text = selectedMap;
            label.Position = new Vector2f(160, 0);
            label.Size = new Vector2f(320, 30);
            label.TextSize = 18;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label, selectedMapLabelStr);

            label = new Label();
            if (isReady)
                label.Text = "Yes";
            else
                label.Text = "No";
            label.Position = new Vector2f(390, 0);
            label.Size = new Vector2f(190, 30);
            label.TextSize = 18;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label, readyLabelStr);

            return panel;
        }

        private void InitializeConnectPanel()
        {
            connectPanel = new Panel();
            connectPanel.Position = new Vector2f(0, 0);
            connectPanel.Size = new Vector2f(1280, 720);
            connectPanel.Renderer.BackgroundColor = new Color(247, 247, 247);
            connectPanel.Renderer.Opacity = 0.9f;

            Label connectLabel = new Label();
            connectLabel.Text = "Waiting for server response...";
            connectLabel.Position = new Vector2f(0, 320);
            connectLabel.Size = new Vector2f(1280, 70);
            connectLabel.HorizontalAlignment = HorizontalAlignment.Center;
            connectLabel.TextSize = 50;
            connectPanel.Add(connectLabel);
        }

        // TCP Send message

        private void SendInfoPacket()
        {
            SentPacket infoPacket = new SentPacket(PacketsHeaders.WaitingStateClientRequest);

            WaitingStateClientRequest request = new WaitingStateClientRequest();
            request.IsReady = clientReady;
            request.Nickname = Client.Resources.Nickname;
            request.SelectedMap = currentPickedMap;
            infoPacket.stringBuilder.Append(JsonSerializer.Serialize(request));

            Client.TCPClient.SendToServer(infoPacket);
        }

        private void SendMapRequest(bool isMapRecved)
        {
            SentPacket mapPacket = new SentPacket(PacketsHeaders.MapClientRequest);

            MapClientRequest request = new MapClientRequest();
            request.MapReceived = isMapRecved;

            mapPacket.stringBuilder.Append(JsonSerializer.Serialize(request));

            Client.TCPClient.SendToServer(mapPacket);

            Logger.Log("Client: Send map request to server. Map status: " + isMapRecved);
        }

        private void SendCustomUnitsRequest()
        {
            SentPacket mapPacket = new SentPacket(PacketsHeaders.CustomUnitsClientRequest);

            mapPacket.stringBuilder.Append(JsonSerializer.Serialize(Client.Resources.PlayerCustomUnits));

            Client.TCPClient.SendToServer(mapPacket);

            Logger.Log("Send custom units request");
        }

        // TCP Handler

        private void HandleWaitingStateServerResponse(ReceivedPacket packet)
        {
            string received = packet.GetContent();
            WaitingStateServerResponse request = packet.GetDeserializedClassOrDefault<WaitingStateServerResponse>();
            if(request==null)
            {
                SendInfoPacket();
                return;
            }

                switch (request.Message)
                {
                    case WaitingMessage.ServerOk:
                        {
                            playersNicknames = request.PlayerNicknames;
                            playersReadyStatus = request.PlayerReadyStatus;
                            selectedMapId = request.PlayerSelectedMap;
                            state = State.Update;
                            Client.Resources.PlayerGameId = request.PlayerGameId;
                        }
                        break;
                    case WaitingMessage.ServerRefuse:
                        {
                            Logger.Log("Stop TCP");
                            Client.TCPClient.Stop();
                            Logger.Log("Server refused you");
                            GameStateManager.GameState = new MainState("Server refused you!");
                        }
                        break;
                    case WaitingMessage.ServerFull:
                        {
                            Logger.Log("Server is full");
                            GameStateManager.GameState = new MainState("Server is full!");
                            Logger.Log("Stop TCP");
                            Client.TCPClient.Stop();
                        }
                        break;
                    case WaitingMessage.ServerInGame:
                        {
                            playersNicknames = request.PlayerNicknames;
                            playersReadyStatus = request.PlayerReadyStatus;
                            state = State.StartGame;
                        }
                        break;
                    case WaitingMessage.ServerChangeNick:
                        {
                            Logger.Log("Change your nickname");
                            GameStateManager.GameState = new MainState("There is already a player with that nickname!");
                            Logger.Log("Stop TCP");
                            Client.TCPClient.Stop();
                        }
                        break;
                }
        }

        private void HandleMapServerResponse(ReceivedPacket packet)
        {
            Map[] request = packet.GetDeserializedClassOrDefault<Map[]>();
            if(request ==null)
            {
                SendMapRequest(false);
                return;
            }

            ServerMaps = request;
            MapPreviews = new Texture[request.Length];
            for(int i=0;i<ServerMaps.Length;i++)
            {
                mapPicker.AddItem(ServerMaps[i].MapName, i.ToString());
                MapPreviews[i] = ServerMaps[i].GetPreview(false);
            }
            mapPicker.SetSelectedItemById(0.ToString());
            SendMapRequest(true);
        }

        private void HandleCustomUnitsServerResponse(ReceivedPacket packet)
        {
            CustomUnitsServerResponse request = packet.GetDeserializedClassOrDefault<CustomUnitsServerResponse>();
            if (request == null)
            {
                SendCustomUnitsRequest();
                return;
            }

            if (!request.IsUnitsReceived) SendCustomUnitsRequest();
        }

        private void HandleStartGameServerRequest(ReceivedPacket packet)
        {
            string received = packet.GetContent();

            StartGameServerRequest request = packet.GetDeserializedClassOrDefault<StartGameServerRequest>();
            if (request == null) return;

            if (request.StartGame)
            {
                Client.Resources.Map = ServerMaps[request.MapID];
                for(int i=0;i<MaxPlayerCount;i++)
                {
                    Client.Resources.GameCustomUnits[i] = request.CustomUnits[i];
                }

                GameStateManager.GameState = new MatchGameState();
            }
        }
    }
}
