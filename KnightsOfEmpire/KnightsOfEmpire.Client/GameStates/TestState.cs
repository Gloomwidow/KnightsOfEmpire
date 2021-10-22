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
        /// <summary>
        /// Create GUI for TestState
        /// </summary>
        public override void Initialize()
        {
            Label label = new Label();
            label.Text = "Knights of Empire";
            label.Position = new Vector2f(270, 200);
            label.TextSize = 80;
            Client.Gui.Add(label);

            Label labelTest = new Label();
            labelTest.Text = "Test State";
            labelTest.Position = new Vector2f(575, 300);
            labelTest.TextSize = 24;
            Client.Gui.Add(labelTest);

            return;
        }


        /// <summary>
        /// Draw GUI
        /// </summary>
        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(247, 247, 247));
            Client.Gui.Draw();
            return;
        }

        /// <summary>
        /// Clear GUI
        /// </summary>
        public override void Dispose()
        {
            Client.Gui.RemoveAllWidgets();
            return;
        }
    }
}
