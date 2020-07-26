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
                if(client.player != null && client.ID != ID)
                    ServerSend.SpawnPlayer(ID, client.player);
            }

            foreach (Client client in Server.Clients.Values)
            {
                if (client.player != null)
                    ServerSend.SpawnPlayer(client.ID, player);
            }
        }
    }
}
