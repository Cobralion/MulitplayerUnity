using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDP
{
    public UdpClient socket;
    public IPEndPoint endPoint;


    public UDP()
    {
        endPoint = new IPEndPoint(IPAddress.Parse(Client.instance.ip), Client.instance.port);
    }

    public void Connect(int localPort)
    {
        socket = new UdpClient(localPort);

        socket.Connect(endPoint);
        socket.BeginReceive(ReceiveCallback, null);

        using(Packet packet = new Packet())
        {
            SendData(packet);
        }
    }

    public void SendData(Packet packet)
    {
        try
        {
            packet.InsertInt(Client.instance.clientID);
            if(socket != null)
            {
                socket.BeginSend(packet.ToArray(), packet.Length(), null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"Error occoured while sending over UDP: {ex}");
        }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
        try
        {
            byte[] data = socket.EndReceive(result, ref endPoint);
            socket.BeginReceive(ReceiveCallback, null);
            if (data.Length < 4)
            {
                Client.instance.Disconnect();
                return;
            }

            HandleData(data);
        }
        catch (Exception ex)
        {
            Disconnect();
            Debug.Log($"Error occoured: {ex}");
        }
    }

    private void HandleData(byte[] data)
    {
        using(Packet packet = new Packet(data))
        {
            int packetLenght = packet.ReadInt();
            data = packet.ReadBytes(packetLenght);
        }
        ThreadManager.ExecuteOnMainThread(() =>
        {
            using (Packet packet = new Packet(data))
            {
                int packetID = packet.ReadInt();
                Client.packetHandlers[packetID](packet);
            }
        });
    }

    private void Disconnect()
    {
        Client.instance.Disconnect();

        endPoint = null;
        socket = null;
    }
}
