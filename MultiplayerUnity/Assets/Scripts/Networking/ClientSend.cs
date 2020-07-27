using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();
        Client.instance.udp.SendData(packet);
    }

    #region Packets
    public static void RequestCube(Vector3 vector3)
    {
        using (Packet packet = new Packet((int)ClientPackets.requestCube))
        {
            packet.Write(Client.instance.clientID);
            packet.Write(vector3);

            Debug.Log($"Sending request for cube for {Client.instance.clientID}");

            SendTCPData(packet);
        }
    }

    public static void WelcomeReceived()
    {
        using(Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.clientID);
            packet.Write(UIManager.instance.usernameFiled.text);

            SendTCPData(packet);
        }
    }

    internal static void PlayerMovement(bool[] inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(inputs.Length);
            foreach (var item in inputs)
            {
                packet.Write(item);
            }

            packet.Write(GameManager.players[Client.instance.clientID].transform.rotation);

            SendUDPData(packet);
        }
    }
    #endregion
}
