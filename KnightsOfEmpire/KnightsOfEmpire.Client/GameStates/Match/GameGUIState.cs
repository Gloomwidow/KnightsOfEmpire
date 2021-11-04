using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

using SFML.Window;
using SFML.Graphics;
using SFML.System;

using TGUI;

using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Networking.TCP;
using KnightsOfEmpire.Common.Networking.UDP;
using KnightsOfEmpire.Common.Resources;
using KnightsOfEmpire.Common.Resources.Waiting;

namespace KnightsOfEmpire.GameStates
{
    public class GameGUIState : GameState
    {
        //Main panel for manage units and buldings
        private Panel mainPanel;

        private Panel mapPanel;
        private Panel statsPanel;
        private Panel buldingsPanel;
        private Panel unitsPanel;

        private Picture mapPicture;

        private Label goldLabel;
        private Label mainBaseLabel;
        private Label unitsLabel;
        private Label playerLabel;

        private BitmapButton[,] buldingsButtons;

        private BitmapButton[,] unitsButtons;

        private Label infoLabel;

        private Color backColor = new Color(247, 247, 247);

        // Info Label elements
        bool isOnButton = false;
        private Vector2i mouseLastPosition = new Vector2i(0, 0);
        private float timeOnButton = 0f;
        private const float timeToShowInfo = 0.5f;
        (int, int) buttonPosition;

        /// <summary>
        /// Initialize Game State
        /// </summary>
        public override void Initialize()
        {
            InitializeGamePanel();
            mainPanel.Visible = true;
            Client.Gui.Add(mainPanel);

            InitializeInfoLabel();
            infoLabel.Visible = false;
            Client.Gui.Add(infoLabel);
        }

        public int MainPanelHeight
        {
            get
            {
                return (int)mainPanel.InnerSize.Y;
            }
        }


