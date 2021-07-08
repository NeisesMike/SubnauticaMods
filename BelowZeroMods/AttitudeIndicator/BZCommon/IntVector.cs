using System;
using UnityEngine;

namespace BZCommon
{
    public struct IntVector : IEquatable<IntVector>
    {
        public int x;
        public int y;
        public int z;

        public int magnitude
        {
            get
            {
                return (int)Mathf.Sqrt(x * x + y * y + z * z);
            }
        }

        public IntVector(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public IntVector(Vector3 vector3)
        {
            x = (int)vector3.x;
            y = (int)vector3.y;
            z = (int)vector3.z;
        }

        public IntVector(string stringXYZ, char splitChar = ' ')
        {
            string[] numbers = stringXYZ.Split(splitChar);
            
            int.TryParse(numbers[0], out x);
            int.TryParse(numbers[1], out y);
            int.TryParse(numbers[2], out z);
        }

        public static implicit operator IntVector(Vector3 v)
        {
            return new IntVector((int)v.x, (int)v.y, (int)v.z);
        }

        public static implicit operator Vector3(IntVector v)
        {
            return new Vector3(v.x, v.y, v.z);
        }        

        public static int Distance(IntVector a, IntVector b)
        {
            return (a - b).magnitude;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static IntVector operator +(IntVector a, IntVector b)
        {
            return new IntVector(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static IntVector operator -(IntVector a, IntVector b)
        {
            return new IntVector(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static IntVector operator *(IntVector a, IntVector b)
        {
            return new IntVector(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        public static IntVector operator *(IntVector a, int b)
        {
            return new IntVector(a.x * b, a.y * b, a.z * b);
        }        

        public static bool operator ==(IntVector objA, IntVector objB)
        {
            if (ReferenceEquals(objA, objB))
            {
                return true;
            }

            return objA == null || objB == null ? false : objA.x == objB.x && objA.y == objB.y && objA.z == objB.z;
        }

        public static bool operator !=(IntVector objA, IntVector objB)
        {
            return !(objA == objB);
        }

        public bool Equals(IntVector other)
        {
            if (this == null)
            {
                return false;
            }

            return ReferenceEquals(this, other) ? true : x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public bool Equals(Vector3 vector3)
        {
            IntVector obj = new IntVector(vector3);

            if (this == null)
            {
                return false;
            }

            return ReferenceEquals(this, obj) ? true : x.Equals(obj.x) && y.Equals(obj.y) && z.Equals(obj.z);
        }

        public override bool Equals(object obj)
        {
            if (this == null)
            {
                return false;
            }

            return ReferenceEquals(this, obj) ? true : obj.GetType() == GetType() && Equals((IntVector)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = x.GetHashCode();
                hashCode = (hashCode * 397) ^ y.GetHashCode();
                hashCode = (hashCode * 397) ^ z.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{x} {y} {z}";
        }
    }
}
