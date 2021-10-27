using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

using KnightsOfEmpire.Common.Networking;
using System.Threading;
using KnightsOfEmpire.Common.Resources.Waiting;
using System.Text.Json;

namespace KnightsOfEmpire.Common.Networking.TCP
{
    // https://docs.microsoft.com/pl-pl/dotnet/framework/network-programming/asynchronous-client-socket-example
    // https://docs.microsoft.com/pl-pl/dotnet/framework/network-programming/asynchronous-server-socket-example

    public class TCPServerConnection
    {
        public Socket socket;
        public DateTime lastMessageTime;
        public bool isEmpty = true;
    }

    public class TCPServer : TCPBase
    {     
        /// <summary>
        /// Timeout in seconds after clients without info sent will be disconnected.
        /// </summary>
        public int MaxClientMessageTimeout { get; protected set; } 

        /// <summary>
        /// Maximum amount of connections accepted by the server. Exceeding connections will be rejected with packet informing about it.
        /// </summary>
        public int MaxConnections { get; protected set; }

        /// <summary>
        /// Current active clients connected to server.
        /// </summary>
        public int CurrentActiveConnections { get; protected set; }

        protected TcpListener Listener;

        protected TCPServerConnection[] Connections;

        protected Task ConnectionTask;

        protected Task TimeoutKickTask;

        public TCPServer(string address, int port, int maxClients, int messageTimeout)
        {
            IPAddress = IPAddress.Parse(address);
            PortNumber = port;
            CurrentActiveConnections = 0;
            isRunning = false;
            MaxConnections = maxClients;
            MaxClientMessageTimeout = messageTimeout;
            Connections = new TCPServerConnection[MaxConnections];
            for(int i=0;i<MaxConnections;i++)
            {
                Connections[i] = new TCPServerConnection();
            }
            ReceivedPackets = new ConcurrentQueue<ReceivedPacket>();
        }

