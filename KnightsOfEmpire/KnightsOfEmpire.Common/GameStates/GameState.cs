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

        protected List<GameState> GameStates;

        protected Dictionary<string, int> TCPPacketRedirects;
        protected Dictionary<string, int> UDPPacketRedirects;

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

        /// <summary>
        /// This function is called when all components are initialized.
        /// There any external dependencies from other GameStates should be referenced.
        /// </summary>
        public virtual void LoadDependencies()
        {
            foreach (GameState gs in GameStates)
            {
                gs.LoadDependencies();
            }
        }
        public virtual void HandleTCPPacket(ReceivedPacket packets) { }

        public virtual void HandleUDPPacket(ReceivedPacket packets) { }
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
        protected void RegisterGameState(GameState gs) 
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
        /// <summary>
        /// Registers redirects fro RedirectTCPPackets function
        /// </summary>
        /// <param name="redirects">
        /// Redirects constisting of header and type of class 
        /// Header can be either full header (4 digits) or start (2 digits)
        /// </param>
        protected void RegisterTCPRedirects((string Header, Type T)[] redirects)
        {
            if (TCPPacketRedirects == null)
            {
                TCPPacketRedirects = new Dictionary<string, int>();
                foreach ((string Header, Type T) redirect in redirects)
                {
                    int pos = GameStates.FindIndex(x => x.GetType() == redirect.T);
                    if (pos == -1)
                    {
                        throw new ArgumentOutOfRangeException($"GameState of type {redirect.T} not registered!");
                    }
                    TCPPacketRedirects.Add(redirect.Header, pos);
                }
            }
        }

        protected void RegisterUDPRedirects((string Header, Type T)[] redirects)
        {
            if (UDPPacketRedirects == null)
            {
                UDPPacketRedirects = new Dictionary<string, int>();
                foreach ((string Header, Type T) redirect in redirects)
                {
                    int pos = GameStates.FindIndex(x => x.GetType() == redirect.T);
                    if (pos == -1)
                    {
                        throw new ArgumentOutOfRangeException($"GameState of type {redirect.T} not registered!");
                    }
                    UDPPacketRedirects.Add(redirect.Header, pos);
                }
            }
        }

        protected void RedirectTCPPackets(List<ReceivedPacket> packets)
        {
            foreach (ReceivedPacket packet in packets)
            {
                string headerStart = packet.GetHeader().Substring(0, 2);
                int GameStatePosition;
                if (TCPPacketRedirects.TryGetValue(headerStart, out GameStatePosition))
                {
                    GameStates[GameStatePosition].HandleTCPPacket(packet);
                }
                if (TCPPacketRedirects.TryGetValue(packet.GetHeader(), out GameStatePosition))
                {
                    GameStates[GameStatePosition].HandleTCPPacket(packet);
                }
            }
        }
        protected void RedirectUDPPackets(List<ReceivedPacket> packets)
        {
            foreach (ReceivedPacket packet in packets)
            {
                string headerStart = packet.GetHeader().Substring(0, 2);
                int GameStatePosition;
                if (UDPPacketRedirects.TryGetValue(headerStart, out GameStatePosition))
                {
                    GameStates[GameStatePosition].HandleUDPPacket(packet);
                }
                if (UDPPacketRedirects.TryGetValue(packet.GetHeader(), out GameStatePosition))
                {
                    GameStates[GameStatePosition].HandleUDPPacket(packet);
                }
            }
        }

    }
}
