using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using TGUI;

using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Networking.TCP;

namespace KnightsOfEmpire.GameStates
{
    class WaitingState : GameState
    {
        // Panel for wait state
        private Panel waitingPanel;
        private ScrollablePanel playerListPanel;
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
            //readyButton.Clicked += ;
            waitingPanel.Add(readyButton);

            notReadyButton = new Button();
            notReadyButton.Text = "Not ready";
            notReadyButton.Position = new Vector2f(885, 520);
            notReadyButton.Size = new Vector2f(150, 40);
            notReadyButton.TextSize = 18;
            notReadyButton.Enabled = false;
            //button.Clicked += ;
            waitingPanel.Add(notReadyButton);

            Button button = new Button();
            button.Text = "Disconnect";
            button.Position = new Vector2f(1095, 650);
            button.Size = new Vector2f(150, 40);
            button.TextSize = 18;
            //button.Clicked += ;
            waitingPanel.Add(button);

        }
    }
}
