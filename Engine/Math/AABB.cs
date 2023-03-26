using System.Numerics;

namespace SBEngine;

public struct AABB
{
    public Vector2 center;
    public Vector2 half_size;
    public Vector2 top_left => center - half_size;
    public Vector2 bottom_right => center + half_size;

    public float right => center.X + half_size.X;
    public float left => center.X - half_size.X;
    public float top => center.Y + half_size.Y;
    public float bottom => center.Y - half_size.Y;

    public AABB(Vector2 _center, Vector2 _half_size)
    {
        center = _center;
        half_size = _half_size;
    }

    public bool ContainsPoint(Vector2 point)
    {
        if (MathF.Abs(center.X - point.X) > half_size.X) return false;
        if (MathF.Abs(center.Y - point.Y) > half_size.Y) return false;

        return true;
    }
}
