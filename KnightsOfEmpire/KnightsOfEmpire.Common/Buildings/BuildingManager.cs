﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Buildings
{
    public class BuildingInfo
    {
        public Building Building;
        public string Name;
        public string Description;
    }

    public static class BuildingManager
    {
        public static int MainBuildingId = 32;
        private static Dictionary<int, BuildingInfo> buildingsInfo = new Dictionary<int, BuildingInfo>()
        { 
            [0] = new BuildingInfo 
            {
                Name = "Barracks",
                Description = "Increases max unit capacity, allowing Empires to have more units at once.",
                Building = new Building
                {
                    BuildCost = 100,
                    MaxHealth = 250,
                    TextureId = 5,
                }
            },
            [1] = new BuildingInfo
            {
                Name = "Gold Mine",
                Description = "Increases gold income.",
                Building = new Building
                {
                    BuildCost = 150,
                    MaxHealth = 250,
                    TextureId = 6,
                }
            },
            [2] = new BuildingInfo
            {
                Name = "Ritual Site",
                Description = "Periodically heals friendly units in close range.",
                Building = new Building
                {
                    BuildCost = 150,
                    MaxHealth = 250,
                    TextureId = 8,
                }
            },
            [MainBuildingId] = new BuildingInfo
            {
                Name = "Main Building",
                Description = "The Heart of Empire. Player is defeated when their main building is destroyed.",
                Building = new Building
                {
                    BuildCost = 0,
                    MaxHealth = 1000,
                    TextureId = 0,
                }
            },
        };

        public static int GetNextBuildingId(int buttonId)
        {
            int skip = 0;
            foreach (int b in buildingsInfo.Keys)
            {
                if (b == MainBuildingId) continue;
                if (skip == buttonId) return b;
                else skip++;
            }
            return -1;
        }

        public static BuildingInfo GetNextBuilding(int buttonId)
        {
            int skip = 0;
            foreach(int b in buildingsInfo.Keys)
            {
                if (b == MainBuildingId) continue;
                if (skip == buttonId) return buildingsInfo[b];
                else skip++;
            }
            return null;
        }


        public static Building GetBuilding(int buildingId)
        {
            BuildingInfo info = null;
            if (!buildingsInfo.TryGetValue(buildingId, out info)) return null;
            Building building = new Building(info.Building);
            building.BuildingId = buildingId;
            return building;
        }

        public static string GetName(int buildingId)
        {
            BuildingInfo info = null;
            if (!buildingsInfo.TryGetValue(buildingId, out info)) return string.Empty;
            return info.Name;
        }

        public static string GetDescription(int buildingId)
        {
            BuildingInfo info = null;
            if (!buildingsInfo.TryGetValue(buildingId, out info)) return string.Empty;
            return info.Description;
        }

        public static int GetTextureId(int buildingId)
        {
            BuildingInfo info = null;
            if (!buildingsInfo.TryGetValue(buildingId, out info)) return 0;
            return info.Building.TextureId;
        }

    }
}
