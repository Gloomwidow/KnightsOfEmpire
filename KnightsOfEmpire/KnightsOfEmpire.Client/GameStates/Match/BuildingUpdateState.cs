using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Map;
using KnightsOfEmpire.Common.Networking;
using KnightsOfEmpire.Common.Resources.Buildings;
using KnightsOfEmpire.Common.Units;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.GameStates.Match
{
    public class BuildingUpdateState : BuildingState
    {
        public Texture BuildingsAtlas;
        public float[,] VisibilityLevel;

        public override void HandleTCPPacket(ReceivedPacket packet)
        {
            switch (packet.GetHeader())
            {
                case PacketsHeaders.RegisterBuildingRequest:
                    RegisterBuilding(packet);
                    break;
                case PacketsHeaders.UnregisterBuildingRequest:
                    UnregisterBuilding(packet);
                    break;

            }
        }
        public override void Initialize()
        {
            base.Initialize();
        }
        public override void LoadResources()
        {
            BuildingsAtlas = new Texture(@"./Assets/Textures/tileset.png");
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
            RectangleShape buildingShape = new RectangleShape();
            RectangleShape hpBar = new RectangleShape();
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                foreach (Building building in GameBuildings[i])
                {
                    Vector2i pos = Map.ToTilePos((Vector2f)building.Position);
                    float visionCoef = VisibilityLevel[pos.X, pos.Y];

                    if (i != Client.Resources.PlayerGameId)
                    {
                        if (visionCoef == FogOfWarState.VisibilityMinLevel) continue;
                    }

                    buildingShape.Size = new Vector2f(Map.TilePixelSize, Map.TilePixelSize);
                    buildingShape.Position = new Vector2f(building.Position.X - (Map.TilePixelSize / 2), building.Position.Y - (Map.TilePixelSize / 2));
                    buildingShape.Texture = BuildingsAtlas;
                    buildingShape.TextureRect = new IntRect(0, 0, 16, 16);
                    buildingShape.FillColor = new Color((byte)(playerColors[i].R * visionCoef), (byte)(playerColors[i].G * visionCoef), (byte)(playerColors[i].B * visionCoef));
                    hpBar.Size = new Vector2f(Map.TilePixelSize * building.HealthPercentage, 5);
                    hpBar.Position = new Vector2f(building.Position.X - (Map.TilePixelSize / 2), building.Position.Y + (Map.TilePixelSize / 2));
                    hpBar.FillColor = playerColors[i];


                    Client.RenderWindow.Draw(buildingShape);
                    Client.RenderWindow.Draw(hpBar);
                }
            }
        }
        protected void RegisterBuilding(ReceivedPacket packet)
        {
            RegisterBuildingRequest request = packet.GetDeserializedClassOrDefault<RegisterBuildingRequest>();
            if (request == null) return;

            Building building = new Building()
            {
                PlayerId = request.PlayerId,
                Position = new Vector2i(request.BuildingPosX, request.BuildingPosY),
                TextureId = request.BuildingTypeId
            };

            GameBuildings[request.PlayerId].Add(building);
        }
        protected void UnregisterBuilding(ReceivedPacket packet)
        {
            UnregisterBuildingRequest request = packet.GetDeserializedClassOrDefault<UnregisterBuildingRequest>();
            int deleteIndex = GameBuildings[request.PlayerId].FindIndex(x => x.EqualPosition(new Vector2i(request.DestroyPosX,request.DestroyPosY)));
            if (deleteIndex != -1) GameBuildings[request.PlayerId].RemoveAt(deleteIndex);
            else return;
        }
        public override void Dispose()
        {
            BuildingsAtlas.Dispose();
        }
    }
}
