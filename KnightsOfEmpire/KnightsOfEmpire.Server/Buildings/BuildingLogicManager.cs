using KnightsOfEmpire.Common.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Server.Buildings
{
    class BuildingData
    {
        public int Id;
        public BuildingLogic Logic;

    }

    public static class BuildingLogicManager
    {
        private static BuildingLogic defaultBuildingLogic = new BuildingLogic();
        private static Dictionary<int, BuildingLogic> buildingsLogic = new Dictionary<int, BuildingLogic>()
        {
            [0] = new UnitCapacityBuildingLogic(10),
            [1] = new GoldGenerationBuildingLogic(1, 2),
            [2] = new FriendlyHealingBuildingLogic(10, 128, 5),
            [BuildingManager.MainBuildingId] = new MainBuildingLogic(5, 1),
        };


        private static BuildingLogic GetLogicOrDefault(int buildingId)
        {
            BuildingLogic logic;
            if(!buildingsLogic.TryGetValue(buildingId, out logic))
            {
                return defaultBuildingLogic;
            }
            return logic;

        }

        public static void ProcessOnCreate(Building building)
        {
            BuildingLogic logic = GetLogicOrDefault(building.BuildingId);
            logic.BuildingInstance = building;
            logic.OnCreate();
        }

        public static void ProcessUpdate(Building building)
        {
            BuildingLogic logic = GetLogicOrDefault(building.BuildingId);
            logic.BuildingInstance = building;
            logic.OnUpdate();
        }

        public static void ProcessOnDestroy(Building building)
        {
            BuildingLogic logic = GetLogicOrDefault(building.BuildingId);
            logic.BuildingInstance = building;
            logic.OnDestroy();
        }
    }
}
