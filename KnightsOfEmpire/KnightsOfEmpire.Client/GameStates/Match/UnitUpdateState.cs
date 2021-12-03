using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Units;
using KnightsOfEmpire.Common.Units;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnightsOfEmpire.GameStates.Match
{
    public class UnitUpdateState : UnitState
    {
        public UnitsSelectionState selectionState;
        public override void HandleTCPPacket(ReceivedPacket packet)
        {
            switch(packet.GetHeader())
            {
                    case PacketsHeaders.GameUnitRegisterRequest:
                        RegisterUnit(packet);
                        break;
                    case PacketsHeaders.GameUnitUnregisterRequest:
                        UnregisterUnit(packet);
                        break;

            }
        }

        public override void HandleUDPPackets(List<ReceivedPacket> packets)
        {
            // TO-DO: handle units update from UDP packets 
        }
        public override void Initialize()
        {
            base.Initialize();
            selectionState = new UnitsSelectionState();
            selectionState.GameUnits = this.GameUnits;
        }

        public override void Update()
        {
            selectionState.Update();
        }

        public override void Render()
        {
            // TO-DO: render units with actual texture instead of simple Rect
            // TO-DO: render health bars
            // TO-DO: color unit textures with teams color based on unit coloring

            // different red colors to distinguish other enemy units
            Color[] playerColors = new Color[]
            {
                new Color(255,25,0),
                new Color(242,11,0),
                new Color(255,0,0),
                new Color(245,11,23)
            };
            // friendly units are always green
            playerColors[Client.Resources.PlayerGameId] = Color.Green;
 
            for (int i=0; i< MaxPlayerCount; i++)
            {
                foreach(Unit unit in GameUnits[i])
                {
                    RectangleShape unitShape = new RectangleShape(new Vector2f(Unit.UnitSize, Unit.UnitSize));
                    unitShape.Position = new Vector2f(unit.Position.X-(Unit.UnitSize/2), unit.Position.Y - (Unit.UnitSize / 2));
                    unitShape.Texture = new Texture(@"./Assets/Textures/cavalry - light.png",new IntRect(0,0,16,16));
                    //unitShape.FillColor = playerColors[i];
                    
                    if (unit.IsSelected) 
                    {
                        unitShape.OutlineColor = Color.Blue;
                        unitShape.OutlineThickness = 1;
                    }

                    RectangleShape hpBar = new RectangleShape(new Vector2f(Unit.UnitSize, 5));
                    hpBar.Position = new Vector2f(unit.Position.X - (Unit.UnitSize / 2), unit.Position.Y + (Unit.UnitSize / 2));
                    hpBar.FillColor = playerColors[i];


                    Client.RenderWindow.Draw(unitShape);
                    Client.RenderWindow.Draw(hpBar);
                }
            }
            selectionState.Render();
        }



        protected void RegisterUnit(ReceivedPacket packet)
        {
            RegisterUnitRequest request = null;
            try
            {
                request = JsonSerializer.Deserialize<RegisterUnitRequest>(packet.GetContent());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Unit unit = new Unit()
            {
                ID = request.ID,
                Position = new Vector2f(request.StartPositionX, request.StartPositionY),
                TextureId = 0,
                Stats = new UnitStats()
            };

            GameUnits[request.PlayerId].Add(unit);
        }

        protected void UnregisterUnit(ReceivedPacket packet)
        {
            UnregisterUnitRequest request = null;
            try
            {
                request = JsonSerializer.Deserialize<UnregisterUnitRequest>(packet.GetContent());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
            int deleteIndex = GameUnits[request.PlayerId].FindIndex(x => x.EqualID(request.ID));
            if (deleteIndex != -1) GameUnits[request.PlayerId].RemoveAt(deleteIndex);
            else Console.WriteLine("Unit not found!!!");

            //TO-DO: Make sure that units deleting won't mess up unit selection with Null References
        }

        

    }  
}
