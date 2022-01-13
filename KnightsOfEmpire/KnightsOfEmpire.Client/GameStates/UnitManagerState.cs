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

using KnightsOfEmpire.Common.Helper;

using KnightsOfEmpire.Common.Units.Enum;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Common.Units.Modifications;
using KnightsOfEmpire.Common.Units.Modifications.Archetypes;

namespace KnightsOfEmpire.GameStates
{
    class UnitManagerState : GameState
    {
        Panel mainPanel;

        Texture[][] UnitsAtlases;
        int maxTextureNumber = 6;
        Texture yes;
        Texture no;

        string soldiers = "Soldiers";
        string archers = "Archers";
        string riders = "Riders";
        string machines = "Machines";

        ScrollablePanel soldiersScroll;
        ScrollablePanel archersScroll;
        ScrollablePanel ridersScroll;
        ScrollablePanel machinesScroll;

        //Max Unit Panel
        Picture unitPicture;
        Picture upgrade0Picture;
        Picture upgrade1Picture;
        Picture upgrade2Picture;
        Picture upgrade3Picture;
        EditBox unitEditbox;

        Label healthLabel;
        Label priceLabel;
        Label movmentSpeedLabel;
        Label viewDistanceLabel;
        Label damageLabel;
        Label buldingMultiLabel;
        Label attackSpeedLabel;
        Label attackDistLabel;


        List<CustomUnit> customUnits;

        //SelectedUnit
        Panel selectedUnitPanel = null;

        //SelectedUpgradePanel
        Panel selectedUpgradePanel = null;

        public override void LoadResources()
        {
            int unitTypesCount = Enum.GetNames(typeof(UnitType)).Length;
            UnitsAtlases = new Texture[unitTypesCount][];

            UnitsAtlases[0] = new Texture[maxTextureNumber];
            for (int i = 0; i < maxTextureNumber; i++)
                UnitsAtlases[0][i] = new Texture(@"./Assets/Textures/heavy infantry.png", IdToTextureRect.GetRect(i, 64, 64));

            UnitsAtlases[1] = new Texture[maxTextureNumber];
            for (int i = 0; i < maxTextureNumber; i++)
                UnitsAtlases[1][i] = new Texture(@"./Assets/Textures/light infantry.png", IdToTextureRect.GetRect(i, 64, 64));

            UnitsAtlases[2] = new Texture[maxTextureNumber];
            for (int i = 0; i < maxTextureNumber; i++)
                UnitsAtlases[2][i] = new Texture(@"./Assets/Textures/cavalry - light.png", IdToTextureRect.GetRect(i, 64, 64));

            UnitsAtlases[3] = new Texture[maxTextureNumber];
            for (int i = 0; i < maxTextureNumber; i++)
                UnitsAtlases[3][i] = new Texture(@"./Assets/Textures/siege engines.png", IdToTextureRect.GetRect(i, 64, 64));

            yes = new Texture(@"./Assets/Textures/yesno.png", new IntRect(0, 0, 16, 16));
            no = new Texture(@"./Assets/Textures/yesno.png", new IntRect(16, 0, 16, 16));
        }

        public override void Initialize()
        {
            customUnits = UnitUpgradeManager.LoadCustomUnitsFromFile().Units.ToList();


            InitializeMainPanel();
            Client.Gui.Add(mainPanel);

            UpdateUnitsScrollPanel(soldiersScroll, UnitType.Melee);
            UpdateUnitsScrollPanel(archersScroll, UnitType.Ranged);
            UpdateUnitsScrollPanel(ridersScroll, UnitType.Cavalry);
            UpdateUnitsScrollPanel(machinesScroll, UnitType.Siege);
        }

        public override void Update()
        {
            
        }

        public override void Render()
        {

            Client.Gui.Draw();
        }

        public override void Dispose()
        {
            Client.Gui.RemoveAllWidgets();
        }

