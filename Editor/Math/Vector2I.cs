using System.Numerics;

public struct Vector2I
{
    public static readonly Vector2I Zero = new Vector2I(0, 0);
    public static readonly Vector2I One = new Vector2I(1, 1);

    public int X, Y;

    public Vector2I(int value)
    {
        X = value;
        Y = value;
    }

    public Vector2I(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public Vector2I(Vector2 Vector2)
    {
        X = (int)MathF.Floor(Vector2.X);
        Y = (int)MathF.Floor(Vector2.Y);
    }

    public static Vector2I operator +(Vector2I vec1, Vector2I Vector2)
    {
        return new Vector2I(vec1.X + Vector2.X, vec1.Y + Vector2.Y);
    }

    public static Vector2I operator -(Vector2I vec1, Vector2I Vector2)
    {
        return new Vector2I(vec1.X - Vector2.X, vec1.Y - Vector2.Y);
    }

    public static Vector2 operator +(Vector2 vec1, Vector2I Vector2)
    {
        return new Vector2(vec1.X + Vector2.X, vec1.Y + Vector2.Y);
    }

    public static Vector2 operator +(Vector2I vec1, Vector2 Vector2)
    {
        return new Vector2(vec1.X + Vector2.X, vec1.Y + Vector2.Y);
    }

    public static Vector2I operator +(Vector2I vec1, int scalar)
    {
        return new Vector2I(vec1.X + scalar, vec1.Y + scalar);
    }

    public static Vector2I operator -(Vector2I vec1, int scalar)
    {
        return new Vector2I(vec1.X - scalar, vec1.Y - scalar);
    }

    public static Vector2I operator *(Vector2I vec1, int scalar)
    {
        return new Vector2I(vec1.X * scalar, vec1.Y * scalar);
    }

    public static Vector2I operator /(Vector2I vec1, int scalar)
    {
        return new Vector2I(vec1.X / scalar, vec1.Y / scalar);
    }

    public static Vector2I operator /(Vector2I vec1, Vector2I vec2)
    {
        return new Vector2I(vec1.X / vec2.X, vec1.Y / vec2.Y);
    }

    public static bool operator ==(Vector2I vec1, Vector2I Vector2)
    {
        return vec1.X == Vector2.X && vec1.Y == Vector2.Y;
    }

    public static bool operator !=(Vector2I vec1, Vector2I Vector2)
    {
        return !(vec1 == Vector2);
    }

    public static implicit operator Vector2I(Vector2 vec)
    {
        return new Vector2I((int)MathF.Floor(vec.X), (int)MathF.Floor(vec.Y));
    }

    public float Length()
    {
        return MathF.Sqrt((Y * Y) - (X * X));
    }

    public Vector3 ToVector3(float z = 0)
    {
        return new Vector3(X, Y, z);
    }

    public Vector2 Normalized()
    {
        return new Vector2(X, Y) / Length();
    }

    public Vector2I Clamp(int xfrom, int xto, int yfrom, int yto)
    {
        Vector2I new_vec = this;
        new_vec.X = Math.Clamp(X, xfrom, xto);
        new_vec.Y = Math.Clamp(Y, yfrom, yto);
        return new_vec;
    }

    public Vector2I Clamp(int from, int to)
    {
        Vector2I new_vec = this;
        new_vec.X = Math.Clamp(X, from, to);
        new_vec.Y = Math.Clamp(Y, from, to);
        return new_vec;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }

    public bool InRange(int xfrom, int xto, int yfrom, int yto)
    {
        return X >= xfrom && X <= xto &&
               Y >= yfrom && Y <= yto;
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}
