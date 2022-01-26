using KnightsOfEmpire.Common.Helper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Networking.TCP
{
    public abstract class TCPBase
    {
        public IPAddress IPAddress { get; protected set; }

        public int PortNumber { get; protected set; }

        protected ConcurrentQueue<ReceivedPacket> ReceivedPackets;
        public bool isRunning { get; protected set; }

        /// <summary>
        /// Gets all packets received so far by the server.
        /// Bear in mind that once you retrieve them, they will disappear and the only copy will be in returned list.
        /// </summary>
        /// <returns>A list of ReceivedPacket, in order of their arrival time (oldest to newest).</returns>
        public List<ReceivedPacket> GetReceivedPackets()
        {
            List<ReceivedPacket> received = new List<ReceivedPacket>();
            if (!isRunning) return received;

            lock (ReceivedPackets)
            {
                while (ReceivedPackets.Count > 0)
                {
                    ReceivedPacket removedPacket;
                    if (!ReceivedPackets.TryDequeue(out removedPacket))
                    {
                        Logger.Log("An error occured while getting packets! Stopping retrieving and returning list.");
                        break;
                    }
                    received.Add(removedPacket);
                }
            }
            return received;
        }
    }
}
