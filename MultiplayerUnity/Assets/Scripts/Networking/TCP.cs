using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

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

    public void Connect()
    {
        Socket = new TcpClient
        {
            ReceiveBufferSize = dataBufferSize,
            SendBufferSize = dataBufferSize
        };

        receiveBuffer = new byte[dataBufferSize];
        Socket.BeginConnect(Client.instance.ip, Client.instance.port, ConnectCallback, Socket);
    }

    private void ConnectCallback(IAsyncResult result)
    {
        Socket.EndConnect(result);

        if (!Socket.Connected) return;
        stream = Socket.GetStream();
        Socket.BeginConnect(Client.instance.ip, Client.instance.port, ConnectCallback, null);
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int byteLenght = stream.EndRead(result);
            if (byteLenght <= 0)
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
