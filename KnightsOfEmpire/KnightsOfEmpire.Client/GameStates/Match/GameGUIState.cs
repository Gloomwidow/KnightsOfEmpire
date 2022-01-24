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
using KnightsOfEmpire.GameStates.Match.Buildings;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Common.Units.Modifications;
using KnightsOfEmpire.Common.Resources.Units;

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
        private Panel buttonsPanel;

        private Picture mapPicture;

        private Label goldLabel;
        private Label mainBaseLabel;
        private Label unitsLabel;
        private Label playerLabel;

        private Label playerFlagLeft;
        private Label playerFlagRight;

        private string gold = "gold value";
        private string capacity = "currect/max";
        private string mainBaseHealth = "2137/2137";
        private string playersLeft = "players left";

        protected Texture BuildingAtlas;
        public int BuildingAtlasSizeX, BuildingAtlasSizeY;

        private BitmapButton[,] buildingsButtons;

        private Texture[,] BuildingButttonTexture;

        private Texture PlayerFlagTexture;

        private BitmapButton[,] unitsButtons;

        protected Texture[] UnitAtlas;
        public int UnitAtlasSizeX, UnitAtlasSizeY;

        private Label infoLabel;

        private Color backColor = new Color(247, 247, 247);

        // Info Label elements
        bool isOnButton = false;
        private Vector2i mouseLastPosition = new Vector2i(0, 0);
        private float timeOnButton = 0f;
        private const float timeToShowInfo = 0.5f;
        (char buttonType, int id) buttonData;

        private int originalResolutionX = 1280;
        private int differenceResolutionX = 0;

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

        public override void LoadResources()
        {
            base.LoadResources();
            PlayerFlagTexture = new Texture(@"./Assets/Textures/flag.png");
        }

        public override void LoadDependencies()
        {
            base.LoadDependencies();
            UnitAtlas = Parent.GetSiblingGameState<UnitUpdateState>().UnitsAtlases;
            UnitAtlasSizeX = (int)UnitAtlas[0].Size.X;
            UnitAtlasSizeY = (int)UnitAtlas[0].Size.Y;
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

        public void SetUnitButtons()
        {
            ResetUnitButtons();
            Building SelectedBuilding = GameStateManager.GameState.GetSiblingGameState<BuildingUpdateState>().GetSiblingGameState<BuildingSelectionState>().SelectedBuilding;
            if (SelectedBuilding.TrainType == -1) return;
            Image imageUnit = UnitAtlas[SelectedBuilding.TrainType].CopyToImage();
            int id = 0;
            for (int i = 0; i < unitsButtons.GetLength(1); i++)
            {
                for (int j = 0; j < unitsButtons.GetLength(0); j++)
                {
                    if (id >= Client.Resources.PlayerCustomUnits.Units.Length) break;
                    Unit u = UnitUpgradeManager.ProduceUnit(Client.Resources.PlayerCustomUnits.Units[id]);
                    if ((int)Client.Resources.PlayerCustomUnits.Units[id].UnitType == SelectedBuilding.TrainType)
                    {
                        IntRect TextureRect = IdToTextureRect.GetRect(
                            u.TextureId,
                            UnitAtlasSizeX, UnitAtlasSizeY);
                        unitsButtons[j, i].Renderer.Texture = new Texture(imageUnit, TextureRect);
                    }
                    else
                    {
                        j--;
                    };
                    id++;
                    if (id >= Constants.MaxUnitsPerPlayer)
                    {
                        imageUnit.Dispose();
                        return;
                    }
                }
            }

        }

        public void ResetUnitButtons()
        {
            for (int i = 0; i < unitsButtons.GetLength(1); i++)
            {
                for (int j = 0; j < unitsButtons.GetLength(0); j++)
                {
                    unitsButtons[j, i].Renderer.Texture = new Texture(1,1);
                }
            }
        }

        public void ChangePlayerInfo(ReceivedPacket packet)
        {
            ChangePlayerInfoRequest request = packet.GetDeserializedClassOrDefault<ChangePlayerInfoRequest>();
            if (request == null) return;
            Client.Resources.GoldAmount = request.GoldAmount;
            Client.Resources.MaxUnitCapacity = request.MaxUnitsCapacity;
            Client.Resources.UnitCapacity = request.CurrentUnitsCapacity;
            Client.Resources.PlayersLeft = request.PlayersLeft;
            if (request.IsDefeated)
            {
                GameStateManager.GameState = new EndMatchGameState("You have been defeated");
                return;
            }
            if (request.PlayersLeft == 1)
            {
                GameStateManager.GameState = new EndMatchGameState("You won");
                return;
            }
            gold = request.GoldAmount.ToString();
            capacity = request.CurrentUnitsCapacity.ToString() + '/' + request.MaxUnitsCapacity.ToString();
            playersLeft = request.PlayersLeft.ToString();
        }

        /// <summary>
        /// Uppdate state
        /// </summary>
        public override void Update()
        {
            goldLabel.Text = gold;
            unitsLabel.Text = capacity;
            playerLabel.Text = playersLeft;
            BuildingUpdateState buildingState = parent.GetSiblingGameState<BuildingUpdateState>();
            mainBaseLabel.Text = buildingState.GetMainBuildingHealthText();
            //Info Label
            if (isOnButton && !infoLabel.Visible && infoLabel.Text != string.Empty)
            {
                if (mouseLastPosition == Mouse.GetPosition(Client.RenderWindow))
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
            if(buildingState.HasBuildingsChanged)
            {
                Image map = Client.Resources.Map.GetPreview();
                Image builds = buildingState.BuildingsMiniMap;
                for(uint x=0;x<map.Size.X;x++)
                {
                    for (uint y = 0; y < map.Size.Y; y++)
                    {
                        Color b = builds.GetPixel(x, y);
                        if (b.A!=0)
                        {
                            map.SetPixel(x, y, b);
                        }
                    }
                }
                mapPanel.Renderer.TextureBackground = new Texture(map);
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
            Building SelectedBuilding = GameStateManager.GameState.GetSiblingGameState<BuildingUpdateState>().GetSiblingGameState<BuildingSelectionState>().SelectedBuilding;
            if (SelectedBuilding == null || SelectedBuilding.TrainType == -1) return;
            buttonData = ((char, int))(((BitmapButton)sender).UserData);
            int counter = 0;
            int id = 0;
            while(counter < Client.Resources.PlayerCustomUnits.Units.Length) 
            {
                if ((int)Client.Resources.PlayerCustomUnits.Units[counter].UnitType == SelectedBuilding.TrainType)
                {
                    if(buttonData.id == id) 
                    {
                        Unit u = UnitUpgradeManager.ProduceUnit(Client.Resources.PlayerCustomUnits.Units[counter]);
                        TrainUnitRequest request = new TrainUnitRequest
                        {
                            UnitTypeId = counter,
                            BuildingPosX = SelectedBuilding.Position.X,
                            BuildingPosY = SelectedBuilding.Position.Y,
                        };

                        SentPacket packet = new SentPacket(PacketsHeaders.GameUnitTrainRequest);
                        packet.stringBuilder.Append(JsonSerializer.Serialize(request));
                        Client.TCPClient.SendToServer(packet);
                    }
                    id++;
                }
                counter++;
            }
        }

        private void ButtonMouseEnter(object sender, EventArgs e)
        {
            isOnButton = true;
            buttonData = ((char, int))(((BitmapButton)sender).UserData);
            if (buttonData.buttonType == 'b')
            {
                BuildingInfo info = BuildingManager.GetNextBuilding(buttonData.id);
                if (info != null)
                {
                    infoLabel.Text = info.Name + "\n" + "(Cost: " + info.Building.BuildCost.ToString() + ")" + "\n" + info.Description;
                    return;
                }
                
            }
            else if(buttonData.buttonType == 'u') 
            {
                Building SelectedBuilding = GameStateManager.GameState.GetSiblingGameState<BuildingUpdateState>().GetSiblingGameState<BuildingSelectionState>().SelectedBuilding;
                if (SelectedBuilding == null || SelectedBuilding.TrainType == -1) return;
                int counter = 0;
                int id = 0;
                while (counter < Client.Resources.PlayerCustomUnits.Units.Length)
                {
                    if ((int)Client.Resources.PlayerCustomUnits.Units[counter].UnitType == SelectedBuilding.TrainType)
                    {
                        if (buttonData.id == id)
                        {
                            Unit info = UnitUpgradeManager.ProduceUnit(Client.Resources.PlayerCustomUnits.Units[counter]);
                            infoLabel.Text = "Cost: " + info.Stats.TrainCost.ToString() + "\n" 
                                + "Health: " + info.Stats.MaxHealth.ToString() + "\n" 
                                + "Attack: " + info.Stats.AttackDamage.ToString() + "\n";
                            return;
                        }
                        id++;
                    }
                    counter++;
                }
            }
            infoLabel.Text = string.Empty;
        }

        private void ButtonMouseLeave(object sender, EventArgs e)
        {
            isOnButton = false;
            infoLabel.Visible = false;
            timeOnButton = 0;
        }

        private void LeaveButtonClick(object sender, EventArgs e)
        {
            Client.TCPClient.Stop();
            Client.UDPClient.Stop();
            GameStateManager.GameState = new MainState("You have left the game");
        }

        public void ResizePanel()
        {
            Vector2u windowSize = Client.RenderWindow.Size;
            int newDifferenceResolutionX = (int)(windowSize.X - originalResolutionX) / 2;
            mainPanel.Position = new Vector2f(0, windowSize.Y - 200);
            mainPanel.Size = new Vector2f(windowSize.X, 200);

            mapPanel.Position = new Vector2f(mapPanel.Position.X - differenceResolutionX + newDifferenceResolutionX, mapPanel.Position.Y);
            statsPanel.Position = new Vector2f(statsPanel.Position.X - differenceResolutionX + newDifferenceResolutionX, statsPanel.Position.Y);
            buttonsPanel.Position = new Vector2f(buttonsPanel.Position.X - differenceResolutionX + newDifferenceResolutionX, buttonsPanel.Position.Y);
            unitsPanel.Position = new Vector2f(unitsPanel.Position.X - differenceResolutionX + newDifferenceResolutionX, unitsPanel.Position.Y);
            buldingsPanel.Position = new Vector2f(buldingsPanel.Position.X - differenceResolutionX + newDifferenceResolutionX, buldingsPanel.Position.Y);

            playerFlagRight.Position = new Vector2f(windowSize.X - 20, playerFlagRight.Position.Y);

            differenceResolutionX = newDifferenceResolutionX;
        }


        private void InitializeGamePanel()
        {
            int id = 0;
            //Main Panel
            mainPanel = new Panel();
            mainPanel.Position = new Vector2f(0, 520);
            mainPanel.Size = new Vector2f(1280, 200);

            Image flagImage = PlayerFlagTexture.CopyToImage();
            Color playerColor = Constants.playerColors[Client.Resources.PlayerGameId];
            for (uint x = 0; x < flagImage.Size.X; x++)
            {
                for (uint y = 0; y < flagImage.Size.Y; y++)
                {
                    Color c = flagImage.GetPixel(x, y);
                    float mg = (c.R / 255.0f);
                    Color resultColor = new Color((byte)(mg * playerColor.R), (byte)(mg * playerColor.G), (byte)(mg * playerColor.B), c.A);
                    flagImage.SetPixel(x, y, resultColor);
                }
            }

            //Map Panel
            mapPanel = new Panel();
            mapPanel.Renderer.BackgroundColor = backColor;
            mapPanel.Renderer.TextureBackground = Client.Resources.Map.GetPreview(true);
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
                label.Text = playersLeft;
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
            for (int j = 0; j < 3; j++)
                for (int i = 0; i < 5; i++)
                {
                    buildingsButtons[i, j] = new BitmapButton();
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
                    unitsButtons[i, j] = new BitmapButton();
                    unitsButtons[i, j].Position = new Vector2f(10 + i * 50, 10 + j * 50);
                    unitsButtons[i, j].Size = new Vector2f(40, 40);
                    unitsButtons[i, j].UserData = ('u', id++);
                    unitsButtons[i, j].Focusable = true;
                    unitsButtons[i, j].Clicked += UnitButtonClick;
                    unitsButtons[i, j].MouseEntered += ButtonMouseEnter;
                    unitsButtons[i, j].MouseLeft += ButtonMouseLeave;

                    unitsPanel.Add(unitsButtons[i, j]);
                }

            buttonsPanel = new Panel();
            buttonsPanel.Renderer.BackgroundColor = backColor;
            buttonsPanel.Position = new Vector2f(1100, 20);
            buttonsPanel.Size = new Vector2f(200, 160);
            mainPanel.Add(buttonsPanel);

            //Buttons
            {
                Button button = new Button();
                button.Position = new Vector2f(0, 0);
                button.Size = new Vector2f(150, 32.5f);
                button.TextSize = 16;
                button.Text = "";
                button.Enabled = false;
                buttonsPanel.Add(button);

                button = new Button();
                button.Position = new Vector2f(0, 42.5f);
                button.Size = new Vector2f(150, 32.5f);
                button.TextSize = 16;
                button.Text = "";
                button.Enabled = false;
                buttonsPanel.Add(button);

                button = new Button();
                button.Position = new Vector2f(0, 85);
                button.Size = new Vector2f(150, 32.5f);
                button.TextSize = 16;
                button.Text = "";
                button.Enabled = false;
                buttonsPanel.Add(button);

                button = new Button();
                button.Position = new Vector2f(0, 127.5f);
                button.Size = new Vector2f(150, 32.5f);
                button.TextSize = 16;
                button.Text = "Leave game";
                button.Clicked += LeaveButtonClick;
                buttonsPanel.Add(button);
            }

            playerFlagLeft = new Label();
            playerFlagLeft.Position = new Vector2f(0, 0);
            playerFlagLeft.Size = new Vector2f(20, 200);
            playerFlagLeft.Renderer.TextureBackground = new Texture(flagImage);
            mainPanel.Add(playerFlagLeft);

            playerFlagRight = new Label();
            playerFlagRight.Position = new Vector2f(1260, 0);
            playerFlagRight.Size = new Vector2f(20, 200);
            playerFlagRight.Renderer.TextureBackground = new Texture(flagImage);
            mainPanel.Add(playerFlagRight);

        }

        private void InitializeInfoLabel()
        {
            infoLabel = new Label();
            infoLabel.Size = new Vector2f(300, 100);
            infoLabel.Renderer.BackgroundColor = Color.White;
            infoLabel.Renderer.BorderColor = Color.Black;
            infoLabel.Renderer.Borders = new Outline(1f);
            infoLabel.VerticalAlignmentAlignment = VerticalAlignment.Center;
            infoLabel.TextSize = 14;
            infoLabel.Text = "Info: Abc";

        }
    }
}
