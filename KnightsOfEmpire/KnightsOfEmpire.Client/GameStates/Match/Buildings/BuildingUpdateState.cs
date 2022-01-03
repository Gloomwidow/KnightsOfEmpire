﻿using KnightsOfEmpire.Common.Buildings;
using KnightsOfEmpire.Common.Extensions;
using KnightsOfEmpire.Common.GameStates;
using KnightsOfEmpire.Common.Helper;
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
        public Texture BuildingsColorAtlas;
        public float[,] VisibilityLevel;
        protected int AtlasSizeX;
        protected int AtlasSizeY;

        public BuildingUpdateState()
        {
            RegisterGameState(new BuildingPlacementState());
        }

        public string GetMainBuildingHealthText()
        {
            Building b = GameBuildings[Client.Resources.PlayerGameId].Find(x => x.BuildingId == BuildingManager.MainBuildingId);
            if(b!=null)
            {
                return $"{b.Health}/{b.MaxHealth}";
            }
            else
            {
                return "Destroyed!";
            }
        }

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

        public override void HandleUDPPacket(ReceivedPacket packet)
        {
            if(packet.GetHeader().StartsWith(PacketsHeaders.UpdateBuildingDataRequest))
            {
                UpdateBuildingDataRequest request = packet.GetDeserializedClassOrDefault<UpdateBuildingDataRequest>();
                if (request == null) return;
                Vector2i searchPos = new Vector2i(request.PosX, request.PosY);
                Building b = GameBuildings[request.PlayerId].Find(x =>
                {
                    return x.Position.Equals(searchPos);
                });
                if(b!=null)
                {
                    b.Health = request.Health;
                }
            }
        }
        public override void LoadDependencies()
        {
            base.LoadDependencies();
            VisibilityLevel = Parent.GetSiblingGameState<FogOfWarState>().VisibilityLevel;
        }
        public override void LoadResources()
        {
            base.LoadResources();
            BuildingsAtlas = new Texture(@"./Assets/Textures/buildings.png");
            BuildingsColorAtlas = new Texture(@"./Assets/Textures/buildings-colors.png");
            AtlasSizeX = (int)BuildingsAtlas.Size.X;
            AtlasSizeY = (int)BuildingsAtlas.Size.Y;
        }

        public override void Render()
        {
            base.Render();
            // TO-DO: color unit textures with teams color based on unit coloring
            RectangleShape buildingShape = new RectangleShape();
            RectangleShape hpBar = new RectangleShape();
            for (int i = 0; i < MaxPlayerCount; i++)
            {
                foreach (Building building in GameBuildings[i])
                {
                    float visionCoef = VisibilityLevel[building.Position.X, building.Position.Y];

                    buildingShape.Size = new Vector2f(Map.TilePixelSize, Map.TilePixelSize);
                    float PositionX = building.Position.X * Map.TilePixelSize;
                    float PositionY = building.Position.Y * Map.TilePixelSize;
                    buildingShape.Position = new Vector2f(PositionX, PositionY);
                    buildingShape.Texture = BuildingsAtlas;
                    buildingShape.TextureRect = IdToTextureRect.GetRect(BuildingManager.GetTextureId(building.BuildingId), AtlasSizeX, AtlasSizeY);
                    buildingShape.FillColor = new Color((byte)(255 * visionCoef), (byte)(255 * visionCoef), (byte)(255 * visionCoef));
                    hpBar.Size = new Vector2f(Map.TilePixelSize * building.HealthPercentage, 5);
                    hpBar.Position = new Vector2f(PositionX, PositionY + Map.TilePixelSize);
                    hpBar.FillColor = PlayerColors[i];


                    Client.RenderWindow.Draw(buildingShape);
                    buildingShape.Texture = BuildingsColorAtlas;
                    buildingShape.FillColor = new Color((byte)(PlayerColors[i].R * visionCoef), (byte)(PlayerColors[i].G * visionCoef), (byte)(PlayerColors[i].B * visionCoef));
                    Client.RenderWindow.Draw(buildingShape);
                    if (i != Client.Resources.PlayerGameId)
                    {
                        if (visionCoef == FogOfWarState.VisibilityMinLevel) continue;
                    }
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
                BuildingId = request.BuildingTypeId,
                PlayerId = request.PlayerId,
                MaxHealth = BuildingManager.GetBuilding(request.BuildingTypeId).MaxHealth,
                Position = new Vector2i(request.BuildingPosX, request.BuildingPosY),
            };

            GameBuildings[request.PlayerId].Add(building);
            Client.Resources.Map.TileTypes[request.BuildingPosX][request.BuildingPosY] = TileType.Building;
        }
        protected void UnregisterBuilding(ReceivedPacket packet)
        {
            UnregisterBuildingRequest request = packet.GetDeserializedClassOrDefault<UnregisterBuildingRequest>();
            int deleteIndex = GameBuildings[request.PlayerId].FindIndex(x => x.Position.Equals(new Vector2i(request.DestroyPosX,request.DestroyPosY)));
            if (deleteIndex != -1)
            {
                GameBuildings[request.PlayerId].RemoveAt(deleteIndex);
                Client.Resources.Map.TileTypes[request.DestroyPosX][request.DestroyPosY] = TileType.Walkable;
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            BuildingsAtlas.Dispose();
        }
    }
}
