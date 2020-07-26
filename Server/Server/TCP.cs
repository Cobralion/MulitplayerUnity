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
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reseiving TCP data: {ex}");
            }
        }
    }
}
