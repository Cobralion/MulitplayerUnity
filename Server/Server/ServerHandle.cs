using System;
using System.Collections.Generic;
using System.Numerics;
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

        public static void PlayerMovement(int fromClient, Packet packet)
        {
            bool[] inputs = new bool[packet.ReadInt()];
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = packet.ReadBool();
            }

            Quaternion rotation = packet.ReadQuaternion();

            Server.Clients[fromClient].player.SetInput(inputs, rotation);
        }

        public static void RequestCube(int fromClient, Packet packet)
        {
            int id = packet.ReadInt();
            Vector3 position = packet.ReadVector3();

            Console.WriteLine($"Received cube request form {fromClient}");

            Server.Clients[fromClient].CreateCube(position);
        }
    }
}
