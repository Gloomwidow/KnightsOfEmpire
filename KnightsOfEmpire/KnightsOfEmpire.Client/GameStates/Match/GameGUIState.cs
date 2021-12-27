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
using KnightsOfEmpire.Common.Resources.Player;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.GameStates.Match;
using KnightsOfEmpire.Common.Helper;

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

        private string gold = "gold value";
        private string capacity = "currect/max";
        private string mainBaseHealth = "2137/2137";

        protected Texture BuildingAtlas;
        public int BuildingAtlasSizeX, BuildingAtlasSizeY;


        private BitmapButton[,] buildingsButtons;

        private Texture[,] BuildingButttonTexture;

        private BitmapButton[,] unitsButtons;

        private Label infoLabel;

        private Color backColor = new Color(247, 247, 247);

        // Info Label elements
        bool isOnButton = false;
        private Vector2i mouseLastPosition = new Vector2i(0, 0);
        private float timeOnButton = 0f;
        private const float timeToShowInfo = 0.5f;
        (char buttonType, int id) buttonData;

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

        public override void HandleTCPPacket(ReceivedPacket packet)
        {
            switch (packet.GetHeader())
            {
                case PacketsHeaders.ChangePlayerInfoRequest:
                    ChangePlayerInfo(packet);
                    break;
            }
        }

        public override void LoadDependencies()
        {
            base.LoadDependencies();
            BuildingAtlas = Parent.GetSiblingGameState<BuildingUpdateState>().BuildingsAtlas;
            BuildingAtlasSizeX = (int)BuildingAtlas.Size.X;
            BuildingAtlasSizeY = (int)BuildingAtlas.Size.Y;
            int id = 0;
            Image imageBuilding = BuildingAtlas.CopyToImage();
            for (int i = 0; i < buildingsButtons.GetLength(1); i++)
            {
                for (int j = 0; j < buildingsButtons.GetLength(0); j++)
                {
                    BuildingInfo b = BuildingManager.GetNextBuilding(id);
                    if (b != null)
                    {
                        IntRect TextureRect = IdToTextureRect.GetRect(
                            b.Building.TextureId,
                            BuildingAtlasSizeX, BuildingAtlasSizeY);
                        buildingsButtons[j, i].Renderer.Texture = new Texture(imageBuilding, TextureRect);
                    }
                    else break;
                    id++;
                }
            }
            imageBuilding.Dispose();
        }

        public void ChangePlayerInfo(ReceivedPacket packet) 
        {
            ChangePlayerInfoRequest request = packet.GetDeserializedClassOrDefault<ChangePlayerInfoRequest>();
            if (request == null) return;
            Client.Resources.GoldAmount = request.GoldAmount;
            Client.Resources.MaxUnitCapacity = request.MaxUnitsCapacity;
            Client.Resources.UnitCapacity = request.CurrentUnitsCapacity;
            gold = request.GoldAmount.ToString();
            capacity = request.CurrentUnitsCapacity.ToString() + '/' + request.MaxUnitsCapacity.ToString();  
        }

        /// <summary>
        /// Uppdate state
        /// </summary>
        public override void Update()
        {
            goldLabel.Text = gold;
            unitsLabel.Text = capacity;
            //Info Label
            if (isOnButton && !infoLabel.Visible && infoLabel.Text!=string.Empty)
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
            buttonData = ((char, int))(((BitmapButton)sender).UserData);
            if (buttonData.buttonType == 'b')
            {
                int buildId = BuildingManager.GetNextBuildingId(buttonData.id);
                Parent.GetSiblingGameState<BuildingUpdateState>().
                    GetSiblingGameState<BuildingPlacementState>().BuildingIdToPlace = buildId;
            }
        }

        private void UnitButtonClick(object sender, EventArgs e)
        {
            // TODO: Add functionality
        }

        private void ButtonMouseEnter(object sender, EventArgs e)
        {
            isOnButton = true;
            buttonData = ((char, int))(((BitmapButton)sender).UserData);
            if(buttonData.buttonType=='b')
            {
                BuildingInfo info = BuildingManager.GetNextBuilding(buttonData.id);
                if (info != null)
                {
                    infoLabel.Text = info.Name + " (Cost: "+ info.Building.BuildCost.ToString() +")"+ "\n" + info.Description;
                }
                else infoLabel.Text = string.Empty;
            }
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
            int id = 0;
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
                label.Text = gold;
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
                label.Text = capacity;
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

            buildingsButtons = new BitmapButton[5, 3];
            for(int j = 0; j < 3; j++)
                for(int i = 0; i < 5; i++)
                {
                    buildingsButtons[i,j] = new BitmapButton();
                    buildingsButtons[i, j].Position = new Vector2f(10 + i * 50, 10 + j * 50);
                    buildingsButtons[i, j].Size = new Vector2f(40, 40);
                    buildingsButtons[i, j].UserData = ('b', id++);
                    buildingsButtons[i, j].Focusable = true;
                    buildingsButtons[i, j].Clicked += BuldingButtonClick;
                    buildingsButtons[i, j].MouseEntered += ButtonMouseEnter;
                    buildingsButtons[i, j].MouseLeft += ButtonMouseLeave;
                    buldingsPanel.Add(buildingsButtons[i, j]);
                }

            //Units Panel
            unitsPanel = new Panel();
            unitsPanel.Renderer.BackgroundColor = backColor;
            unitsPanel.Position = new Vector2f(730, 20);
            unitsPanel.Size = new Vector2f(360, 160);
            mainPanel.Add(unitsPanel);
            id = 0;
            unitsButtons = new BitmapButton[7, 3];
            for (int j = 0; j < 3; j++)
                for (int i = 0; i < 7; i++)
                {
                    BitmapButton bitbutton = new BitmapButton();
                    bitbutton.Position = new Vector2f(10 + i * 50, 10 + j * 50);
                    bitbutton.Size = new Vector2f(40, 40);
                    bitbutton.UserData = ('u', id++);
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
            infoLabel.Size = new Vector2f(500, 60);
            infoLabel.Renderer.BackgroundColor = Color.White;
            infoLabel.Renderer.BorderColor = Color.Black;
            infoLabel.Renderer.Borders = new Outline(1f);
            infoLabel.VerticalAlignmentAlignment = VerticalAlignment.Center;
            infoLabel.TextSize = 14;
            infoLabel.Text = "Info: Abc";

        }
    }
}
