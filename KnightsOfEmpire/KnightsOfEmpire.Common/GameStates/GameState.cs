using KnightsOfEmpire.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.GameStates
{
    public abstract class GameState
    {
        public const int MaxPlayerCount = 4;

        protected GameState parent = null;

        protected GameState Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if(parent==null)
                {
                    parent = value;
                }
            }
        }

        protected List<GameState> GameStates;

        public GameState()
        {
            GameStates = new List<GameState>();
        }

        public virtual void LoadResources() 
        { 
            foreach(GameState gs in GameStates)
            {
                gs.LoadResources();
            }
        }
        public virtual void Initialize() 
        {
            foreach (GameState gs in GameStates)
            {
                gs.Initialize();
            }
        }

        public virtual void LoadDependencies()
        {
            foreach (GameState gs in GameStates)
            {
                gs.LoadDependencies();
            }
        }

        public virtual void HandleTCPPacket(ReceivedPacket packets) { }
        public virtual void HandleTCPPackets(List<ReceivedPacket> packets) 
        {
            foreach(ReceivedPacket packet in packets)
            {
                HandleTCPPacket(packet);
            }
        }
        public virtual void HandleUDPPackets(List<ReceivedPacket> packets) { }
        public virtual void Update() 
        {
            foreach (GameState gs in GameStates)
            {
                gs.Update();
            }
        }
        public virtual void Render() 
        {
            foreach (GameState gs in GameStates)
            {
                gs.Render();
            }
        }
        public virtual void Dispose() 
        {
            foreach (GameState gs in GameStates)
            {
                gs.Dispose();
            }
        }
        public void RegisterGameState(GameState gs) 
        {
            if(gs.Parent!=null)
            {
                throw new ArgumentOutOfRangeException("Tried to add already claimed game state to children!");
            }
            gs.Parent = this;
            GameStates.Add(gs);
        }

        public T GetSiblingGameState<T>() where T: GameState
        {
            var gs = GameStates.Find(x => x.GetType()==typeof(T));
            return gs as T;
        }

    }
}