        void InitializeMainPanel()
        {
            mainPanel = new Panel();
            mainPanel.Position = new Vector2f(0, 0);
            mainPanel.Size = new Vector2f(1280, 720);
            mainPanel.Renderer.BackgroundColor = new Color(247, 247, 247);

            Label label = new Label();
            label.Text = "Units Manager";
            label.Position = new Vector2f(0, 10);
            label.Size = new Vector2f(1280, 60);
            label.TextSize = 40;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            mainPanel.Add(label);


            Panel panel = CreateUnitsPanel(soldiers);
            panel.Position = new Vector2f(30, 100);
            mainPanel.Add(panel);
            soldiersScroll =  (ScrollablePanel)panel.Get("Scroll" + soldiers);

            panel = CreateUnitsPanel(archers);
            panel.Position = new Vector2f(30, 250);
            mainPanel.Add(panel);
            archersScroll = (ScrollablePanel)panel.Get("Scroll" + archers);

            panel = CreateUnitsPanel(riders);
            panel.Position = new Vector2f(30, 400);
            mainPanel.Add(panel);
            ridersScroll = (ScrollablePanel)panel.Get("Scroll" + riders);

            panel = CreateUnitsPanel(machines);
            panel.Position = new Vector2f(30, 550);
            mainPanel.Add(panel);
            machinesScroll = (ScrollablePanel)panel.Get("Scroll" + machines);

            panel = CreateMaxUnitPanel();
            panel.Position = new Vector2f(670, 100);
            mainPanel.Add(panel);

            panel = CreateUpgradesPanel();
            panel.Position = new Vector2f(670, 310);
            mainPanel.Add(panel);

            Button button = new Button();
            button.Position = new Vector2f(790, 640);
            button.Size = new Vector2f(120, 40);
            button.Text = "Save";
            button.TextSize = 18;
            button.Clicked += ButtonSaveClick;
            mainPanel.Add(button);

            button = new Button();
            button.Position = new Vector2f(940, 640);
            button.Size = new Vector2f(120, 40);
            button.Text = "Exit";
            button.TextSize = 18;
            button.Clicked += ButtonExitClick;
            mainPanel.Add(button);
        }

        void ButtonSaveClick(object o, EventArgs e)
        {
            CustomUnits customUnitsSave = new CustomUnits
            {
                Units = customUnits.ToArray()
            };
            UnitUpgradeManager.SaveCustomUnitsToFile(customUnitsSave);

            Client.Resources.PlayerCustomUnits = customUnitsSave;

            GameStateManager.GameState = new MainState("Units saved");
        }

        void ButtonExitClick(object o, EventArgs e)
        {
            GameStateManager.GameState = new MainState("Warning: Units not saved");
        }

        void ButtonAddClick(object o, EventArgs e)
        {
            Button button = (Button)o;
            if (button.Name == "Add" + soldiers)
                AddCustomUnits(soldiersScroll, UnitType.Melee);
            if (button.Name == "Add" + archers)
                AddCustomUnits(archersScroll, UnitType.Ranged);
            if (button.Name == "Add" + riders)
                AddCustomUnits(ridersScroll, UnitType.Cavalry);
            if (button.Name == "Add" + machines)
                AddCustomUnits(machinesScroll, UnitType.Siege);

        }

        void ButtonRemoveClick(object o, EventArgs e)
        {
            Button button = (Button)o;
            if (button.Name == "Remove" + soldiers)
                RemoveCustomUnits(soldiersScroll, UnitType.Melee);
            if (button.Name == "Remove" + archers)
                RemoveCustomUnits(archersScroll, UnitType.Ranged);
            if (button.Name == "Remove" + riders)
                RemoveCustomUnits(ridersScroll, UnitType.Cavalry);
            if (button.Name == "Remove" + machines)
                RemoveCustomUnits(machinesScroll, UnitType.Siege);
        }

        void PanelUnitClick(object o, EventArgs e)
        {
            if(selectedUnitPanel != null)
            {
                selectedUnitPanel.Renderer.Borders = new Outline(0);
            }

            Panel panel = (Panel)o;
            selectedUnitPanel = panel;

            selectedUnitPanel.Renderer.Borders = new Outline(1);

            CustomUnit customUnit = (CustomUnit)panel.UserData;
            UpdateMaxUnitPanel(customUnit);
        }

