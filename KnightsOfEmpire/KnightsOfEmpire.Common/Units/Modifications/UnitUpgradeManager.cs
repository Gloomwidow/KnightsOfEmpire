using KnightsOfEmpire.Common.Helper;
using KnightsOfEmpire.Common.Units.Enum;
using KnightsOfEmpire.Common.Units.Modifications.Archetypes;
using KnightsOfEmpire.Common.Units.Modifications.Custom;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Units.Modifications
{
    public static class UnitUpgradeManager
    {
        public static int UnitUpgradesStartIndex = 1;
        public static int UnitUpgradesEndIndex = 11;


        public static readonly Dictionary<UnitType, int> ArchetypeUpgradesIds = new Dictionary<UnitType, int>
        {
            [UnitType.Melee] = 9001,
            [UnitType.Ranged] = 9002,
            [UnitType.Cavalry] = 9003,
            [UnitType.Siege] = 9004
        };

        public static Dictionary<int, UnitUpgrade> UnitUpgrades = new Dictionary<int, UnitUpgrade>
        {
            [0] = new EmptyUnitUpgrade(),

            [1] = new AttackModification(),
            [2] = new AttackModification2(),
            [3] = new AttackModification3(),
            [4] = new AttackModification4(),
            [5] = new AttackModification5(),
            [6] = new SpeedModification(),
            [7] = new SpeedModification2(),
            [8] = new HealthModification(),
            [9] = new HealthModification2(),
            [10] = new ViewModification(),
            [11] = new ViewModification2(),

            [ArchetypeUpgradesIds[UnitType.Melee]] = new MeleeArchetypeUnitUpgrade(),
            [ArchetypeUpgradesIds[UnitType.Ranged]] = new RangedArchetypeUnitUpgrade(),
            [ArchetypeUpgradesIds[UnitType.Cavalry]] = new CavalryArchetypeUnitUpgrade(),
            [ArchetypeUpgradesIds[UnitType.Siege]] = new SiegeArchetypeUnitUpgrade(),
        };

        

        private static CustomUnits defaultUnits = new CustomUnits
        {
            Units = new CustomUnit[]
            {
                new CustomUnit(UnitType.Melee, 0, "Melee 1"),
                new CustomUnit(UnitType.Melee, 1, "Melee 2"),
                new CustomUnit(UnitType.Ranged, 0, "Ranged 1"),
                new CustomUnit(UnitType.Ranged, 1, "Ranged 2"),
                new CustomUnit(UnitType.Cavalry, 0, "Cavalry 1"),
                new CustomUnit(UnitType.Cavalry, 1, "Cavalry 2"),
                new CustomUnit(UnitType.Siege, 0, "Siege 1"),
                new CustomUnit(UnitType.Siege, 1, "Siege 2"),
            }
        };

        

        public static bool IsCustomUnitsValid(CustomUnits units)
        {
            // 1. Units count must less than Constants.MaxUnitPerPlayer
            // 2. Units upgrades count must be of length Constants.MaxUpgradesPerUnit (excluding Archetype Upgrade)
            // 3. Upgrades with declared ids must exists in UnitUpgrades
            // 4. Upgrades won't have archetype upgrades (those are handled by UnitType)

            if (units.Units.Length >= Constants.MaxUnitsPerPlayer) return false;
            foreach(CustomUnit unitInfo in units.Units)
            {
                if (unitInfo.UpgradeList.Length != Constants.MaxUpgradesPerUnit) return false;

                foreach(int id in unitInfo.UpgradeList)
                {
                    if (ArchetypeUpgradesIds.Values.Contains(id)) return false;
                    if (!UnitUpgrades.ContainsKey(id)) return false;
                }
            }
            
            return true;
        }

        public static CustomUnits LoadCustomUnitsFromFile()
        {
            CustomUnits customUnits = null;
            string directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            try
            {
                using (StreamReader rd = File.OpenText(directory + @"\custom_units.json"))
                {
                    string text = rd.ReadToEnd();
                    try
                    {
                        customUnits = JsonSerializer.Deserialize<CustomUnits>(text);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (customUnits != null) return customUnits;
            else return defaultUnits;
        }

        public static void SaveCustomUnitsToFile(CustomUnits customUnitsToSave)
        {
            string directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            using (StreamWriter wr = File.CreateText(directory + @"\custom_units.json"))
            {
                wr.Write(JsonSerializer.Serialize(customUnitsToSave));
            }
        } 

        public static Unit ProduceUnit(CustomUnit unitInfo)
        {
            Unit u = new Unit();
            u.Stats = new UnitStats();
            u.TextureId = unitInfo.TextureId;
            UnitUpgrades[ArchetypeUpgradesIds[unitInfo.UnitType]].Upgrade(u);

            foreach(int upgradeId in unitInfo.UpgradeList)
            {
                UnitUpgrades[upgradeId].Upgrade(u);
            }

            return u;
        }

        
    }
}
