using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net;
using System.Collections.Concurrent;

namespace KnightsOfEmpire.Common.Networking.TCP
{
    //https://docs.microsoft.com/pl-pl/dotnet/framework/network-programming/asynchronous-client-socket-example
    public class TCPClient : TCPBase
    {

        protected Socket ServerSocket;

        public string LastError { get; private set; }

        public TCPClient(string address, int port)
        {
            ReceivedPackets = new ConcurrentQueue<ReceivedPacket>();
            IPAddress = IPAddress.Parse(address);
            PortNumber = port;
            isRunning = false;
        }

        /// <summary>
        /// Gets IP Address of this client. Used for setting up UDP Client.
        /// </summary>
        public IPEndPoint ClientAddress
        {
            get
            {
                return (IPEndPoint)ServerSocket.LocalEndPoint;
            }
        }

        public void Start()
        {
            if (isRunning) return;
            Console.WriteLine($"Connecting to server {IPAddress}:{PortNumber}...");
            try
            {
                ServerSocket = new Socket(IPAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
                ServerSocket.Connect(IPAddress, PortNumber);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex is SocketException) HandleSocketException((SocketException)ex);
                return;
            }

            isRunning = true;
            DataState state = new DataState(-1);
            ServerSocket.BeginReceive(state.buffer, 0, DataState.BufferSize, 0,
               new AsyncCallback(ReceiveCallback), state);

        }

        public void SendToServer(SentPacket packet)
        {
            if (!isRunning) return;

            byte[] byteData = Encoding.ASCII.GetBytes(packet.GetContent());
            try
            {
                ServerSocket.BeginSend(byteData, 0, byteData.Length, 0,
                    new AsyncCallback(SendCallback), null);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if(ex is SocketException) HandleSocketException((SocketException)ex);
            }
        }

        protected void SendCallback(IAsyncResult ar)
        {
            try
            {
                int bytesSent = ServerSocket.EndSend(ar);
                //Console.WriteLine($"Sent {bytesSent} bytes to server.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex is SocketException) HandleSocketException((SocketException)ex);
            }
        }

        protected void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;

                DataState state = (DataState)ar.AsyncState;
                Console.WriteLine($"Receiving data from server!");

                int bytesRead = ServerSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    content = state.sb.ToString();

                    int bufferPacketStart = 0;
                    int lastEofPos = 0;

                    while (bufferPacketStart < content.Length && (lastEofPos = content.IndexOf(Packet.EOFTag, bufferPacketStart)) > -1)
                    {
                        string packetContent = content.Substring(bufferPacketStart, lastEofPos - bufferPacketStart + Packet.EOFTag.Length);
                        bufferPacketStart = lastEofPos + Packet.EOFTag.Length;
                        Console.WriteLine($"Read {content.Length} bytes from server. \n Content staarts with: { content.Substring(0, Math.Min(content.Length, 100)) }");

                        ReceivedPacket packet = new ReceivedPacket(state.ConnectionID, packetContent);

                        ReceivedPackets.Enqueue(packet);

                        DataState newState = new DataState(state.ConnectionID);
                        ServerSocket.BeginReceive(newState.buffer, 0, DataState.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), newState);
                    }
                    if (!content.EndsWith(Packet.EOFTag))
                    {
                        ServerSocket.BeginReceive(state.buffer, 0, DataState.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex is SocketException) HandleSocketException((SocketException)ex);
            }
        }

        /// <summary>
        /// Stops TCP Client, closing active server connection.
        /// After this function is used, TCPClient class should be reinitialized with 'new' operator instead of using Start() again.
        /// </summary>
        public void Stop()
        {
            if (!isRunning) return;

            try
            {
                ServerSocket.Close();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            isRunning = false;
        }

        protected void HandleSocketException(SocketException ex)
        {
            if(ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                Console.WriteLine("Connection with server was closed unexpectedly (server is down?).");
                Console.WriteLine("Stopping TCPClient since connection was closed.");
                LastError = "Server closed connection!";
                Stop();
            }
            else if (ex.SocketErrorCode == SocketError.ConnectionRefused)
            {
                Console.WriteLine("Connection with server was refused (is server up or ports open?).");
                LastError = "Server refusal!";
                Stop();
            }
        }
    }
}
