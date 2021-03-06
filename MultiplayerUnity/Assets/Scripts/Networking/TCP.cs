﻿using System;
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
    private Packet receiveData;

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

        receiveData = new Packet();

        stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            int byteLenght = stream.EndRead(result);
            if (byteLenght <= 0)
            {
                Client.instance.Disconnect();
                return;
            }

            byte[] data = new byte[byteLenght];
            Array.Copy(receiveBuffer, data, byteLenght);

            receiveData.Reset(HandleData(data));

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Disconnect();
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
            if(packetLenght <= 0)
            {
                return true;
            }
        }

        while(packetLenght > 0 && packetLenght <= receiveData.UnreadLength())
        {
            byte[] packetBytes = receiveData.ReadBytes(packetLenght);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using(Packet packet = new Packet(packetBytes))
                {
                    int packetID = packet.ReadInt();
                    Client.packetHandlers[packetID](packet);
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
            Debug.Log($"Error sending data to Server via TCP: {ex}");
        }
    }

    private void Disconnect()
    {
        Client.instance.Disconnect();

        stream = null;
        receiveData = null;
        receiveBuffer = null;
        Socket = null;
    }
}
