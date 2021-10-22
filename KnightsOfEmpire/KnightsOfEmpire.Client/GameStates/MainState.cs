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

namespace KnightsOfEmpire.GameStates
{
    class MainState : GameState
    {
        // Main view
        private Panel mainPanel;
        private Label messageLabel;

        // Settings view
        private Panel settingsPanel;

        // Private state to control GameState
        private enum State { Main, Settings, AfterUnits }
        private State state = State.Main;

        /// <summary>
        /// Initialize GUI for main menu
        /// </summary>
        public override void Initialize()
        {
            InitializeMainPanel();
            mainPanel.Visible = true;
            Client.Gui.Add(mainPanel);
        }

        public override void Update()
        {
            switch (state)
            {
                case State.Main:
                    mainPanel.Visible = true;
                    break;
                case State.Settings:
                    break;
                case State.AfterUnits:
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




        private void StartButton(object sender, EventArgs e)
        {
            GameStateManager.GameState = new ConnectState();
        }

        private void UnitsButton(object sender, EventArgs e)
        {

        }

        private void SettingsButton(object sender, EventArgs e)
        {

        }

        private void ExitButton(object sender, EventArgs e)
        {
            if (Client.TCPClient != null)
            {
                Client.TCPClient.Stop();
            }
            Client.RenderWindow.Close();
        }

        private void InitializeMainPanel()
        {
            Vector2u windowSize = Client.RenderWindow.Size;

            mainPanel = new Panel();
            mainPanel.Position = new Vector2f(0, 0);
            mainPanel.Size = ((Vector2f)windowSize);

            Label label = new Label();
            label.Text = "Knights of Empire";
            label.Position = new Vector2f(0, 190);
            label.Size = new Vector2f(1280, 150);
            label.TextSize = 100;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.Add(label);

            label = new Label();
            label.Text = "Menu";
            label.Position = new Vector2f(0, 350);
            label.Size = new Vector2f(1280, 30);
            label.TextSize = 22;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            mainPanel.Add(label);

            Button button = new Button();
            button.Text = "Start";
            button.Position = new Vector2f(490, 390);
            button.Size = new Vector2f(300, 44);
            button.TextSize = 24;
            button.Clicked += StartButton;
            mainPanel.Add(button);

            button = new Button();
            button.Text = "Units Manager";
            button.Position = new Vector2f(490, 445);
            button.Size = new Vector2f(300, 44);
            button.TextSize = 24;
            button.Clicked += UnitsButton;
            mainPanel.Add(button);

            button = new Button();
            button.Text = "Game Settings";
            button.Position = new Vector2f(490, 500);
            button.Size = new Vector2f(300, 44);
            button.TextSize = 24;
            button.Clicked += SettingsButton;
            mainPanel.Add(button);

            button = new Button();
            button.Text = "Exit";
            button.Position = new Vector2f(490, 555);
            button.Size = new Vector2f(300, 44);
            button.TextSize = 24;
            button.Clicked += ExitButton;
            mainPanel.Add(button);

            messageLabel = new Label();
            messageLabel.Text = "Error message";
            messageLabel.Position = new Vector2f(0, 625);
            messageLabel.Size = new Vector2f(1280, 30);
            messageLabel.TextSize = 18;
            messageLabel.HorizontalAlignment = HorizontalAlignment.Center;
            messageLabel.Visible = false;
            mainPanel.Add(messageLabel);
        }

        private void InitializeSettingsPanel()
        {
            Vector2u windowSize = Client.RenderWindow.Size;

            settingsPanel = new Panel();
            settingsPanel.Position = new Vector2f(0, 0);
            settingsPanel.Size = ((Vector2f)windowSize);

            Label label = new Label();
            label.Text = "Knights of Empire";
            label.Position = new Vector2f(0, 190);
            label.Size = new Vector2f(1280, 150);
            label.TextSize = 100;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            settingsPanel.Add(label);

            label = new Label();
            label.Text = "Menu";
            label.Position = new Vector2f(0, 370);
            label.Size = new Vector2f(1280, 30);
            label.TextSize = 22;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            settingsPanel.Add(label);

            

            messageLabel = new Label();
            messageLabel.Text = "Error message";
            messageLabel.Position = new Vector2f(0, 645);
            messageLabel.Size = new Vector2f(1280, 30);
            messageLabel.TextSize = 18;
            messageLabel.HorizontalAlignment = HorizontalAlignment.Center;
            messageLabel.Visible = false;
            settingsPanel.Add(messageLabel);
        }

    }
}
