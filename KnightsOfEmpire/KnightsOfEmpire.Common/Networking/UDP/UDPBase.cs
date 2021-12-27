using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using KnightsOfEmpire.Common.Resources;
using KnightsOfEmpire.Common.Extensions;

namespace KnightsOfEmpire.Common.Networking.UDP
{
    public class UDPBase
    {
        public IPAddress IPAddress { get; protected set; }
        public int PortNumber { get; protected set; }
        protected Socket Socket { get; set; }

        protected ConcurrentQueue<ReceivedPacket> ReceivedPackets;
        public bool isRunning { get; protected set; }

        public UDPBase()
        {
            ReceivedPackets = new ConcurrentQueue<ReceivedPacket>();
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

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
                        Console.WriteLine("An error occured while getting packets! Stopping retrieving and returning list.");
                        break;
                    }
                    received.Add(removedPacket);
                }
            }
            received.Sort((p1, p2) =>
            {
                return p1.ReceiveTime.CompareTo(p2.ReceiveTime);
            });
            return received;
        }
    }
}
