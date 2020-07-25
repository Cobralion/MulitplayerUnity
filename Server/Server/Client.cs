using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Client
    {
        public int ID
        {
            get;
            private set;
        }

        public TCP tcp
        {
            get;
            private set;
        }

        public Client(int clientID)
        {
            ID = clientID;
            tcp = new TCP(clientID);
        }
    }
}
