using System.Collections;
using System.Collections.Generic;
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
    }
}
