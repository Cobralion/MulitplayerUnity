using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ServerHandle
    {
        public static void WelcomeReceived(int fromClient, Packet packet)
        {
            int clientIDCheck = packet.ReadInt();
            string username = packet.ReadString();

            Console.WriteLine($"{Server.Clients[fromClient].tcp.Socket.Client.RemoteEndPoint} connected succesfully and is now player {fromClient}.");
            if(fromClient != clientIDCheck)
            {
                Console.WriteLine($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({clientIDCheck})!");
            }

            Server.Clients[fromClient].SendIntoGame(username);
        }
    }
}
