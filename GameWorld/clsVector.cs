using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameWorld
{
    // vector has direction and magnitude
    public class clsVector
    {
        private clsPoint _vector = new clsPoint(0,0,0);

        public clsVector()
        {
            setVector(0,0,0);
        }

        public clsVector(float x, float y, float z)
        {
            setVector(x, y, z);
        }

        public clsVector(clsVector vector)
        {
            setVector(vector.x, vector.y, vector.z);
        }

        public clsVector(clsPoint from, clsPoint to)
        {
            setVector(from, to);
        }

        public void setVector(float x, float y, float z)
        {
            _vector.x = x;
            _vector.y = y;
            _vector.z = z;
        }

        public void setVector(clsVector vector)
        {
            setVector(vector.x, vector.y, vector.z);
        }

        // all vectors start from 0,0
        public void setVector(clsPoint from, clsPoint to)
        {
            _vector = new clsPoint(to.x - from.x, to.y - from.y, to.z - from.z);
        }

        public float x
        {
            get 
            {
                if (double.IsNaN(_vector.x)) _vector.x = 0;
                return _vector.x; 
            }
            set { _vector.x = value; }
        }

        public float y
        {
            get 
            {
                if (double.IsNaN(_vector.y)) _vector.y = 0;
                return _vector.y; 
            }
            set { _vector.y = value; }
        }

        public float z
        {
            get 
            {
                if (double.IsNaN(_vector.z)) _vector.z = 0;
                return _vector.z; 
            }
            set { _vector.z = value; }
        }

        public float magnitude
        {
            get
            {
                // return magnitude
                return (float)Math.Sqrt((this.x * this.x) + (this.y * this.y) + (this.z * this.z));
            }
            set
            {
                // multiple the normalized vector by the new magnitude
                clsVector nomalized = this.normalized;
                this.x = nomalized.x * value;
                this.y = nomalized.y * value;
                this.z = nomalized.z * value;
            }
        }

        // radians -3.14 to 3.14
        public float radians2D
        {
            get
            {
                return (float)Math.Atan2(this.y, this.x);
            }
            set
            {
                this.x = (float)Math.Cos(value);
                this.y = (float)Math.Sin(value);
            }
        }

        // degree -180 - 0 - 180
        public float degrees2D
        {
            get
            {
                return (this.radians2D * 57.2957795f);
            }
            set
            {
                this.radians2D = (value / 57.2957795f);
            }
        }

        // return a vector with the same direction but it magnitude is scaled the less then one.
        public clsVector normalized
        {
            get 
            {
                return new clsVector((this.x / this.magnitude), (this.y / this.magnitude), (this.z / this.magnitude));
            }
        }

        public static clsVector operator +(clsVector vectorA, clsVector vectorB)
        {
            return new clsVector(vectorA.x + vectorB.x, vectorA.y + vectorB.y, vectorA.z + vectorB.z);
        }

        public static clsVector operator -(clsVector vectorA, clsVector vectorB)
        {
            return new clsVector(vectorA.x - vectorB.x, vectorA.y - vectorB.y, vectorA.z - vectorB.z);
        }

        public static clsVector operator *(clsVector vectorA, clsVector vectorB)
        {
            return new clsVector(vectorA.x - vectorB.x, vectorA.y - vectorB.y, vectorA.z - vectorB.z);
        }

        public static clsVector operator -(clsVector vector)
        {
            return new clsVector(-vector.x,-vector.y, -vector.z);
        }

        public static clsVector operator +(clsVector vector)
        {
            return new clsVector(+vector.x, +vector.y, +vector.z);
        }

        public static bool operator <(clsVector vectorA, clsVector vectorB)
        {
            return vectorA.magnitude < vectorB.magnitude;
        }

        public static bool operator <=(clsVector vectorA, clsVector vectorB)
        {
            return vectorA.magnitude <= vectorB.magnitude;
        }

        public static bool operator >(clsVector vectorA, clsVector vectorB)
        {
            return vectorA.magnitude > vectorB.magnitude;
        }

        public static bool operator >=(clsVector vectorA, clsVector vectorB)
        {
            return vectorA.magnitude >= vectorB.magnitude;
        }

        public static bool operator ==(clsVector vectorA, clsVector vectorB)
        {
            return vectorA.magnitude == vectorB.magnitude;
        }

        public static bool operator !=(clsVector vectorA, clsVector vectorB)
        {
            return vectorA.magnitude != vectorB.magnitude;
        }

        public static clsVector operator /(clsVector vector, float divisor)
        {
            return new clsVector(vector.x / divisor, vector.y / divisor, vector.z / divisor);
        }

        public static clsVector operator *(clsVector vector, float divisor)
        {
            return new clsVector(vector.x * divisor, vector.y * divisor, vector.z * divisor);
        }

        // returns a vector that is perpendicular to the plane created by this and the passed vector
        public clsVector crossProduct(clsVector vectorA, clsVector vectorB)
        {
            return new clsVector(vectorA.y * vectorB.z - vectorA.z * vectorB.y, vectorA.z * vectorB.x - vectorA.x * vectorB.z, vectorA.x * vectorB.y - vectorA.y * vectorB.x);
        }

        public clsVector crossProduct(clsVector vector)
        {
            return this.crossProduct(this, vector);
        }

        public float dotProduct(clsVector vectorA, clsVector vectorB) 
        {
            return vectorA.x * vectorB.x + vectorA.y * vectorB.y + vectorA.z * vectorB.z;
        }

        public float dotProduct(clsVector vector)
        {
            return this.dotProduct(this, vector);
        }

        public clsVector reflection(clsVector vector)
        {
            // R = V - 2 * (V . N) * N
            clsVector n = vector.normalized; // n must be normalized
            return this - (n * (2 * this.dotProduct(n)));
        }

        public clsVector bisection2d(clsVector vector)
        {
            clsVector t = this.normalized;
            clsVector v = vector.normalized;

            if ((t.x == v.x) || (t.y == v.y))
            {
                // return the perpendicular of collinear vectors
                return perpendicular2d;
            }
            else return  (t + v).normalized;
        }

        public clsVector perpendicular2d
        {
            get
            {
            return new clsVector(this.y, -this.x, 0);
            }
        }

        public clsVector rotate2d(float degrees)
        {
            clsVector result = new clsVector(this.x, this.y, this.z);
            result.x = this.x * (float)Math.Cos(degrees) - this.y * (float)Math.Sin(degrees);
            result.y = this.x * (float)Math.Sin(degrees) - this.y * (float)Math.Cos(degrees);
            return result;
        }
    }
}
