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
        public static int StartGold = 1000;
        public static int StartMaxCapacity = 10;
        public Map.Map Map { get; set; }
        public int PlayersLeft { get; set; }

        public int StartPlayerCount { get; set; }

        public FlowFieldManager NavigationManager;

        public string[] Nicknames { get; set; }

        public CustomUnits[] GameCustomUnits { get; set; }

        public int[] GoldAmount { get; protected set; }

        public bool[] IsDefeated { get; protected set; }
        public bool[] IsDefeatDone { get; protected set; }

        public int[] CurrentUnitsCapacity { get; protected set; }

        public int[] MaxUnitsCapacity { get; protected set; }
        public bool[] HasChanged { get; set; }

        public Map.Map[] MapsPool;

        public int CurrentMapId = 0;

        public void DefeatPlayer(int playerId, int maxPlayers) 
        {
            IsDefeated[playerId] = true;
            for(int i = 0; i < maxPlayers; i++) 
            {
                HasChanged[i] = true;
            }
            PlayersLeft--;
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

        public bool AddUnit(int playerId, int cost, int capacity)
        {
            if (!UseGold(playerId, cost)) return false;
            if(!AddUnitToCapacity(playerId,capacity))
            {
                AddGold(playerId, cost);
                return false;
            }
            return true;
        }

        public void Reset() 
        {
            Init(IsDefeated.Length);
            for (int i = 0; i < MapsPool.Length; i++)
            {
                MapsPool[i] = new Map.Map(MapsPool[i].MapName);
            }
        }
        public void Init(int maxPlayers) 
        {
            Nicknames = new string[maxPlayers];
            GameCustomUnits = new CustomUnits[maxPlayers];
            GoldAmount = new int[maxPlayers];
            IsDefeated = new bool[maxPlayers];
            IsDefeatDone = new bool[maxPlayers];
            CurrentUnitsCapacity = new int[maxPlayers];
            MaxUnitsCapacity = new int[maxPlayers];
            HasChanged = new bool[maxPlayers];
            PlayersLeft = 0;
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
                IsDefeated[i] = false;
                IsDefeatDone[i] = false;
                HasChanged[i] = true;
            }
        }
    }
}