        void PictureUnitClick(object o, EventArgs e)
        {
            CustomUnit customUnit = (CustomUnit)selectedUnitPanel.UserData;
            customUnit.TextureId = (customUnit.TextureId + 1) % maxTextureNumber;

            unitPicture.Renderer.Texture = UnitsAtlases[(int)customUnit.UnitType][customUnit.TextureId];

            Picture picture = (Picture)selectedUnitPanel.Get("Picture");
            picture.Renderer.Texture = UnitsAtlases[(int)customUnit.UnitType][customUnit.TextureId];
        }

        void PictureUpgradeClick(object o, EventArgs e)
        {
            if (selectedUnitPanel == null || selectedUpgradePanel == null)
                return;

            Picture picture = (Picture)o;
            picture.Renderer.Texture = yes;

            Picture mini = (Picture)selectedUnitPanel.Get(((int)picture.UserData).ToString());
            mini.Renderer.Texture = yes;

            CustomUnit customUnit = (CustomUnit)selectedUnitPanel.UserData;

            customUnit.UpgradeList[(int)picture.UserData] = (int)selectedUpgradePanel.UserData;

            UpdateMaxUnitPanelStats(customUnit);

            
        }

        void PictureUpgradeDoubleClick(object o, EventArgs e)
        {
            if (selectedUnitPanel == null)
                return;

            Picture picture = (Picture)o;
            picture.Renderer.Texture = no;

            Picture mini = (Picture)selectedUnitPanel.Get(((int)picture.UserData).ToString());
            mini.Renderer.Texture = no;

            CustomUnit customUnit = (CustomUnit)selectedUnitPanel.UserData;

            customUnit.UpgradeList[(int)picture.UserData] = 0;

            UpdateMaxUnitPanelStats(customUnit);
        }

        void EditBoxUnitChange(object o, EventArgs e)
        {
            CustomUnit customUnit = (CustomUnit)selectedUnitPanel.UserData;
            customUnit.Name = unitEditbox.Text;

            Label label = (Label)selectedUnitPanel.Get("Name");
            label.Text = customUnit.Name;
        }

        void PanelUpgradeClick(object o, EventArgs e)
        {
            if(selectedUpgradePanel != null)
            {
                selectedUpgradePanel.Renderer.Borders = new Outline(0);
            }
            selectedUpgradePanel = (Panel)o;
            selectedUpgradePanel.Renderer.Borders = new Outline(1);
        }

        Panel CreateUnitsPanel(string name)
        {
            Panel panel = new Panel();
            panel.Size = new Vector2f(620, 140);

            Label label = new Label();
            label.Text = name;
            label.Position = new Vector2f(10, 10);
            label.Size = new Vector2f(95, 30);
            label.TextSize = 18;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label);

            Button button = new Button();
            button.Text = "Add";
            button.Position = new Vector2f(10, 50);
            button.Size = new Vector2f(90, 30);
            button.TextSize = 14;
            button.Clicked += ButtonAddClick;
            panel.Add(button, "Add" + name);

            button = new Button();
            button.Text = "Remove";
            button.Position = new Vector2f(10, 90);
            button.Size = new Vector2f(90, 30);
            button.TextSize = 14;
            button.Clicked += ButtonRemoveClick;
            panel.Add(button, "Remove" + name);

            ScrollablePanel scrollablePanel = new ScrollablePanel();
            scrollablePanel.Position = new Vector2f(120, 10);
            scrollablePanel.Size = new Vector2f(470, 120);
            panel.Add(scrollablePanel, "Scroll" + name);

            return panel;
        }

