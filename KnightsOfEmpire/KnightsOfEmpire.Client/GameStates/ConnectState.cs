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
using KnightsOfEmpire.Common.Networking.TCP;

namespace KnightsOfEmpire.GameStates
{
    public class ConnectState : GameState
    {
        // Private varible to control connection proces
        protected enum State { Null, TryValidate, TryConnect, Unconnect, Connect }
        protected State state = State.Null;

        // IP and Port variable
        private string ip;
        private string port;
        private Int32 portInt;

        // Error label
        private Label ipErrorLabel;
        private Label portErrorLabel;
        private Label unableToConnectLabel;

        // Error strings
        private const string ipErrorStr = "Incorrect IP Adress";
        private const string ipEmptyStr = "Empty IP Adress";
        private const string portErrorStr = "Incorrect Port Adress";
        private const string portEmptyStr = "Empty Port Adress";

        // Main view
        private Panel mainPanel;

        // Connect view
        private Panel connectPanel;


        /// <summary>
        /// Initialize GUI for connection to server
        /// </summary>
        public override void Initialize()
        {
            Vector2u windowSize = Client.RenderWindow.Size;

            mainPanel = new Panel();
            mainPanel.Position = new Vector2f(0, 0);
            mainPanel.Size = ((Vector2f)windowSize);

            Label label = new Label();
            label.Text = "Knights of Empire";
            label.Position = new Vector2f(270, 200);
            label.TextSize = 80;
            mainPanel.Add(label);

            Label labelIP = new Label();
            labelIP.Text = "IP Adress";
            labelIP.Position = new Vector2f(580, 360);
            labelIP.TextSize = 24;
            mainPanel.Add(labelIP);

            EditBox editBoxIP = new EditBox();
            editBoxIP.Position = new Vector2f(490, 400);
            editBoxIP.Size = new Vector2f(300, 40);
            editBoxIP.TextSize = 18;
            editBoxIP.DefaultText = "0.0.0.0";
            editBoxIP.Alignment = HorizontalAlignment.Center;
            editBoxIP.InputValidator = "[0-9.]*";
            mainPanel.Add(editBoxIP, "EditBoxIP");

            ipErrorLabel = new Label();
            ipErrorLabel.Position = new Vector2f(580, 440);
            ipErrorLabel.Text = ipErrorStr;
            ipErrorLabel.TextSize = 13;
            ipErrorLabel.Visible = false;
            ipErrorLabel.Renderer.TextColor = new Color(201, 52, 52);
            mainPanel.Add(ipErrorLabel);

            Label labelPort = new Label();
            labelPort.Text = "Port";
            labelPort.Position = new Vector2f(610, 460);
            labelPort.TextSize = 24;
            mainPanel.Add(labelPort);

            EditBox editBoxPort = new EditBox();
            editBoxPort.Position = new Vector2f(490, 500);
            editBoxPort.Size = new Vector2f(300, 40);
            editBoxPort.TextSize = 18;
            editBoxPort.DefaultText = "00000";
            editBoxPort.Alignment = HorizontalAlignment.Center;
            editBoxPort.InputValidator = "[0-9]*";
            mainPanel.Add(editBoxPort, "EditBoxPort");

            portErrorLabel = new Label();
            portErrorLabel.Position = new Vector2f(580, 540);
            portErrorLabel.Text = portErrorStr;
            portErrorLabel.TextSize = 13;
            portErrorLabel.Visible = false;
            portErrorLabel.Renderer.TextColor = new Color(201, 52, 52);
            mainPanel.Add(portErrorLabel);

            Button button = new Button();
            button.Position = new Vector2f(565, 570);
            button.Size = new Vector2f(150, 40);
            button.Text = "Connect";
            button.TextSize = 18;
            button.Clicked += Connect;
            mainPanel.Add(button);

            unableToConnectLabel = new Label();
            unableToConnectLabel.Position = new Vector2f(565, 630);
            unableToConnectLabel.Text = "Unable to connect";
            unableToConnectLabel.TextSize = 16;
            unableToConnectLabel.Visible = false;
            unableToConnectLabel.Renderer.TextColor = new Color(201, 52, 52);
            mainPanel.Add(unableToConnectLabel);

            mainPanel.Visible = true;
            Client.Gui.Add(mainPanel);

            connectPanel = new Panel();
            connectPanel.Position = new Vector2f(0, 0);
            connectPanel.Size = ((Vector2f)windowSize);
            connectPanel.Renderer.BackgroundColor = new Color(247, 247, 247);
            connectPanel.Renderer.Opacity = 0.9f;

            Label connectLabel = new Label();
            connectLabel.Text = "Connection...";
            connectLabel.Position = new Vector2f(470, 327);
            connectLabel.TextSize = 50;
            connectPanel.Add(connectLabel);

            connectPanel.Visible = false;
            Client.Gui.Add(connectPanel);

        }

