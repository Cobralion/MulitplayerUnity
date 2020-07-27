using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Server
    {
        public static int MaxPlayers
        {
            get;
            set;
        }

        public static int Port
        {
            get;
            set;
        }

        private static Dictionary<int, Client> clients;

        public static Dictionary<int, Client> Clients
        {
            get => clients ?? (clients = new Dictionary<int, Client>());
        }

        public delegate void PacketHandler(int fromClient, Packet packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;
            tcpListener = new TcpListener(IPAddress.Any, Port);

            Console.WriteLine("Server starting...");

            InitializeServerData();

            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(TCPConnnectCallback, null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"Server started on port {Port}");
        }

        private static void TCPConnnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(TCPConnnectCallback, null);
            Console.WriteLine($"Incomming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if(Clients[i].tcp.Socket == null)
                {
                    Clients[i].tcp.Connect(client);
                    return;
                }
            }

            Console.WriteLine("Server is full");
        }

        private static void UDPReceiveCallback(IAsyncResult result)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(result, ref clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4) return;

                using(Packet packet = new Packet(data))
                {
                    int clientID = packet.ReadInt();
                    if (clientID == 0) return;

                    if(clients[clientID].udp.endPoint == null)
                    {
                        clients[clientID].udp.Connect(clientEndPoint);
                        return;
                    }

                    if(clients[clientID].udp.endPoint.ToString() == clientEndPoint.ToString())
                    {
                        clients[clientID].udp.HandleData(packet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex}");
            }
        }

        public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
        {
            try
            {
                if (clientEndPoint != null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending UDP data to {clientEndPoint}: {ex}");
            }
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                { (int)ClientPackets.requestCube, ServerHandle.RequestCube },
            };

            Console.WriteLine("Inited server data");
        }
    }
}
