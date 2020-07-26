using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class TCP
    {
        public const int dataBufferSize = 4096;

        public TcpClient Socket
        {
            get;
            private set;
        }

        private NetworkStream stream;
        private byte[] receiveBuffer;
        private Packet receiveData;

        private readonly int id;

        public TCP(int id)
        {
            this.id = id;
        }

        public void Connect(TcpClient socket)
        {
            Socket = socket;
            Socket.SendBufferSize = dataBufferSize;
            Socket.ReceiveBufferSize = dataBufferSize;

            stream = Socket.GetStream();
            receiveData = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(id, "Welcome to the Server");
        }

        public void SendData(Packet packet)
        {
            try
            {
                if(Socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to send packet to {id} via TCP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLenght = stream.EndRead(result);
                if(byteLenght <= 0)
                {
                    //TODO: Disconnect
                    return;
                }

                byte[] data = new byte[byteLenght];
                Array.Copy(receiveBuffer, data, byteLenght);

                receiveData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reseiving TCP data: {ex}");
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLenght = 0;

            receiveData.SetBytes(data);

            if (receiveData.UnreadLength() >= 4)
            {
                packetLenght = receiveData.ReadInt();
                if (packetLenght <= 0)
                {
                    return true;
                }
            }

            while (packetLenght > 0 && packetLenght <= receiveData.UnreadLength())
            {
                byte[] packetBytes = receiveData.ReadBytes(packetLenght);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetID = packet.ReadInt();
                        Server.packetHandlers[packetID](id, packet);
                    }
                });

                packetLenght = 0;

                if (receiveData.UnreadLength() >= 4)
                {
                    packetLenght = receiveData.ReadInt();
                    if (packetLenght <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLenght <= 1)
                return true;
            return false;
        }
    }
}
