using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWorld
{
    public class clsPoint
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public clsPoint(float x, float y, float z = 0)
        {
            setPoint(x, y, z);
        }

        public void setPoint(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void add(clsVector Vector)
        {
            this.x += Vector.x;
            this.y += Vector.y;
            this.z += Vector.z;
        }
    }
}
