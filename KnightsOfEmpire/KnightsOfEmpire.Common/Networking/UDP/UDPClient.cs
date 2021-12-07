using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace KnightsOfEmpire.Common.Networking.UDP
{
    /// <summary>
    /// UDP Client will only receive packets with updates. Sending will be done by TCPClient.
    /// </summary>
    public class UDPClient : UDPBase
    {
        private EndPoint SenderEndpoint = new IPEndPoint(IPAddress.Any, 0);
        public UDPClient(string address, int port) : base()
        {
            this.IPAddress = IPAddress.Parse(address);
            this.PortNumber = port;
        }

        public void Start()
        {
            if (isRunning) return;

            Console.WriteLine($"Setting up UDPClient on {IPAddress}:{PortNumber}...");
            try
            {
                Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
                Socket.Bind(new IPEndPoint(IPAddress, PortNumber));
            }
            catch(Exception ex)
            {
                Console.WriteLine("Cannot connect UDPClient!");
                Console.WriteLine(ex.ToString());
                return;
            }

            isRunning = true;
            Console.WriteLine($"UDP Client Running!");
            DataState state = new DataState(-1);
            Socket.BeginReceiveFrom(state.buffer, 0, DataState.BufferSize, SocketFlags.None, ref SenderEndpoint, new AsyncCallback(ReceiveCallback), state);
        } 
        
        public void Stop()
        {
            if (!isRunning) return;
            Console.WriteLine($"Stopping UDP Client...");
            try
            {
                Socket.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            isRunning = false;
            Console.WriteLine($"UDP Client stopped!");
        }

        protected void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                DataState so = (DataState)ar.AsyncState;
                int bytes = Socket.EndReceiveFrom(ar, ref SenderEndpoint);
                int bufferPacketStart = 0;
                int lastEofPos = 0;
                string content = Encoding.ASCII.GetString(so.buffer, 0, bytes);

                while (bufferPacketStart < content.Length && (lastEofPos = content.IndexOf(Packet.EOFTag, bufferPacketStart)) > -1)
                {
                    string packetContent = content.Substring(bufferPacketStart, lastEofPos - bufferPacketStart + Packet.EOFTag.Length);
                    bufferPacketStart = lastEofPos + Packet.EOFTag.Length;

                    ReceivedPacket packet = new ReceivedPacket(-1, packetContent);

                    ReceivedPackets.Enqueue(packet);

                }
                Socket.BeginReceiveFrom(so.buffer, 0, DataState.BufferSize, SocketFlags.None, ref SenderEndpoint, new AsyncCallback(ReceiveCallback), so);
                //Console.WriteLine($"Received {bytes} bytes from server.\n {r.GetContent()}");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex is SocketException) HandleSocketException((SocketException)ex);
            }
        }

        protected void HandleSocketException(SocketException ex)
        {
            
        }
    }
}