        Panel CreateMiniUnitPanel(CustomUnit customUnit)
        {
            Panel panel = new Panel();
            panel.Size = new Vector2f(130, 100);
            panel.Renderer.BackgroundColor = new Color(247, 247, 247);
            panel.UserData = customUnit;
            panel.Clicked += PanelUnitClick;

            Picture picture = new Picture();
            picture.Size = new Vector2f(50, 50);
            picture.Position = new Vector2f(10, 10);
            picture.IgnoreMouseEvents = true;
            picture.Renderer.Texture = UnitsAtlases[(int)customUnit.UnitType][customUnit.TextureId];
            panel.Add(picture, "Picture");

            //Upgrades
            picture = new Picture();
            picture.Size = new Vector2f(20, 20);
            picture.Position = new Vector2f(70, 10);
            picture.IgnoreMouseEvents = true;
            if (customUnit.UpgradeList[0] == 0)
                picture.Renderer.Texture = no;
            else
                picture.Renderer.Texture = yes;
            panel.Add(picture, "0");

            picture = new Picture();
            picture.Size = new Vector2f(20, 20);
            picture.Position = new Vector2f(100, 10);
            picture.IgnoreMouseEvents = true;
            if (customUnit.UpgradeList[1] == 0)
                picture.Renderer.Texture = no;
            else
                picture.Renderer.Texture = yes;
            panel.Add(picture, "1");

            picture = new Picture();
            picture.Size = new Vector2f(20, 20);
            picture.Position = new Vector2f(70, 40);
            picture.IgnoreMouseEvents = true;
            if (customUnit.UpgradeList[2] == 0)
                picture.Renderer.Texture = no;
            else
                picture.Renderer.Texture = yes;
            panel.Add(picture, "2");

            picture = new Picture();
            picture.Size = new Vector2f(20, 20);
            picture.Position = new Vector2f(100, 40);
            picture.IgnoreMouseEvents = true;
            if (customUnit.UpgradeList[3] == 0)
                picture.Renderer.Texture = no;
            else
                picture.Renderer.Texture = yes;
            panel.Add(picture, "3");

            Label label = new Label();
            label.Text = customUnit.Name;
            label.Position = new Vector2f(10, 70);
            label.Size = new Vector2f(110, 20);
            label.TextSize = 14;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label, "Name");

