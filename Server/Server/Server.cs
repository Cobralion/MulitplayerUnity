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

        private static TcpListener tcpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;
            tcpListener = new TcpListener(IPAddress.Any, Port);

            Console.WriteLine("Server starting...");

            InitializeServerData();

            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnnectCallback), null);

            Console.WriteLine($"Server started on port {Port}");
        }

        private static void TCPConnnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnnectCallback), null);
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

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }
        }
    }
}
