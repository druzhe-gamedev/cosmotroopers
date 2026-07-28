using System;

namespace CodeBase.Core.Mathematics
{
    public struct Vector2Int
    {
        public int X { get; set; }
        public int Y { get; set; }

        public static Vector2Int Zero = new (0, 0);
        public static Vector2Int One = new (1, 1);
        
        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{{x: {X}; y: {Y}}}";

        public float Length => MathF.Sqrt(X * X + Y * Y);

        public Vector2Int Normalized() => new((int)(X / Length), (int)(Y / Length)); 
        public static Vector2Int operator +(Vector2Int lhs, Vector2Int rhs) => new(lhs.X + rhs.X, lhs.Y + rhs.Y);
        public static Vector2Int operator -(Vector2Int lhs, Vector2Int rhs) => new(lhs.X - rhs.X, lhs.Y - rhs.Y);
        public static Vector2Int operator *(Vector2Int lhs, Vector2Int rhs) => new(lhs.X * rhs.X, lhs.Y * rhs.Y);
        public static Vector2Int operator /(Vector2Int lhs, Vector2Int rhs) => new(lhs.X / rhs.X, lhs.Y / rhs.Y);
        public static bool operator > (Vector2Int lhs, Vector2Int rhs) => lhs.X > rhs.X && lhs.Y > rhs.Y;
        public static bool operator < (Vector2Int lhs, Vector2Int rhs) => lhs.X < rhs.X && lhs.Y < rhs.Y;
        public static bool operator >= (Vector2Int lhs, Vector2Int rhs) => lhs.X >= rhs.X && lhs.Y >= rhs.Y;
        public static bool operator <= (Vector2Int lhs, Vector2Int rhs) => lhs.X <= rhs.X && lhs.Y <= rhs.Y;
        
        public static Vector2Int operator *(Vector2Int lhs, int rhs) => new(lhs.X * rhs, lhs.Y * rhs);
        public static Vector2Int operator /(Vector2Int lhs, int rhs) => new(lhs.X / rhs, lhs.Y / rhs);
    }

    public static class Vector2IntExtensions
    {
        public static Vector2Int WithX(this Vector2Int v, int x) => new(x, v.Y);
        public static Vector2Int WithY(this Vector2Int v, int y) => new(v.X, y);

        public static float Dot(this Vector2Int v1, Vector2Int v2) =>
            (v1.X * v2.X + v1.Y * v2.Y) / (v1.Length * v2.Length);
    }
}