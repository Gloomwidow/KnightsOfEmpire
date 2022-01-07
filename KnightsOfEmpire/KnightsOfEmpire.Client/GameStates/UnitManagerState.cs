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

namespace KnightsOfEmpire.GameStates
{
    class UnitManagerState : GameState
    {
        Panel mainPanel;

        public override void LoadResources()
        {
            
        }

        public override void Initialize()
        {
            InitializeMainPanel();
            Client.Gui.Add(mainPanel);
        }

        public override void HandleTCPPackets(List<ReceivedPacket> packets)
        {
            
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

            Panel panel = CreateUnitsPanel("Soldiers");
            panel.Position = new Vector2f(30, 100);
            mainPanel.Add(panel);

            panel = CreateUnitsPanel("Archers");
            panel.Position = new Vector2f(30, 250);
            mainPanel.Add(panel);

            panel = CreateUnitsPanel("Riders");
            panel.Position = new Vector2f(30, 400);
            mainPanel.Add(panel);

            panel = CreateUnitsPanel("Machines");
            panel.Position = new Vector2f(30, 550);
            mainPanel.Add(panel);

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
            // TODO: Save

            GameStateManager.GameState = new MainState("Units saved");
        }

        void ButtonExitClick(object o, EventArgs e)
        {
            GameStateManager.GameState = new MainState("Warning: Units not saved");
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
            panel.Add(button, "Add" + name);

            button = new Button();
            button.Text = "Remove";
            button.Position = new Vector2f(10, 90);
            button.Size = new Vector2f(90, 30);
            button.TextSize = 14;
            panel.Add(button, "Remove" + name);

            ScrollablePanel scrollablePanel = new ScrollablePanel();
            scrollablePanel.Position = new Vector2f(120, 10);
            scrollablePanel.Size = new Vector2f(470, 120);
            panel.Add(scrollablePanel, "Scroll" + name);

            for(int i=0;i<5;i++)
            {
                Panel unitPanel = CreateMiniUnitPanel();
                unitPanel.Position = new Vector2f(140 * i, 0);
                scrollablePanel.Add(unitPanel);
            }

            return panel;
        }

        Panel CreateMiniUnitPanel()
        {
            Panel panel = new Panel();
            panel.Size = new Vector2f(130, 100);
            panel.Renderer.BackgroundColor = new Color(247, 247, 247);

            Picture picture = new Picture();
            picture.Size = new Vector2f(50, 50);
            picture.Position = new Vector2f(10, 10);
            picture.IgnoreMouseEvents = true;
            panel.Add(picture);

            //Upgrades
            picture = new Picture();
            picture.Size = new Vector2f(20, 20);
            picture.Position = new Vector2f(70, 10);
            picture.IgnoreMouseEvents = true;
            panel.Add(picture);

            picture = new Picture();
            picture.Size = new Vector2f(20, 20);
            picture.Position = new Vector2f(100, 70);
            picture.IgnoreMouseEvents = true;
            panel.Add(picture);

            picture = new Picture();
            picture.Size = new Vector2f(20, 20);
            picture.Position = new Vector2f(70, 40);
            picture.IgnoreMouseEvents = true;
            panel.Add(picture);

            picture = new Picture();
            picture.Size = new Vector2f(20, 20);
            picture.Position = new Vector2f(100, 40);
            picture.IgnoreMouseEvents = true;
            panel.Add(picture);

            Label label = new Label();
            label.Text = "Worior";
            label.Position = new Vector2f(10, 70);
            label.Size = new Vector2f(110, 20);
            label.TextSize = 14;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label);

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
                panel.Add(picture);

                //Upgrades
                picture = new Picture();
                picture.Size = new Vector2f(30, 30);
                picture.Position = new Vector2f(130, 10);
                panel.Add(picture);

                picture = new Picture();
                picture.Size = new Vector2f(30, 30);
                picture.Position = new Vector2f(130, 50);
                panel.Add(picture);

                picture = new Picture();
                picture.Size = new Vector2f(30, 30);
                picture.Position = new Vector2f(130, 90);
                panel.Add(picture);

                picture = new Picture();
                picture.Size = new Vector2f(30, 30);
                picture.Position = new Vector2f(130, 130);
                panel.Add(picture);

                EditBox editBox = new EditBox();
                editBox.Position = new Vector2f(10, 130);
                editBox.Size = new Vector2f(110, 30);
                editBox.Alignment = HorizontalAlignment.Center;
                editBox.DefaultText = "Unit Name";
                editBox.TextSize = 14;
                panel.Add(editBox);
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
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Movment\nSpeed";
                label.Position = new Vector2f(10, 70);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "View\nDistance";
                label.Position = new Vector2f(10, 110);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "100";
                label.Position = new Vector2f(90, 30);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "20";
                label.Position = new Vector2f(90, 70);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "5";
                label.Position = new Vector2f(90, 110);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
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
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Attack\nSpeed";
                label.Position = new Vector2f(10, 70);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "Attack\nDistance";
                label.Position = new Vector2f(10, 110);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "25";
                label.Position = new Vector2f(90, 30);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "10";
                label.Position = new Vector2f(90, 70);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);

                label = new Label();
                label.Text = "40";
                label.Position = new Vector2f(90, 110);
                label.Size = new Vector2f(80, 40);
                label.TextSize = 14;
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignmentAlignment = VerticalAlignment.Center;
                label.IgnoreMouseEvents = true;
                panel.Add(label);
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

            for(int i=0; i<10; i++)
            {
                Panel info = CreateUpgradeInfoPanel("Upgrade " + (i+1).ToString() + ", Info: Lorem ipsum...");
                info.Position = new Vector2f(0, i * 50);
                scrollablePanel.Add(info);
            }

            return panel;
        }

        Panel CreateUpgradeInfoPanel(string info)
        {
            Panel panel = new Panel();
            panel.Size = new Vector2f(530, 40);
            panel.Renderer.BackgroundColor = new Color(247, 247, 247);

            Label label = new Label();
            label.Text = "Upgrade 1, Info: Lorem ipsum...";
            label.Position = new Vector2f(10, 0);
            label.Size = new Vector2f(510, 40);
            label.TextSize = 14;
            label.HorizontalAlignment = HorizontalAlignment.Left;
            label.VerticalAlignmentAlignment = VerticalAlignment.Center;
            label.IgnoreMouseEvents = true;
            panel.Add(label);

            return panel;
        }
    }
}
