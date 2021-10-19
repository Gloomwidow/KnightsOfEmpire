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
    class TestState : GameState
    {
        Gui gui;

        public override void LoadResources()
        {
            return;
        }

        public override void Initialize()
        {
            gui = new Gui(Client.RenderWindow);

            Label label = new Label();
            label.Text = "Knights of Empire";
            label.Position = new Vector2f(270, 200);
            label.TextSize = 80;
            gui.Add(label);

            Label labelTest = new Label();
            labelTest.Text = "Test State";
            labelTest.Position = new Vector2f(575, 300);
            labelTest.TextSize = 24;
            gui.Add(labelTest);

            return;
        }

        public override void HandlePackets(List<ReceivedPacket> packets)
        {
            return;
        }

        public override void Update()
        {
            return;
        }

        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(247, 247, 247));
            gui.Draw();
            return;
        }

        public override void Dispose()
        {
            gui.Dispose();
            return;
        }
    }
}
