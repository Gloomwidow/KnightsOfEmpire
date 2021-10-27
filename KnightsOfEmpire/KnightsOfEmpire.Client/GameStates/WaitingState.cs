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
using KnightsOfEmpire.Common.Networking.TCP;
using KnightsOfEmpire.Common.Resources;
using KnightsOfEmpire.Common.Resources.Waiting;

namespace KnightsOfEmpire.GameStates
{
    class WaitingState : GameState
    {
        // State to manage GameState
        private enum State { First, Main, Uppdate, StartGame}
        private State state = State.First;

        // Players list
        private bool clientReady = false;
        private string[] playersNicknames = null;
        private bool[] playersReadyStatus = null;

        // Panel for waiting to server first response
        private Panel connectPanel;

        // Panel for wait state
        private Panel waitingPanel;

        private ScrollablePanel playerListPanel;
        private const string NumLabelStr = "Num";
        private const string NicknameLabelStr = "Nick";
        private const string ReadyLabelStr = "Ready";

        private Button readyButton;
        private Button notReadyButton;

        /// <summary>
        /// Initialize GUI
        /// </summary>
        public override void Initialize()
        {
            InitializeWaitingPanel();
            waitingPanel.Visible = true;
            Client.Gui.Add(waitingPanel);

            InitializeConnectPanle();
            connectPanel.Visible = true;
            Client.Gui.Add(connectPanel);

            // Exsample of player list
            //int numPl = 10;
            //for(int i=0; i<numPl; i++)
            //{
            //    Panel panel = CreatePlayerPanel(i + 1, "Player" + (i + 1).ToString(), false, numPl);
            //    panel.Position = new Vector2f(10, i * 40);
            //    playerListPanel.Add(panel, "");
            //}
            //Group group = new Group();
            //group.Size = new Vector2f(10, 10);
            //group.Position = new Vector2f(10, numPl * 40 - 10);
            //playerListPanel.Add(group);
        }

        /// <summary>
        /// Handle TCP request from server
        /// </summary>
        /// <param name="packets">List of recived packets</param>
        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            foreach (ReceivedPacket packet in packets)
            {
                string received = packet.GetContent();
                if (received.StartsWith("2001 OK"))
                {
                    continue;
                }
                WaitingStateServerResponse request = null;
                try
                {
                    request = JsonSerializer.Deserialize<WaitingStateServerResponse>(received);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    SendInfoPacket();
                }

                if (request != null)
                {
                    switch (request.Message)
                    {
                        case WaitingMessage.ServerOk:
                            {
                                playersNicknames = request.PlayerNicknames;
                                playersReadyStatus = request.PlayerReadyStatus;
                                state = State.Uppdate;
                            }
                            break;
                        case WaitingMessage.ServerRefuse:
                            {
                                GameStateManager.GameState = new MainState("Server refuse you");
                            }
                            break;
                        case WaitingMessage.ServerFull:
                            {
                                GameStateManager.GameState = new MainState("Server was full");
                            }
                            break;
                        case WaitingMessage.ServerInGame:
                            {
                                playersNicknames = request.PlayerNicknames;
                                playersReadyStatus = request.PlayerReadyStatus;
                                state = State.StartGame;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Uppdate state
        /// </summary>
        public override void Update()
        {
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
                case State.Uppdate:
                    {
                        playerListPanel.RemoveAllWidgets();

                        int playerNumber = 0;
                        for (int i=0; i<playersNicknames.Length; i++)
                        {
                            if(playersNicknames[i] != string.Empty)
                            {
                                playerNumber++;

                                if(playersNicknames[i] == Client.Resources.Nickname)
                                {
                                    // 
                                }
                            }
                        }
                        int j = 0;
                        for (int i = 0; i < playersNicknames.Length; i++)
                        {
                            if (playersNicknames[i] != string.Empty)
                            {
                                Panel panel = CreatePlayerPanel(j + 1, playersNicknames[i], playersReadyStatus[i], playerNumber);
                                panel.Position = new Vector2f(10, j * 40);
                                playerListPanel.Add(panel, (j+1).ToString());
                                j++;
                            }
                        }
                        Group group = new Group();
                        group.Size = new Vector2f(10, 10);
                        group.Position = new Vector2f(10, playerNumber * 40 - 10);
                        playerListPanel.Add(group);
                        state = State.Main;
                    }
                    break;
                case State.StartGame:
                    {
                        // TODO: Load resource and start game
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

        private void SendInfoPacket()
        {
            SentPacket infoPacket = new SentPacket();

            WaitingStateClientRequest request = new WaitingStateClientRequest();
            request.IsReady = clientReady;
            request.Nickname = Client.Resources.Nickname;
            infoPacket.stringBuilder.Append(JsonSerializer.Serialize(request));

            Client.TCPClient.SendToServer(infoPacket);
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
            label.Position = new Vector2f(60, 250);
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

            Picture picture = new Picture();
            picture.Position = new Vector2f(110, 300);
            picture.Size = new Vector2f(340, 280);
            waitingPanel.Add(picture);

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
            label.Position = new Vector2f(80, 10);
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
            // TODO: Send to server that player is ready

            // TODO: Get player list from server
        }

        private void NotReadyButton(object sender, EventArgs e)
        {
            // TODO: Send to server that player is not ready

            // TODO: Get player list from server
        }

        private void DisconnectButton(object sender, EventArgs e)
        {
            if(Client.TCPClient != null)
            {
                Client.TCPClient.Stop();
            }
            GameStateManager.GameState = new MainState("Disconnect from server");
        }

        private Panel CreatePlayerPanel(int number, string nickname, bool isReady, int total = 4)
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
            panel.Add(label, NumLabelStr);

            label = new Label();
            label.Text = nickname;
            label.Position = new Vector2f(70, 0);
            label.Size = new Vector2f(320, 30);
            label.TextSize = 18;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label, NicknameLabelStr);

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
            panel.Add(label, ReadyLabelStr);

            return panel;
        }

        void InitializeConnectPanle()
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
    }
}
