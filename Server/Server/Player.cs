using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server
{
    public class Player
    {
        public int id;
        public string username;

        public Vector3 position;
        public Quaternion rotation;

        public Player(int id, string username, Vector3 position)
        {
            this.id = id;
            this.username = username;
            this.position = position;
            this.rotation = Quaternion.Identity;
        }
    }
}