            return panel;
        }

        Panel CreateMaxUnitPanel()
        {
            Panel maxPanel = new Panel();
            maxPanel.Size = new Vector2f(570, 190);

            {
                Panel panel = new Panel();
                panel.Size = new Vector2f(170, 170);
                panel.Position = new Vector2f(10, 10);
                panel.Renderer.BackgroundColor = new Color(247, 247, 247);
                maxPanel.Add(panel);

                Picture picture = new Picture();
                picture.Size = new Vector2f(110, 110);
                picture.Position = new Vector2f(10, 10);
                picture.Clicked += PictureUnitClick;
                panel.Add(picture);
                unitPicture = picture;

                //Upgrades
                picture = new Picture();
                picture.Size = new Vector2f(30, 30);
                picture.Position = new Vector2f(130, 10);
                picture.Clicked += PictureUpgradeClick;
                picture.DoubleClicked += PictureUpgradeDoubleClick;
                picture.UserData = 0;
                panel.Add(picture);
                upgrade0Picture = picture;

                picture = new Picture();
                picture.Size = new Vector2f(30, 30);
                picture.Position = new Vector2f(130, 50);
                picture.Clicked += PictureUpgradeClick;
                picture.DoubleClicked += PictureUpgradeDoubleClick;
                picture.UserData = 1;
                panel.Add(picture);
                upgrade1Picture = picture;

                picture = new Picture();
                picture.Size = new Vector2f(30, 30);
                picture.Position = new Vector2f(130, 90);
                picture.Clicked += PictureUpgradeClick;
                picture.DoubleClicked += PictureUpgradeDoubleClick;
                picture.UserData = 2;
                panel.Add(picture);
                upgrade2Picture = picture;

                picture = new Picture();
                picture.Size = new Vector2f(30, 30);
                picture.Position = new Vector2f(130, 130);
                picture.Clicked += PictureUpgradeClick;
                picture.DoubleClicked += PictureUpgradeDoubleClick;
                picture.UserData = 3;
                panel.Add(picture);
                upgrade3Picture = picture;

                EditBox editBox = new EditBox();
                editBox.Position = new Vector2f(10, 130);
                editBox.Size = new Vector2f(110, 30);
                editBox.Alignment = HorizontalAlignment.Center;
                editBox.DefaultText = "Unit Name";
                editBox.TextSize = 14;
                editBox.TextChanged += EditBoxUnitChange;
                panel.Add(editBox);
                unitEditbox = editBox;
            }

            //Stats
            {
                Panel panel = new Panel();
                panel.Size = new Vector2f(180, 170);
                panel.Position = new Vector2f(190, 10);
                panel.Renderer.BackgroundColor = new Color(247, 247, 247);
                maxPanel.Add(panel);

                Label label = new Label();
                label.Text = "Stats";
                label.Position = new Vector2f(10, 10);
                label.Size = new Vector2f(160, 20);
                label.TextSize = 16;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Health";
                label.Position = new Vector2f(10, 30);
                label.Size = new Vector2f(80, 30);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Price";
                label.Position = new Vector2f(10, 62.5f);
                label.Size = new Vector2f(80, 20);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Movment\nSpeed";
                label.Position = new Vector2f(10, 85);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "View\nDistance";
                label.Position = new Vector2f(10, 120);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "0";
                label.Position = new Vector2f(90, 30);
                label.Size = new Vector2f(80, 30);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
                healthLabel = label;

                label = new Label();
                label.Text = "0";
                label.Position = new Vector2f(90, 62.5f);
                label.Size = new Vector2f(80, 20);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
                priceLabel = label;

                label = new Label();
                label.Text = "0";
                label.Position = new Vector2f(90, 85);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
                movmentSpeedLabel = label;

                label = new Label();
                label.Text = "0";
                label.Position = new Vector2f(90, 120);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
                viewDistanceLabel = label;
            }

            //Stats Attack
            {
                Panel panel = new Panel();
                panel.Size = new Vector2f(180, 170);
                panel.Position = new Vector2f(380, 10);
                panel.Renderer.BackgroundColor = new Color(247, 247, 247);
                maxPanel.Add(panel);

                Label label = new Label();
                label.Text = "Attack";
                label.Position = new Vector2f(10, 10);
                label.Size = new Vector2f(160, 20);
                label.TextSize = 16;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Damage";
                label.Position = new Vector2f(10, 30);
                label.Size = new Vector2f(80, 20);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Bulding\nMultiplier";
                label.Position = new Vector2f(10, 50);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Attack\nSpeed";
                label.Position = new Vector2f(10, 85);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Attack\nDistance";
                label.Position = new Vector2f(10, 120);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "0";
                label.Position = new Vector2f(90, 30);
                label.Size = new Vector2f(80, 20);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
                damageLabel = label;

                label = new Label();
                label.Text = "0";
                label.Position = new Vector2f(90, 50);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
                buldingMultiLabel = label;

                label = new Label();
                label.Text = "0";
                label.Position = new Vector2f(90, 85);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
                attackSpeedLabel = label;

                label = new Label();
                label.Text = "0";
                label.Position = new Vector2f(90, 120);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
                attackDistLabel = label;
            }

            return maxPanel;
        }

        Panel CreateUpgradesPanel()
        {
            Panel panel = new Panel();
            panel.Size = new Vector2f(570, 310);

            ScrollablePanel scrollablePanel = new ScrollablePanel();
            scrollablePanel.Size = new Vector2f(550, 290);
            scrollablePanel.Position = new Vector2f(10, 10);
            panel.Add(scrollablePanel);

            int pos = 0;
            for(int i = UnitUpgradeManager.UnitUpgradesStartIndex; i<=UnitUpgradeManager.UnitUpgradesEndIndex; i++)
            {
                UnitUpgrade unitUpgrade = UnitUpgradeManager.UnitUpgrades[i];
                Panel info = CreateUpgradeInfoPanel(unitUpgrade.Name, unitUpgrade.Description);
                info.Position = new Vector2f(0, pos * 50);
                pos++;
                info.UserData = i;
                info.Clicked += PanelUpgradeClick;
                scrollablePanel.Add(info);
            }

            return panel;
        }

        Panel CreateUpgradeInfoPanel(string name, string info)
        {
            Panel panel = new Panel();
            panel.Size = new Vector2f(530, 40);
            panel.Renderer.BackgroundColor = new Color(247, 247, 247);

            Label label = new Label();
            label.Text = name;
            label.Position = new Vector2f(10, 0);
            label.Size = new Vector2f(100, 40);
            label.TextSize = 14;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label);

            label = new Label();
            label.Text = info;
            label.Position = new Vector2f(110, 0);
            label.Size = new Vector2f(410, 40);
            label.TextSize = 14;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label);

            return panel;
        }

        void UpdateUnitsScrollPanel(ScrollablePanel scrollablePanel, UnitType unitType)
        {
            scrollablePanel.RemoveAllWidgets();

            int i = 0;
            foreach(CustomUnit customUnit in customUnits)
            {
                if(customUnit.UnitType == unitType)
                {
                    Panel unitPanel = CreateMiniUnitPanel(customUnit);
                    unitPanel.Position = new Vector2f(140 * i, 0);
                    scrollablePanel.Add(unitPanel);
                    i++;
                }
            }
            scrollablePanel.UserData = i;
        }

        void AddCustomUnits(ScrollablePanel scrollablePanel, UnitType unitType)
        {
            if(customUnits.Count >= Constants.MaxUnitsPerPlayer)
            {
                // TODO: Add Error
                return;
            }

            CustomUnit customUnit = new CustomUnit(unitType, 0, "Unit");
            Panel panel = CreateMiniUnitPanel(customUnit);
            panel.Position = new Vector2f(140 * (int)scrollablePanel.UserData, 0);
            scrollablePanel.Add(panel);
            scrollablePanel.UserData = (int)scrollablePanel.UserData + 1;

            customUnits.Add(customUnit);
        }

        void RemoveCustomUnits(ScrollablePanel scrollablePanel, UnitType unitType)
        {
            CustomUnit customUnit = (CustomUnit)selectedUnitPanel.UserData;
            if (customUnit.UnitType == unitType)
            {
                customUnits.Remove(customUnit);
                UpdateUnitsScrollPanel(scrollablePanel, unitType);
            }
        }

        void UpdateMaxUnitPanel(CustomUnit customUnit)
        {
            unitPicture.Renderer.Texture = UnitsAtlases[(int)customUnit.UnitType][customUnit.TextureId];
            if (customUnit.UpgradeList[0] == 0)
                upgrade0Picture.Renderer.Texture = no;
            else
                upgrade0Picture.Renderer.Texture = yes;

            if (customUnit.UpgradeList[1] == 0)
                upgrade1Picture.Renderer.Texture = no;
            else
                upgrade1Picture.Renderer.Texture = yes;

            if (customUnit.UpgradeList[2] == 0)
                upgrade2Picture.Renderer.Texture = no;
            else
                upgrade2Picture.Renderer.Texture = yes;

            if (customUnit.UpgradeList[3] == 0)
                upgrade3Picture.Renderer.Texture = no;
            else
                upgrade3Picture.Renderer.Texture = yes;

            unitEditbox.Text = customUnit.Name;

            UpdateMaxUnitPanelStats(customUnit);

        }

        void UpdateMaxUnitPanelStats(CustomUnit customUnit)
        {
            Unit unit = UnitUpgradeManager.ProduceUnit(customUnit);

            healthLabel.Text = unit.Stats.MaxHealth.ToString();
            priceLabel.Text = unit.Stats.TrainCost.ToString();
            viewDistanceLabel.Text = unit.Stats.VisibilityDistance.ToString();
            movmentSpeedLabel.Text = unit.Stats.MovementSpeed.ToString("0.####");

            damageLabel.Text = unit.Stats.AttackDamage.ToString();
            buldingMultiLabel.Text = unit.Stats.BuildingsDamageMultiplier.ToString("0.####");
            attackSpeedLabel.Text = unit.Stats.AttackSpeed.ToString("0.####");
            attackDistLabel.Text = unit.Stats.AttackDistance.ToString("0.####");
        }
    }
}
