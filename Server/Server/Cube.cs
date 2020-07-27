using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Server
{
    public class Cube
    {
        public int id;
        public Vector3 position;
        public Quaternion rotation;

        public Cube(int id, Vector3 pos)
        {
            this.id = id;
            this.position = pos;
            var random = new Random();
            rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitX, (float)random.NextDouble() * MathF.PI * 2);
        }
    }
}
