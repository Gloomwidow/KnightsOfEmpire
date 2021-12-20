using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Navigation;
using KnightsOfEmpire.Common.Units;

namespace KnightsOfEmpire.Common.Resources
{
    public class ServerResources
    {
        public static int StartGold = 50;
        public static int StartMaxCapacity = 10;
        public Map.Map Map { get; set; }

        public FlowFieldManager NavigationManager;

        public string[] Nicknames { get; set; }

        public CustomUnits[] CustomUnits { get; set; }

        public int[] GoldAmount { get; protected set; }

        public bool[] IsDefeated { get; protected set; }

        public int[] CurrentUnitsCapacity { get; protected set; }

        public int[] MaxUnitsCapacity { get; protected set; }
        public bool[] HasChanged { get; set; }

        public void DefeatPlayer(int playerId) 
        {
            IsDefeated[playerId] = true;
            HasChanged[playerId] = true;
        }
        public void IncreaseCapacity(int playerId, int amount) 
        {
            MaxUnitsCapacity[playerId] += amount;
            HasChanged[playerId] = true;
        }

        public void DecreaseCapacity(int playerId, int amount)
        {
            MaxUnitsCapacity[playerId] -= amount;
            HasChanged[playerId] = true;
        }

        public bool AddUnitToCapacity(int playerId, int amount) 
        {
            if(CurrentUnitsCapacity[playerId] + amount > MaxUnitsCapacity[playerId]) 
            {
                return false;
            }
            CurrentUnitsCapacity[playerId] += amount;
            HasChanged[playerId] = true;
            return true;
        }

        public void RemoveUnitFromCapacity(int playerId, int amount)
        {
            CurrentUnitsCapacity[playerId] -= amount;
            HasChanged[playerId] = true;
        }

        public void AddGold(int playerId, int amount)
        {
            GoldAmount[playerId] += amount;
            HasChanged[playerId] = true;
        }

        public bool UseGold(int playerId, int amount)
        {
            if (GoldAmount[playerId] < amount) return false;
            GoldAmount[playerId] -= amount;
            HasChanged[playerId] = true;
            return true;
        }

        public void Reset() 
        {
            Init(IsDefeated.Length);
        }
        public void Init(int maxPlayers) 
        {
            Nicknames = new string[maxPlayers];
            CustomUnits = new CustomUnits[maxPlayers];
            GoldAmount = new int[maxPlayers];
            IsDefeated = new bool[maxPlayers];
            CurrentUnitsCapacity = new int[maxPlayers];
            MaxUnitsCapacity = new int[maxPlayers];
            HasChanged = new bool[maxPlayers];
            Map = null;
            ResetPlayerGameInfo(maxPlayers);
        }

        public ServerResources(int maxPlayers)
        {
            Init(maxPlayers);
        }
        public void ResetPlayerGameInfo(int maxPlayers) 
        {
            for(int i = 0; i < maxPlayers; i++) 
            {
                GoldAmount[i] = StartGold;
                MaxUnitsCapacity[i] = StartMaxCapacity;
                CurrentUnitsCapacity[i] = 0;
                HasChanged[i] = true;
            }
        }
    }
}
