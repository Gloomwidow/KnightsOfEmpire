using KnightsOfEmpire.Common.GameStates;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGUI;

namespace KnightsOfEmpire.GameStates.Match
{
    class DefeatGameState : GameState
    {
        private Panel panel;


        public override void Initialize()
        {
            InitializePanel();
            panel.Visible = true;
            Client.Gui.Add(panel);
        }

        public override void Render()
        {
            Client.Gui.Draw();
        }
        public override void Dispose()
        {
            Client.Gui.RemoveAllWidgets();
        }

        private void StartButton(object sender, EventArgs e)
        {
            GameStateManager.GameState = new MainState();
        }

        private void InitializePanel()
        {
            panel = new Panel();
            panel.Position = new Vector2f(0, 0);
            panel.Size = new Vector2f(1280, 720);

            Label label = new Label();
            label.Text = "You have been defeated";
            label.Position = new Vector2f(0, 190);
            label.Size = new Vector2f(1280, 150);
            label.TextSize = 100;
            label.HorizontalAlignment = HorizontalAlignment.Center;
            panel.Add(label);

            Button button = new Button();
            button.Text = "Leave game";
            button.Position = new Vector2f(490, 390);
            button.Size = new Vector2f(300, 44);
            button.TextSize = 24;
            button.Clicked += StartButton;
            panel.Add(button);
        }
    }
}
