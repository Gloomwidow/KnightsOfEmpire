using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Helper;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Units;
using KnightsOfEmpire.Common.Units;
using KnightsOfEmpire.Common.Units.Enum;
using KnightsOfEmpire.Common.Units.Modifications;
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

        public Texture[] UnitsAtlases;
        public Vector2i[] UnitAtlasSizes;


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
            int unitTypesCount = Enum.GetNames(typeof(UnitType)).Length;
            UnitsAtlases = new Texture[unitTypesCount];
            UnitAtlasSizes = new Vector2i[unitTypesCount];
            UnitsAtlases[0] = new Texture(@"./Assets/Textures/heavy infantry.png");
            UnitsAtlases[1] = new Texture(@"./Assets/Textures/light infantry.png");
            UnitsAtlases[2] = new Texture(@"./Assets/Textures/cavalry - light.png");
            //TO-Do: once we will find siege textures, replace them
            UnitsAtlases[3] = new Texture(@"./Assets/Textures/cavalry - heavy.png");
            for(int i=0;i<unitTypesCount;i++)
            {
                UnitAtlasSizes[i] = new Vector2i((int)UnitsAtlases[i].Size.X, (int)UnitsAtlases[i].Size.Y);
            }
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
                    CustomUnit unitInfo = Client.Resources.GameCustomUnits[unit.PlayerId].Units[unit.UnitTypeId];
                    unitShape.Size = new Vector2f(Unit.UnitSize, Unit.UnitSize);
                    unitShape.Position = new Vector2f(unit.Position.X-(Unit.UnitSize/2), unit.Position.Y - (Unit.UnitSize / 2));
                    int unitArchetypeId = (int)unitInfo.UnitType;

                    unitShape.Texture = UnitsAtlases[unitArchetypeId];
                    unitShape.TextureRect = IdToTextureRect.GetRect(unitInfo.TextureId, UnitAtlasSizes[unitArchetypeId]);
                    //unitShape.FillColor = new Color((byte)(PlayerColors[i].R*visionCoef), (byte)(PlayerColors[i].G * visionCoef), (byte)(PlayerColors[i].B * visionCoef));
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

            Unit unit = UnitUpgradeManager.ProduceUnit(
                Client.Resources.GameCustomUnits[request.PlayerId].Units[request.UnitTypeId]);

            unit.ID = request.ID;
            unit.PlayerId = request.PlayerId;
            unit.Position = new Vector2f(request.StartPositionX, request.StartPositionY);
            unit.UnitTypeId = request.UnitTypeId;

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
            for(int i=0;i<4;i++)
            {
                UnitsAtlases[i].Dispose();
            }
        }
    }  
}
