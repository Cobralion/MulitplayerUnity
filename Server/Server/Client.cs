using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server
{
    public class Client
    {
        public int ID
        {
            get;
            private set;
        }

        public Player player
        {
            get;
            private set;
        }

        public Cube Cube
        {
            get;
            private set;
        }

        public TCP tcp
        {
            get;
            private set;
        }

        public UDP udp
        {
            get;
            private set;
        }

        public Client(int clientID)
        {
            ID = clientID;
            tcp = new TCP(clientID);
            udp = new UDP(clientID);
        }

        public void SendIntoGame(string playerName)
        {
            player = new Player(ID, playerName, Vector3.Zero);

            foreach (Client client in Server.Clients.Values)
            {
                if (client.player != null && client.ID != ID)
                {
                    ServerSend.SpawnPlayer(ID, client.player);
                    if(client.Cube != null)
                        ServerSend.CreateCube(ID, client.Cube);
                }
            }

            foreach (Client client in Server.Clients.Values)
            {
                if (client.player != null)
                {
                    ServerSend.SpawnPlayer(client.ID, player);
                }
            }
        }

        public void CreateCube(Vector3 position)
        {
            if (Cube != null)
            {
                Console.WriteLine($"Cube {Cube.id} should be destroyed");
                ServerSend.DestroyCube(0, Cube);
            }

            Cube = new Cube(ID, position);

            //foreach (Client client in Server.Clients.Values)
            //{
            //    if (client.player != null && client.ID != ID)
            //        ServerSend.CreateCube(client.Cube);
            //}

            Console.WriteLine($"Created Cube {Cube.id}");
            ServerSend.CreateCube(Cube);
        }

        public void Disconnect()
        {
            Console.WriteLine($"{tcp.Socket.Client.RemoteEndPoint} has disconnected!");
            player = null;
            Cube = null;

            tcp.Disconnect();
            udp.Disconnect();
        }
    }
}