        /// <summary>
        /// Update state in ConnectState to check user data or connect to server
        /// </summary>
        public override void Update()
        {
            switch (state)
            {
                case State.Null:
                    break;
                case State.TryValidate:
                    {
                        ipErrorLabel.Visible = false;
                        portErrorLabel.Visible = false;
                        unableToConnectLabel.Visible = false;

                        ValidateIP();
                        ValidatePort();

                        if (ipErrorLabel.Visible == false && portErrorLabel.Visible == false)
                            state = State.TryConnect;
                    }
                    break;
                
                case State.TryConnect:
                    {
                        if(connectPanel.Visible == true)
                        {
                            Client.TCPClient = new TCPClient(ip, portInt);
                            Client.TCPClient.Start();

                            if (Client.TCPClient.isRunning)
                                state = State.Connect;
                            else
                                state = State.Unconnect;
                        }
                        connectPanel.Visible = true;
                    }
                    break;
                case State.Unconnect:
                    {
                        connectPanel.Visible = false;

                        unableToConnectLabel.Visible = true;

                        state = State.Null;
                    }
                    break;
                case State.Connect:
                    GameStateManager.GameState = new TestState();
                    break;
            }
        }

        /// <summary>
        /// Draw GUI
        /// </summary>
        public override void Render()
        {
            Client.RenderWindow.Clear(new Color(247, 247, 247));
            Client.Gui.Draw();
        }

        /// <summary>
        /// Remove all wigets from GUI
        /// </summary>
        public override void Dispose()
        {
            Client.Gui.RemoveAllWidgets();
        }



        private void Connect(object sender, EventArgs e)
        {
            ip = ((EditBox)((Button)sender).ParentGui.Get("EditBoxIP")).Text;
            port = ((EditBox)((Button)sender).ParentGui.Get("EditBoxPort")).Text;

            state = State.TryValidate;

            Console.WriteLine("IP:" + ip + " Port: " + port);
        }

        private void ValidateIP()
        {
            if (ip == "")
            {
                ipErrorLabel.Visible = true;
                ipErrorLabel.Text = ipEmptyStr;
            }
            else
            {
                ipErrorLabel.Text = ipErrorStr;

                string[] bytes = ip.Split('.');
                if (bytes.Length != 4)
                    ipErrorLabel.Visible = true;

                foreach (string b in bytes)
                {
                    if (b == "")
                        ipErrorLabel.Visible = true;
                    else if (b.Length > 1 && b[0] == '0')
                        ipErrorLabel.Visible = true;
                    else if (!byte.TryParse(b, out _))
                        ipErrorLabel.Visible = true;
                }
            }
        }

        void ValidatePort()
        {
            if (port == "")
            {
                portErrorLabel.Visible = true;
                portErrorLabel.Text = portEmptyStr;
            }
            else
            {
                portErrorLabel.Text = portErrorStr;

                if (port.Length > 1 && port[0] == '0')
                    portErrorLabel.Visible = true;
                if (!Int32.TryParse(port, out portInt))
                    portErrorLabel.Visible = true;
            }
        }

    }
}
