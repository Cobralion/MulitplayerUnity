using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class GameLogic
    {
        public static void Update()
        {
            foreach (Client client in Server.Clients.Values)
            {
                if(client.player != null)
                {
                    client.player.Update();
                }
            }

            ThreadManager.UpdateMain();
        }
    }
}
