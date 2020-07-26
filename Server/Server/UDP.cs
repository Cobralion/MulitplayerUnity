using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Server
{
    public class UDP
    {
        public IPEndPoint endPoint;
        private int clientID;

        public UDP(int id)
        {
            clientID = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        public void SendData(Packet packet)
        {
            Server.SendUDPData(endPoint, packet);
        }

        public void HandleData(Packet packetData)
        {
            int packetLength = packetData.ReadInt();
            byte[] packetBytes = packetData.ReadBytes(packetLength);


            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetID = packet.ReadInt();
                    Server.packetHandlers[packetID](clientID, packet);
                }
            });
        }
    }
}