        /// <summary>
        /// Uppdate state
        /// </summary>
        public override void Update()
        {
            //Info Label
            if(isOnButton && !infoLabel.Visible)
            {
                if(mouseLastPosition == Mouse.GetPosition(Client.RenderWindow))
                {
                    timeOnButton += Client.DeltaTime;
                    if (timeOnButton >= timeToShowInfo)
                    {
                        infoLabel.Position = (Vector2f)Mouse.GetPosition(Client.RenderWindow) + new Vector2f(0, 18);
                        infoLabel.Visible = true;
                    }
                }
                mouseLastPosition = Mouse.GetPosition(Client.RenderWindow);
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



        private void BuldingButtonClick(object sender, EventArgs e)
        {
            // TODO: Add functionality
        }

        private void UnitButtonClick(object sender, EventArgs e)
        {
            // TODO: Add functionality
        }

        private void ButtonMouseEnter(object sender, EventArgs e)
        {
            isOnButton = true;
            buttonPosition = ((int, int))((BitmapButton)sender).UserData;
        }

        private void ButtonMouseLeave(object sender, EventArgs e)
        {
            isOnButton = false;
            infoLabel.Visible = false;
            timeOnButton = 0;
        }

        private void MenuButtonClick(object sender, EventArgs e)
        {
            // TODO: Add functionality
        }

        private void LeaveButtonClick(object sender, EventArgs e)
        {
            // TODO: Add functionality
        }


        private void InitializeGamePanel()
        {
            //Main Panel
            mainPanel = new Panel();
            mainPanel.Position = new Vector2f(0, 520);
            mainPanel.Size = new Vector2f(1280, 200);

            //Map Panel
            mapPanel = new Panel();
            mapPanel.Renderer.BackgroundColor = backColor;
            mapPanel.Position = new Vector2f(20, 20);
            mapPanel.Size = new Vector2f(160, 160);
            mainPanel.Add(mapPanel);

            mapPicture = new Picture();
            mapPicture.Position = new Vector2f(0, 0);
            mapPicture.Size = new Vector2f(160, 160);
            mapPanel.Add(mapPicture);

            //Stats Panel
            statsPanel = new Panel();
            statsPanel.Renderer.BackgroundColor = backColor;
            statsPanel.Position = new Vector2f(200, 20);
            statsPanel.Size = new Vector2f(230, 160);
            mainPanel.Add(statsPanel);

            // Labels
            {
                Label label = new Label();
                label.Position = new Vector2f(15, 10);
                label.Size = new Vector2f(100, 35);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.TextSize = 16;
                label.Text = "Gold";
                statsPanel.Add(label);

                label = new Label();
                label.Position = new Vector2f(15, 45);
                label.Size = new Vector2f(100, 35);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.TextSize = 16;
                label.Text = "Main Base";
                statsPanel.Add(label);

                label = new Label();
                label.Position = new Vector2f(15, 80);
                label.Size = new Vector2f(100, 35);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.TextSize = 16;
                label.Text = "Units";
                statsPanel.Add(label);

                label = new Label();
                label.Position = new Vector2f(15, 115);
                label.Size = new Vector2f(100, 35);
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.TextSize = 16;
                label.Text = "Players";
                statsPanel.Add(label);
            }

            // Labels values
            {
                Label label = new Label();
                label.Position = new Vector2f(115, 10);
                label.Size = new Vector2f(100, 35);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.TextSize = 16;
                label.Text = "100";
                statsPanel.Add(label);
                goldLabel = label;

                label = new Label();
                label.Position = new Vector2f(115, 45);
                label.Size = new Vector2f(100, 35);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.TextSize = 16;
                label.Text = "387 / 500";
                statsPanel.Add(label);
                mainBaseLabel = label;

                label = new Label();
                label.Position = new Vector2f(115, 80);
                label.Size = new Vector2f(100, 35);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.TextSize = 16;
                label.Text = "32 / 40";
                statsPanel.Add(label);
                unitsLabel = label;

                label = new Label();
                label.Position = new Vector2f(115, 115);
                label.Size = new Vector2f(100, 35);
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.TextSize = 16;
                label.Text = "3";
                statsPanel.Add(label);
                playerLabel = label;
            }


            //Bulding Panel
            buldingsPanel = new Panel();
            buldingsPanel.Renderer.BackgroundColor = backColor;
            buldingsPanel.Position = new Vector2f(450, 20);
            buldingsPanel.Size = new Vector2f(260, 160);
            mainPanel.Add(buldingsPanel);

            buldingsButtons = new BitmapButton[5, 3];
            for(int j = 0; j < 3; j++)
                for(int i = 0; i < 5; i++)
                {
                    BitmapButton bitbutton = new BitmapButton();
                    bitbutton.Position = new Vector2f(10 + i * 50, 10 + j * 50);
                    bitbutton.Size = new Vector2f(40, 40);
                    bitbutton.UserData = (i, j);
                    bitbutton.Focusable = true;
                    bitbutton.Clicked += BuldingButtonClick;
                    bitbutton.MouseEntered += ButtonMouseEnter;
                    bitbutton.MouseLeft += ButtonMouseLeave;
                    
                    buldingsPanel.Add(bitbutton);
                }

            //Units Panel
            unitsPanel = new Panel();
            unitsPanel.Renderer.BackgroundColor = backColor;
            unitsPanel.Position = new Vector2f(730, 20);
            unitsPanel.Size = new Vector2f(360, 160);
            mainPanel.Add(unitsPanel);

            unitsButtons = new BitmapButton[7, 3];
            for (int j = 0; j < 3; j++)
                for (int i = 0; i < 7; i++)
                {
                    BitmapButton bitbutton = new BitmapButton();
                    bitbutton.Position = new Vector2f(10 + i * 50, 10 + j * 50);
                    bitbutton.Size = new Vector2f(40, 40);
                    bitbutton.UserData = (i, j);
                    bitbutton.Focusable = true;
                    bitbutton.Clicked += UnitButtonClick;
                    bitbutton.MouseEntered += ButtonMouseEnter;
                    bitbutton.MouseLeft += ButtonMouseLeave;

                    unitsPanel.Add(bitbutton);
                }

            //Buttons
            Button button = new Button();
            button.Position = new Vector2f(1110, 20);
            button.Size = new Vector2f(150, 32.5f);
            button.TextSize = 16;
            button.Text = "Menu";
            button.Clicked += MenuButtonClick;
            mainPanel.Add(button);

            button = new Button();
            button.Position = new Vector2f(1110, 62.5f);
            button.Size = new Vector2f(150, 32.5f);
            button.TextSize = 16;
            button.Text = "";
            button.Enabled = false;
            mainPanel.Add(button);

            button = new Button();
            button.Position = new Vector2f(1110, 105);
            button.Size = new Vector2f(150, 32.5f);
            button.TextSize = 16;
            button.Text = "";
            button.Enabled = false;
            mainPanel.Add(button);

            button = new Button();
            button.Position = new Vector2f(1110, 147.5f);
            button.Size = new Vector2f(150, 32.5f);
            button.TextSize = 16;
            button.Text = "Leave game";
            button.Clicked += LeaveButtonClick;
            mainPanel.Add(button);
        }

        private void InitializeInfoLabel()
        {
            infoLabel = new Label();
            infoLabel.Size = new Vector2f(100, 20);
            infoLabel.Renderer.BackgroundColor = Color.White;
            infoLabel.Renderer.BorderColor = Color.Black;
            infoLabel.Renderer.Borders = new Outline(1f);
            infoLabel.VerticalAlignmentAlignment = VerticalAlignment.Center;
            infoLabel.TextSize = 14;
            infoLabel.Text = "Info: Abc";

        }
    }
}
