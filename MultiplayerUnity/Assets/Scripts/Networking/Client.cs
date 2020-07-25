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
    }

    public void ConnectToServer()
    {
        tcp.Connect();
    }
}
