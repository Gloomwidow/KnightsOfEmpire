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
        public float[,] VisibilityLevel;

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

        public override void HandleUDPPacket(ReceivedPacket packet)
        {
            if(packet.GetHeader() == PacketsHeaders.GameUnitUpdateRequest)
            {
                UpdateUnitsResponse updateUnitsResponse = packet.GetDeserializedClassOrDefault<UpdateUnitsResponse>();
                if (updateUnitsResponse == null) return;
                foreach (UpdateUnitData data in updateUnitsResponse.UnitData)
                {
                    if (data == null) break;
                    Unit unit = GameUnits[data.PlayerId].Find((Unit u) => { return u.EqualID(data.UnitId); });
                    if(unit!=null) unit.UpdateData(data);
                }
            }
            
        }

        public UnitUpdateState()
        {
            RegisterGameState(new UnitsSelectionState());
        }
        public override void LoadResources()
        {
            UnitsAtlas = new Texture(@"./Assets/Textures/cavalry - light.png");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadDependencies()
        {
            base.LoadDependencies();
            VisibilityLevel = Parent.GetSiblingGameState<FogOfWarState>().VisibilityLevel;
        }

        public override void Render()
        {
            base.Render();
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
                    unitShape.FillColor = new Color((byte)(PlayerColors[i].R*visionCoef), (byte)(PlayerColors[i].G * visionCoef), (byte)(PlayerColors[i].B * visionCoef));
                    unitShape.OutlineColor = Color.White;
                    unitShape.OutlineThickness = 0;
                    if (unit.IsSelected) unitShape.OutlineThickness = 1;
                    hpBar.Size = new Vector2f(Unit.UnitSize*unit.Stats.HealthPercentage, 5);
                    hpBar.Position = new Vector2f(unit.Position.X - (Unit.UnitSize / 2), unit.Position.Y + (Unit.UnitSize / 2));
                    hpBar.FillColor = PlayerColors[i];


                    Client.RenderWindow.Draw(unitShape);
                    Client.RenderWindow.Draw(hpBar);
                }
            }
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
            base.Dispose();
            UnitsAtlas.Dispose();
        }
    }  
}
