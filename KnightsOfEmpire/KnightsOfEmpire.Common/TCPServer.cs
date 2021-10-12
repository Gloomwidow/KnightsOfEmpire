using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using KnightsOfEmpire.Common.Enums.Networking;
using System.Collections.Concurrent;
using System.Collections.Generic;

using KnightsOfEmpire.Common.Networking;

namespace KnightsOfEmpire.Common.Networking.TCP
{
    // https://docs.microsoft.com/pl-pl/dotnet/framework/network-programming/asynchronous-client-socket-example
    // https://docs.microsoft.com/pl-pl/dotnet/framework/network-programming/asynchronous-server-socket-example

    public class TCPDataState
    {
        public int ConnectionID;

        public const int BufferSize = 8192;

        public byte[] buffer;
        public StringBuilder sb;

        public TCPDataState(int connectionID)
        {
            ConnectionID = connectionID;
            buffer = new byte[BufferSize];
            sb = new StringBuilder();
        }
    }

    public class TCPServerConnection
    {
        public Socket socket;
        public DateTime lastMessageTime;
        public bool isEmpty = true;
    }

    public class TCPServer
    {     
        public int MaxClientMessageTimeout { get; protected set; } 

        public int MaxConnections { get; protected set; }

        public int CurrentActiveConnections { get; protected set; }

        public IPAddress IPAddress { get; protected set; }

        public int PortNumber { get; protected set; }

        protected TcpListener Listener;

        protected bool isRunning = false;

        protected TCPServerConnection[] Connections;

        protected Task ConnectionTask;

        protected ASCIIEncoding Encoder;

        protected ConcurrentBag<ReceivedPacket> ReceivedPackets;

        public TCPServer(string address, int port, int maxClients, int messageTimeout)
        {
            IPAddress = IPAddress.Parse(address);
            PortNumber = port;
            MaxConnections = maxClients;
            MaxClientMessageTimeout = messageTimeout;
            Connections = new TCPServerConnection[MaxConnections];
            Encoder = new ASCIIEncoding();
            ReceivedPackets = new ConcurrentBag<ReceivedPacket>();
        }

        public void Start()
        {
            if (isRunning) return;

            Console.WriteLine("Initializing TCP Server...");
            try
            {
                Listener = new TcpListener(IPAddress, PortNumber);
            }
            catch (SocketException exception)
            {
                Console.WriteLine("Exception occured while starting TCP Listener!");
                Console.WriteLine($"Error Code:{exception.ErrorCode}");
                Console.WriteLine($"Socket Error Code:{exception.SocketErrorCode}");
                Console.WriteLine($"{exception.Message}");
                Console.WriteLine($"{exception.StackTrace}");
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
                    if (Listener.Pending())
                    {
                        Socket socket = Listener.AcceptSocket();
                        Console.WriteLine("Connection request received from:" + socket.RemoteEndPoint);

                        bool hasBeenAccepted = false;

                        for(int i=0;i<MaxConnections;i++)
                        {
                            if(Connections[i].isEmpty)
                            {
                                Console.WriteLine($"Accepting this client at position {i}!");
                                Connections[i].isEmpty = false;
                                Connections[i].socket = socket;
                                Connections[i].lastMessageTime = DateTime.Now();
                            }
                        }
                    }
                }
            });

        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            TCPDataState state = (StateObject)ar.AsyncState;
            Socket sender = Connections[state.ConnectionID];
            Console.WriteLine($"Receiving data from client {state.ConnectionID}");
           
            int bytesRead = sender.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                content = state.sb.ToString();

                if (content.IndexOf(Packet.EOFTag) > -1)
                {
                    Console.WriteLine($"Read {content.Length} bytes from {handler.RemoteEndPoint}. \n Data : {content}");


                    StateObject newState = new StateObject();
                    newState.workSocket = state.workSocket;
                    handler.BeginReceive(newState.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), newState);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }





    }
}

