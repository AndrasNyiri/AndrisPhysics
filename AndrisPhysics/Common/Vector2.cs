using System;

namespace AndrisPhysics.Common
{
    class Vector2
    {
        private const double DEG_TO_RAD = Math.PI / 180;

        public static Vector2 zero = new Vector2(0, 0);
        public static Vector2 right = new Vector2(1, 0);
        public static Vector2 left = new Vector2(-1, 0);
        public static Vector2 up = new Vector2(0, 1);
        public static Vector2 down = new Vector2(0, -1);
        public static Vector2 upRight = new Vector2(1, 1);
        public static Vector2 upLeft = new Vector2(-1, 1);
        public static Vector2 downRight = new Vector2(1, -1);
        public static Vector2 downLeft = new Vector2(-1, -1);

        public float x;
        public float y;

        public float Magnitude => (float)Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

        public Vector2 UnitVector
        {
            get
            {
                float mag = Math.Abs(Magnitude);
                return mag > 0 ? new Vector2(x / mag, y / mag) : zero;
            }
        }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float Dot(Vector2 v)
        {
            float aDot = x * v.x + y * v.y;
            float mags = Magnitude * v.Magnitude;
            float angle = mags > 0 || mags < 0 ? aDot / mags : 0;
            return Magnitude * v.Magnitude * angle;
        }

        public static Vector2 Rotate(Vector2 v, double degrees)
        {
            return RotateRadians(v, degrees * DEG_TO_RAD);
        }

        public static Vector2 RotateRadians(Vector2 v, double radians)
        {
            float ca = Convert.ToSingle(Math.Cos(radians));
            float sa = Convert.ToSingle(Math.Sin(radians));
            return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }

        public static Vector2 operator *(Vector2 v1, float scalar)
        {
            return new Vector2(v1.x * scalar, v1.y * scalar);
        }

        public static Vector2 operator *(float scalar, Vector2 v1)
        {
            return new Vector2(v1.x * scalar, v1.y * scalar);
        }

        public static Vector2 operator /(Vector2 v1, float scalar)
        {
            return new Vector2(v1.x / scalar, v1.y / scalar);
        }

        public static float Distance(Vector2 v1, Vector2 v2)
        {
            return (float)Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y));
        }

    }
}
