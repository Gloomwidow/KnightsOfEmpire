using KnightsOfEmpire.Common.GameStates;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGUI;

namespace KnightsOfEmpire.GameStates.Match
{
    class EndMatchGameState : GameState
    {
        private Panel panel;
        private string message;

        public EndMatchGameState(string message) : base()
        {
            this.message = message;
        }


        public override void Initialize()
        {
            Client.RenderWindow.Size = new Vector2u(1280, 720);
            Client.Gui.View = new View(new FloatRect(new Vector2f(0, 0), new Vector2f(Client.RenderWindow.Size.X, Client.RenderWindow.Size.Y)));
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
            Client.TCPClient.Stop();
            GameStateManager.GameState = new MainState();
        }

        private void InitializePanel()
        {
            panel = new Panel();
            panel.Position = new Vector2f(0, 0);
            panel.Size = new Vector2f(1280, 720);

            Label label = new Label();
            label.Text = message;
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
