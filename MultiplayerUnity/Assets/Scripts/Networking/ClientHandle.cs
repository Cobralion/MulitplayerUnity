using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string msg = packet.ReadString();
        int myID = packet.ReadInt();

        Debug.Log($"Message from Server: {msg}");
        Client.instance.clientID = myID;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.Socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotaion = packet.ReadQuaternion();

        GameManager.instance.SpawnPlayer(id, username, position, rotaion);
    }

    public static void PlayerPosition(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        GameManager.players[id].transform.position = position;
    }

    public static void PlayerRotation(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotaion = packet.ReadQuaternion();

        GameManager.players[id].transform.rotation = rotaion;
    }

    public static void CreateCube(Packet packet)
    {
        var id = packet.ReadInt();
        var pos = packet.ReadVector3();
        var rot = packet.ReadQuaternion();

        Debug.Log($"Creating cube at {pos} for client {id}");

        GameManager.instance.SpawnCube(id, pos, rot);
    }

    public static void DestroyCube(Packet packet)
    {
        var id = packet.ReadInt();

        Debug.Log($"Destroing cube for client {id}");

        GameManager.instance.DestroyCube(id);
    }
}
