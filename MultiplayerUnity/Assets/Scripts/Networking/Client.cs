using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client instance;

    public string ip = "127.0.0.1";
    public int port = 4200;
    public int clientID = 0;
    public TCP tcp;
    public UDP udp;

    private bool isConnected = false;

    public delegate void PacketHandler(Packet packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    public void Awake()
    {
        if (instance == null)
            instance = this;
        else if(instance != this)
            Destroy(this);
    }

    public void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        InitClientData();
        isConnected = true;
        tcp.Connect();
    }

    private void InitClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.spawnPlayer, ClientHandle.SpawnPlayer },
            { (int)ServerPackets.playerPosition, ClientHandle.PlayerPosition },
            { (int)ServerPackets.playerRotation, ClientHandle.PlayerRotation },
        };
        Debug.Log("Initialized data");
    }

    public void Disconnect()
    {
        if(isConnected)
        {
            isConnected = false;
            tcp.Socket.Close();
            udp.socket.Close();

            Debug.Log("Disconnected from server");
        }
    }
}
