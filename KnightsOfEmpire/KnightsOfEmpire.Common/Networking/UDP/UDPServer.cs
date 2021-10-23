using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KnightsOfEmpire.Common.Networking.UDP
{
    public class UDPServer : UDPBase
    {
        public UDPServer(string address, int port) : base()
        {
            IPAddress = IPAddress.Parse(address);
            PortNumber = port;
        }

        public void Start()
        {
            if (isRunning) return;
            Console.WriteLine($"Starting UDP Server on {IPAddress.ToString()}:{PortNumber}...");
            try
            {
                Socket.Connect(IPAddress, PortNumber);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Cannot start UDP Server!");
                Console.WriteLine(ex.ToString());
                return;
            }

            isRunning = true;
            Console.WriteLine("UDP Server Started!");
        }

        public void Stop()
        {
            if (!isRunning) return;
            Console.WriteLine($"Stopping UDP Server...");
            try
            {
                Socket.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            isRunning = false;
            Console.WriteLine("UDP Server Stopped!");
        }

        public void SentTo(SentPacket packet, IPEndPoint address, int clientID)
        {
            if (!isRunning) return;
            byte[] data = Encoding.ASCII.GetBytes(packet.GetContent());
            Socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, address, new AsyncCallback(SendCallback), new DataState(clientID));
        }

        protected void SendCallback(IAsyncResult ar)
        {
            try
            {
                DataState so = (DataState)ar.AsyncState;
                int bytes = Socket.EndSendTo(ar);
                Console.WriteLine($"Sent {bytes} bytes to client {so.ConnectionID}!");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

    }
}
