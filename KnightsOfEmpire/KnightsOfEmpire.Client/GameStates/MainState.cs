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
        private string message = "";

        // Settings view
        private Panel settingsPanel;
        private Label errorNicknameLabel;
        private EditBox nicknameEditBox;

        private const string errorNicknameStr = "Incorrect Nickname";
        private const string emptyNicknameStr = "Empty Nickname";

        private const string settingsSaveStr = "Settings save correctly";
        private const string settingsUnsaveStr = "Settings unsave";

        // Private state to control GameState
        private enum State { Main, Settings, Message }
        private State state = State.Main;

        /// <summary>
        /// For create Main State without any message
        /// </summary>
        public MainState()
        {
            state = State.Main;
        }

        /// <summary>
        /// For create Main State with message
        /// </summary>
        /// <param name="message">Message to show in state</param>
        public MainState(string message)
        {
            state = State.Message;
            this.message = message;
        }

        /// <summary>
        /// Initialize GUI for main menu
        /// </summary>
        public override void Initialize()
        {
            InitializeMainPanel();
            mainPanel.Visible = true;
            Client.Gui.Add(mainPanel);

            InitializeSettingsPanel();
            settingsPanel.Visible = false;
            Client.Gui.Add(settingsPanel);
        }

        /// <summary>
        /// Update Main State
        /// </summary>
        public override void Update()
        {
            switch (state)
            {
                case State.Main:
                    mainPanel.Visible = true;
                    settingsPanel.Visible = false;
                    break;
                case State.Settings:
                    mainPanel.Visible = false;
                    settingsPanel.Visible = true;
                    break;
                case State.Message:
                    mainPanel.Visible = true;
                    settingsPanel.Visible = false;
                    messageLabel.Visible = true;
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
            // TODO: add units panel
        }

        private void SettingsButton(object sender, EventArgs e)
        {
            state = State.Settings;

            // Uppdate all settings
            nicknameEditBox.Text = Client.Resources.Nickname;

            // Clear all errors in settings panel
            errorNicknameLabel.Visible = false;
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
            mainPanel = new Panel();
            mainPanel.Position = new Vector2f(0, 0);
            mainPanel.Size = new Vector2f(1280, 720);

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
            messageLabel.Text = message;
            messageLabel.Position = new Vector2f(0, 625);
            messageLabel.Size = new Vector2f(1280, 30);
            messageLabel.TextSize = 18;
            messageLabel.HorizontalAlignment = HorizontalAlignment.Center;
            messageLabel.Visible = false;
            mainPanel.Add(messageLabel);
        }

        private void SettingsSave(object sender, EventArgs e)
        {
            if (NicknameValidate())
            {
                state = State.Main;

                Client.Resources.Nickname = nicknameEditBox.Text;

                messageLabel.Text = settingsSaveStr;
                messageLabel.Visible = true;
            }
        }

        private void SettingsBack(object sender, EventArgs e)
        {
            state = State.Main;

            if(nicknameEditBox.Text != Client.Resources.Nickname)
            {
                messageLabel.Text = settingsUnsaveStr;
                messageLabel.Visible = true;
            }
            else
            {
                messageLabel.Visible = false;
            }
        }

        private bool NicknameValidate()
        {
            if (nicknameEditBox.Text == "")
            {
                errorNicknameLabel.Text = emptyNicknameStr;
                errorNicknameLabel.Visible = true;
                return false;
            }
            if(nicknameEditBox.Text.Length < 3 || nicknameEditBox.Text.Length > 20)
            {
                errorNicknameLabel.Text = errorNicknameStr;
                errorNicknameLabel.Visible = true;
                return false;
            }
            return true;
        }

        private void InitializeSettingsPanel()
        {
            settingsPanel = new Panel();
            settingsPanel.Position = new Vector2f(0, 0);
            settingsPanel.Size = new Vector2f(1280, 720);

            Label label = new Label();
            label.Text = "Knights of Empire";
            label.Position = new Vector2f(0, 160);
            label.Size = new Vector2f(1280, 110);
            label.TextSize = 80;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            settingsPanel.Add(label);

            label = new Label();
            label.Text = "Settings";
            label.Position = new Vector2f(0, 260);
            label.Size = new Vector2f(1280, 38);
            label.TextSize = 20;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            settingsPanel.Add(label);

            label = new Label();
            label.Text = "Nickname";
            label.Position = new Vector2f(0, 340);
            label.Size = new Vector2f(1280, 32);
            label.TextSize = 24;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            settingsPanel.Add(label);

            nicknameEditBox = new EditBox();
            nicknameEditBox.Position = new Vector2f(490, 380);
            nicknameEditBox.Size = new Vector2f(300, 40);
            nicknameEditBox.DefaultText = "User";
            nicknameEditBox.Alignment = HorizontalAlignment.Center;
            nicknameEditBox.TextSize = 18;
            nicknameEditBox.InputValidator = "[-0-9a-zA-Z_]*";
            settingsPanel.Add(nicknameEditBox);


            errorNicknameLabel = new Label();
            errorNicknameLabel.Text = errorNicknameStr;
            errorNicknameLabel.Position = new Vector2f(0, 425);
            errorNicknameLabel.Size = new Vector2f(1280, 18);
            errorNicknameLabel.TextSize = 13;
            errorNicknameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            errorNicknameLabel.Renderer.TextColor = new Color(201, 52, 52);
            errorNicknameLabel.Visible = false;
            settingsPanel.Add(errorNicknameLabel);

            Button button = new Button();
            button.Position = new Vector2f(565, 470);
            button.Size = new Vector2f(150, 40);
            button.Text = "Save";
            button.TextSize = 18;
            button.Clicked += SettingsSave;
            settingsPanel.Add(button);

            button = new Button();
            button.Position = new Vector2f(1095, 650);
            button.Size = new Vector2f(150, 40);
            button.Text = "Back";
            button.TextSize = 18;
            button.Clicked += SettingsBack;
            settingsPanel.Add(button);
        }

    }
}
