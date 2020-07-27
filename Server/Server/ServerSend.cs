using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ServerSend
    {
        #region Packets
        public static void Welcome(int toClient, string msg)
        {
            using(Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(msg);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

        public static void SpawnPlayer(int toClient, Player player)
        {
            using(Packet packet = new Packet((int)ServerPackets.spawnPlayer))
            {
                packet.Write(player.id);
                packet.Write(player.username);
                packet.Write(player.position);
                packet.Write(player.rotation);

                SendTCPData(toClient, packet);
            }
        }

        public static void PlayerPosition(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerPosition))
            {
                packet.Write(player.id);
                packet.Write(player.position);

                SendUDPDataToAll(packet);
            }
        }
        public static void PlayerRotation(Player player)
        {
            using (Packet packet = new Packet((int)ServerPackets.playerRotation))
            {
                packet.Write(player.id);
                packet.Write(player.rotation);

                SendUDPDataToAll(packet, player.id);
            }
        }

        public static void CreateCube(Cube cube)
        {
            using (Packet packet = new Packet((int)ServerPackets.createCube))
            {
                packet.Write(cube.id);
                packet.Write(cube.position);
                packet.Write(cube.rotation);

                SendTCPDataToAll(packet);
            }
        }

        public static void CreateCube(int toClient, Cube cube)
        {
            using (Packet packet = new Packet((int)ServerPackets.createCube))
            {
                packet.Write(cube.id);
                packet.Write(cube.position);
                packet.Write(cube.rotation);

                SendTCPData(toClient, packet);
            }
        }

        public static void DestroyCube(int toClient, Cube cube)
        {
           using(Packet packet = new Packet((int)ServerPackets.destroyCube))
            {
                packet.Write(cube.id);

                SendTCPDataToAll(packet);
            }
        }
        #endregion

        private static void SendTCPData(int toClient, Packet packet)
        {
            packet.WriteLength();

            Server.Clients[toClient].tcp.SendData(packet);
        }


        private static void SendTCPDataToAll(Packet packet)
        {
            packet.WriteLength();

            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.Clients[i].tcp?.SendData(packet);
            }
        }


        private static void SendTCPDataToAll(Packet packet, int except)
        {
            packet.WriteLength();

            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if (i == except) continue;

                Server.Clients[i].tcp.SendData(packet);
            }
        }

        private static void SendUDPData(int toClient, Packet packet)
        {
            packet.WriteLength();

            Server.Clients[toClient].udp.SendData(packet);
        }

        private static void SendUDPDataToAll(Packet packet)
        {
            packet.WriteLength();

            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                Server.Clients[i].udp.SendData(packet);
            }
        }

        private static void SendUDPDataToAll(Packet packet, int except)
        {
            packet.WriteLength();

            for (int i = 1; i < Server.MaxPlayers; i++)
            {
                if (i == except) continue;

                Server.Clients[i].udp.SendData(packet);
            }
        }

    }
}
