using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class ServerSend
    {

        public static void Welcome(int toClient, string msg)
        {
            using(Packet packet = new Packet((int)ServerPackets.welcome))
            {
                packet.Write(msg);
                packet.Write(toClient);

                SendTCPData(toClient, packet);
            }
        }

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
                Server.Clients[i].tcp.SendData(packet);
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
    }
}
