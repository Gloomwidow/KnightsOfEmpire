using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
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
        public float[,] VisibilityLevel;

        List<UpdateUnitData> updateUnitDatas;

        public Texture UnitsAtlas;


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
            List<UpdateUnitsResponse> updateUnitResponses = new List<UpdateUnitsResponse>();

            foreach(ReceivedPacket packet in packets)
            {
                if(packet.GetHeader() == PacketsHeaders.GameUnitUpdateRequest)
                {

                    UpdateUnitsResponse updateUnitsResponse = packet.GetDeserializedClassOrDefault<UpdateUnitsResponse>();
                    if(updateUnitsResponse!=null) updateUnitResponses.Add(updateUnitsResponse);
                }
            }
            updateUnitResponses.Sort((UpdateUnitsResponse r1, UpdateUnitsResponse r2) =>
            {
                return r2.TimeStamp.CompareTo(r1.TimeStamp);
            });

            updateUnitDatas = new List<UpdateUnitData>();
            foreach(UpdateUnitsResponse response in updateUnitResponses)
            {
                foreach(UpdateUnitData data in response.UnitData)
                {
                    if(data != null)
                    {
                        updateUnitDatas.Add(data);
                    }
                }
            }
        }
        public override void LoadResources()
        {
            UnitsAtlas = new Texture(@"./Assets/Textures/cavalry - light.png");
        }
        public override void Initialize()
        {
            base.Initialize();
            selectionState = new UnitsSelectionState();
            selectionState.GameUnits = this.GameUnits;
           
        }

        public override void LoadDependencies()
        {
            VisibilityLevel = Parent.GetSiblingGameState<FogOfWarState>().VisibilityLevel;
        }

        public override void Update()
        {
            selectionState.Update();

            // Update Units Data

            foreach(UpdateUnitData data in updateUnitDatas)
            {
                Unit unit = GameUnits[data.PlayerId].Find((Unit u) => { return u.EqualID(data.UnitId); });
                if(unit != null)
                {
                    unit.UpdateData(data);
                }
            }

            //Console.WriteLine("Uppdate units!");
        }

        public override void Render()
        {
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
            RectangleShape unitShape = new RectangleShape();
            RectangleShape hpBar = new RectangleShape();
            for (int i=0; i< MaxPlayerCount; i++)
            {
                foreach(Unit unit in GameUnits[i])
                {
                    Vector2i pos = Map.ToTilePos(unit.Position);
                    float visionCoef = VisibilityLevel[pos.X, pos.Y];

                    if(i!=Client.Resources.PlayerGameId)
                    {
                        if (visionCoef == FogOfWarState.VisibilityMinLevel) continue;
                    }

                    unitShape.Size = new Vector2f(Unit.UnitSize, Unit.UnitSize);
                    unitShape.Position = new Vector2f(unit.Position.X-(Unit.UnitSize/2), unit.Position.Y - (Unit.UnitSize / 2));
                    unitShape.Texture = UnitsAtlas;
                    unitShape.TextureRect = new IntRect(0, 0, 16, 16);
                    unitShape.FillColor = new Color((byte)(playerColors[i].R*visionCoef), (byte)(playerColors[i].G * visionCoef), (byte)(playerColors[i].B * visionCoef));
                    unitShape.OutlineColor = Color.Blue;
                    unitShape.OutlineThickness = 0;
                    if (unit.IsSelected) unitShape.OutlineThickness = 1;
                    hpBar.Size = new Vector2f(Unit.UnitSize*unit.Stats.HealthPercentage, 5);
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
            RegisterUnitRequest request = packet.GetDeserializedClassOrDefault<RegisterUnitRequest>();
            if (request == null) return;

            Unit unit = new Unit()
            {
                ID = request.ID,
                PlayerId = request.PlayerId,
                Position = new Vector2f(request.StartPositionX, request.StartPositionY),
                TextureId = 0,
                Stats = new UnitStats()
            };

            GameUnits[request.PlayerId].Add(unit);
        }

        protected void UnregisterUnit(ReceivedPacket packet)
        {
            UnregisterUnitRequest request = packet.GetDeserializedClassOrDefault<UnregisterUnitRequest>();
            int deleteIndex = GameUnits[request.PlayerId].FindIndex(x => x.EqualID(request.ID));
            if (deleteIndex != -1) GameUnits[request.PlayerId].RemoveAt(deleteIndex);
            else return;
        }

        public override void Dispose()
        {
            UnitsAtlas.Dispose();
        }
    }  
}