        /// <summary>
        /// Start TCP Server, allowing client to connect and exchange data.
        /// </summary>
        public void Start()
        {
            if (isRunning) return;

            Console.WriteLine("Initializing TCP Server...");
            try
            {
                Listener = new TcpListener(IPAddress, PortNumber);
                Listener.Start();
            }
            catch (SocketException exception)
            {
                Console.WriteLine("Exception occured while starting TCP Listener!");
                Console.WriteLine(exception.ToString());
                return;
            }
            Console.WriteLine("TCP Listener is running!");
            Console.WriteLine($"Address: {Listener.LocalEndpoint}");
            Console.WriteLine("Starting connection thread...");

            isRunning = true;

            ConnectionTask = new Task(() =>
            {
                while(isRunning)
                {
                    Socket socket = Listener.AcceptSocket();
                    Console.WriteLine("Connection request received from:" + socket.RemoteEndPoint);
                    bool hasBeenAccepted = false;
                    for(int i=0;i<MaxConnections;i++)
                    {
                        if(Connections[i].isEmpty)
                        {
                            Console.WriteLine($"Accepting this client at position {i}!");
                            hasBeenAccepted = true;
                            Connections[i].isEmpty = false;
                            Connections[i].socket = socket;
                            Connections[i].lastMessageTime = DateTime.Now;
                            CurrentActiveConnections++;
                            DataState state = new DataState(i);
                            Connections[i].socket.BeginReceive(state.buffer, 0, DataState.BufferSize, 0,
                                new AsyncCallback(ReceiveCallback), state);

                            break;
                        }
                    }

                    if(!hasBeenAccepted)
                    {
                        Console.WriteLine("Max Connections Achieved! Acknowledging and closing connection with this client!");
                        WaitingStateServerResponse ServerFullResponse = new WaitingStateServerResponse
                        {
                            Message = WaitingMessage.ServerFull,
                        };
                        SentPacket serverFullPacket = new SentPacket(-1);
                        serverFullPacket.stringBuilder.Append(JsonSerializer.Serialize(ServerFullResponse));
                        socket.Send(Encoding.ASCII.GetBytes(serverFullPacket.GetContent()));
                        socket.Close();
                   }
                }
            });

            TimeoutKickTask = new Task(() =>
            {
                while (isRunning)
                {
                    for (int i = 0; i < MaxConnections; i++)
                    {
                        if (!Connections[i].isEmpty)
                        {
                            TimeSpan lastMessageTime = DateTime.Now - Connections[i].lastMessageTime;
                            if (lastMessageTime.TotalSeconds > MaxClientMessageTimeout)
                            {
                                Console.WriteLine($"Client {i} hasn't sent message for {MaxClientMessageTimeout} seconds. Disconnecting client {i} now!");
                                DisconnectClient(i);
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
            });

            ConnectionTask.Start();
            TimeoutKickTask.Start();

            Console.WriteLine("TCP Server is fully running!");
        }
        /// <summary>
        /// Stops TCP Server, closing any active connections.
        /// After this function is used, TCPServer classs should be reinitialized with 'new' operator instead of using Start() again.
        /// </summary>
        public void Stop()
        {
            if (!isRunning) return;

            Console.WriteLine("Stoppping TCP Server...");
            isRunning = false;
            ConnectionTask.Wait();
            TimeoutKickTask.Wait();

            for(int i=0;i<MaxConnections;i++)
            {
                if(!Connections[i].isEmpty)
                {
                    Connections[i].socket.Close();
                }
            }

            Listener.Stop();

            Console.WriteLine("Server successfully stop!");
        }

        /// <summary>
        /// Send prepared packet to currently connected client.
        /// </summary>
        /// <param name="packet"> Packet to send. ClientID param in packet should be filled with id of client we want send packet to.</param>
        public void SendToClient(SentPacket packet)
        {
            if (!isRunning) return;
            if (packet.ClientID < 0 || packet.ClientID >= MaxConnections) return;
            if (Connections[packet.ClientID].isEmpty) return;

            byte[] byteData = Encoding.ASCII.GetBytes(packet.GetContent());
  
            Connections[packet.ClientID].socket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), packet.ClientID);
        }

        public IPEndPoint GetClientAddress(int id)
        {
            if (!IsClientConnected(id)) return null;
            return (IPEndPoint)Connections[id].socket.RemoteEndPoint;
        }

        public bool IsClientConnected(int clientID)
        {
            return !Connections[clientID].isEmpty;
        }

        protected void SendCallback(IAsyncResult ar)
        {
            int receiverID = (int)ar.AsyncState;
            try
            {  
                int bytesSent = Connections[receiverID].socket.EndSend(ar);
                Console.WriteLine($"Sent {bytesSent} bytes to client {receiverID}.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex is SocketException) HandleSocketException((SocketException)ex, receiverID);
            }
        }

        protected void ReceiveCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            DataState state = (DataState)ar.AsyncState;
            Socket sender = Connections[state.ConnectionID].socket;
            Console.WriteLine($"Receiving data from client {state.ConnectionID}");

            try
            {
                

                int bytesRead = sender.EndReceive(ar);

                if (bytesRead > 0)
                {
                    Connections[state.ConnectionID].lastMessageTime = DateTime.Now;
                    state.sb.Append(Encoding.ASCII.GetString(
                        state.buffer, 0, bytesRead));

                    content = state.sb.ToString();

                    int bufferPacketStart = 0;
                    int lastEofPos = 0;

                    while (bufferPacketStart < content.Length && (lastEofPos = content.IndexOf(Packet.EOFTag, bufferPacketStart)) > -1)
                    {
                        string packetContent = content.Substring(bufferPacketStart, lastEofPos - bufferPacketStart + Packet.EOFTag.Length);
                        bufferPacketStart = lastEofPos + Packet.EOFTag.Length;
                        Console.WriteLine($"Read {content.Length} bytes from {sender.RemoteEndPoint}. \n Data : {packetContent}");

                        ReceivedPacket packet = new ReceivedPacket(state.ConnectionID, packetContent);

                        ReceivedPackets.Enqueue(packet);

                        DataState newState = new DataState(state.ConnectionID);
                        sender.BeginReceive(newState.buffer, 0, DataState.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), newState);
                    }
                    if(bufferPacketStart<content.Length)
                    {
                        sender.BeginReceive(state.buffer, 0, DataState.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), state);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (ex is SocketException) HandleSocketException((SocketException)ex, state.ConnectionID);
            }
        }

        public void DisconnectClient(int clientID)
        {
            Connections[clientID].socket.Disconnect(true);
            Connections[clientID].isEmpty = true;
            CurrentActiveConnections--;
        }

        protected void HandleSocketException(SocketException ex, int clientID)
        {
            if (ex.SocketErrorCode == SocketError.ConnectionReset)
            {
                Console.WriteLine($"Connection with client {clientID} was closed unexpectedly (client is down?).");
                Console.WriteLine("Disconnecting this client now...");
                DisconnectClient(clientID);
            }
        }
    }
}

