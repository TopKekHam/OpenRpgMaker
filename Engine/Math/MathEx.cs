using System.Numerics;
using System.Runtime.CompilerServices;

namespace SBEngine;

[Flags]
public enum Alignment
{
    LEFT = 1,
    TOP = 2,
    RIGHT = 4,
    BOTTOM = 8,
    HCENTER = 16,
    VCENTER = 32,
    CENTER = HCENTER | VCENTER,
    ALL = LEFT | RIGHT | TOP | BOTTOM,
}

[Flags]
public enum BoxSide
{
    NONE = 0,
    LEFT = 1,
    TOP = 2,
    RIGHT = 4,
    BOTTOM = 8,
    VERTICAL = TOP | BOTTOM,
    HORIZONTAL = LEFT | RIGHT,
    ALL = LEFT | RIGHT | TOP | BOTTOM,
}

public unsafe static class MathEx
{

    public const float DTR = 3.14159274F / 180f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 XY(this Vector3 Vector3)
    {
        return new Vector2(Vector3.X, Vector3.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 XZ(this Vector3 Vector3)
    {
        return new Vector2(Vector3.X, Vector3.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 YZ(this Vector3 Vector3)
    {
        return new Vector2(Vector3.Y, Vector3.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Abs(this Vector2 vec)
    {
        return new Vector2(MathF.Abs(vec.X), MathF.Abs(vec.Y));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Cross(this Vector2 a, Vector2 b)
    {
        return a.X * b.Y - a.Y * b.X;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SmoothStep01(float value)
    {
        return value * value * (3 - 2 * value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp01(float value)
    {
        if (value > 1) return 1;
        if (value < 0) return 0;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Clamp(float value, float min, float max)
    {
        if (value > max) return max;
        if (value < min) return min;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Clamp(int value, int min, int max)
    {
        if (value > max) return max;
        if (value < min) return min;
        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
    {
        return new Vector2(
            Clamp(value.X, min.X, max.X),
            Clamp(value.Y, min.Y, max.Y)
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
    {
        return new Vector3(
            Clamp(value.X, min.X, max.X),
            Clamp(value.Y, min.Y, max.Y),
            Clamp(value.Z, min.Z, max.Z)
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Map(this float value, float from, float to)
    {
        return from + (value * (to - from));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Map(this Vector2 value, Vector2 from, Vector2 to)
    {
        return from + (value * (to - from));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Map(this Vector3 value, Vector3 from, Vector3 to)
    {
        return from + (value * (to - from));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Normalize(this float value, float from, float to)
    {
        return (value - from) / (to - from);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Normalize(this Vector2 value, Vector2 from, Vector2 to)
    {
        return new Vector2((value.X - from.X) / (to.X - from.X), (value.Y - from.Y) / (to.Y - from.Y));
    }

    public static void MinMaxVector2(this Vector2 vec1, Vector2 Vector2, out Vector2 min, out Vector2 max)
    {
        min = new Vector2();
        max = new Vector2();

        min.X = MathF.Min(vec1.X, Vector2.X);
        min.Y = MathF.Min(vec1.Y, Vector2.Y);

        max.X = MathF.Max(vec1.X, Vector2.X);
        max.Y = MathF.Max(vec1.Y, Vector2.Y);
    }

    
    public static void MinMaxVector3(this Vector3 vec1, Vector3 Vector2, out Vector3 min, out Vector3 max)
    {
        min = new Vector3();
        max = new Vector3();

        min.X = MathF.Min(vec1.X, Vector2.X);
        min.Y = MathF.Min(vec1.Y, Vector2.Y);
        min.Z = MathF.Min(vec1.Z, Vector2.Z);

        max.X = MathF.Max(vec1.X, Vector2.X);
        max.Y = MathF.Max(vec1.Y, Vector2.Y);
        max.Z = MathF.Max(vec1.Z, Vector2.Z);
    }
    
    public static void Orbit(Vector3 pivot, Vector2 angles, ref Vector3 cameraPosition, ref Quaternion rotation)
    {
        var rotX = Quaternion.CreateFromAxisAngle(Vector3.UnitY, angles.X);
        var axisX = Vector3.Transform(Vector3.UnitX, rotX);
        var rotY = Quaternion.CreateFromAxisAngle(axisX, angles.Y);

        if (angles.X != 0)
        {
            cameraPosition = Vector3.Transform(cameraPosition,rotX);
        }

        if (angles.Y != 0)
        {
            cameraPosition = Vector3.Transform(cameraPosition, rotY);
        }

        Console.WriteLine(cameraPosition);

        var rotMat = Matrix4x4.CreateLookAt(pivot, cameraPosition, Vector3.UnitY);

        rotation = Quaternion.CreateFromRotationMatrix(rotMat);
    }

    public static Vector2 ToDir(this BoxSide sides)
    {
        Vector2 dir = Vector2.Zero;

        if (sides == BoxSide.LEFT)
        {
            dir.X = -1;
        }

        if (sides == BoxSide.RIGHT)
        {
            dir.X = 1;
        }

        if (sides == BoxSide.TOP)
        {
            dir.Y = 1;
        }

        if (sides == BoxSide.BOTTOM)
        {
            dir.Y = -1;
        }

        return dir;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Quantize(this float f, float size)
    {
        return MathF.Round(f / size) * size;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Quantize(this Vector2 vec, float size)
    {
        vec.X = MathF.Round(vec.X / size) * size;
        vec.Y = MathF.Round(vec.Y / size) * size;

        return vec;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Quantize(this Vector3 vec, float size)
    {
        vec.X = MathF.Round(vec.X / size) * size;
        vec.Y = MathF.Round(vec.Y / size) * size;
        vec.Z = MathF.Round(vec.Z / size) * size;

        return vec;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int GridPositionToLocal(this int number, int chunkSize)
    {
        if (number < 0)
        {
            int num = number % chunkSize;

            if (num == 0) return 0;

            return chunkSize + num;
        }
        else
        {
            return number % chunkSize;
        }
    }

    public static Vector2I GridPositionToLocal(this Vector2I vec, int chunkSize)
    {
        return new Vector2I(vec.X.GridPositionToLocal(chunkSize), vec.Y.GridPositionToLocal(chunkSize));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Fract(float value) { return value - MathF.Truncate(value); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Rand(float n) { return Fract(MathF.Sin(n) * 43758.5453123f); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Mix(float x, float y, float a) { return x * (1 - a) + y * a; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SmoothStep(float edge0, float edge1, float a)
    {
        a = a * a * (3f - 2f * a);
        float x = edge0 + (a * (edge1 - edge0));
        return x;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Fract(Vector2 value) { return new Vector2(Fract(value.X), Fract(value.Y)); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Floor(Vector2 value) { return new Vector2(MathF.Floor(value.X), MathF.Floor(value.Y)); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot(Vector2 vec1, Vector2 Vector2) { return (vec1.X * Vector2.X) + (vec1.Y * Vector2.Y); }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Hash3(Vector2 p)
    {
        Vector3 q = new Vector3(Dot(p, new Vector2(127.1f, 311.7f)),
                                Dot(p, new Vector2(269.5f, 183.3f)),
                                Dot(p, new Vector2(419.2f, 371.9f)));

        return new Vector3(
            Fract(MathF.Sin(q.X) * 43758.5453f),
            Fract(MathF.Sin(q.Y) * 43758.5453f),
            Fract(MathF.Sin(q.Z) * 43758.5453f)
            );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Truncate(this Vector2 vec)
    {
        return new Vector2(MathF.Truncate(vec.X), MathF.Truncate(vec.Y));
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="a1">First item</param>
    /// <param name="n1">First item index to start the sum</param>
    /// <param name="s">Max sum</param>
    /// <param name="q">Mutiplier</param>
    /// <returns>n2 Sum that equels or less than s</returns>
    public static double GeometricProgressionFindN2(double a1, double n1, double s, double q)
    {
        double z = -(((s / a1) * (1.0 - q)) - Math.Pow(q, n1)) / q;
        return Math.Log(z, q);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a1">First item</param>
    /// <param name="n1">First item index to start the sum</param>
    /// <param name="n2">Last item index</param>
    /// <param name="q">Mutiplier</param>
    /// <returns>Sum of the progression</returns>
    public static double GeometricProgressionSum(double a1, double n1, double n2, double q)
    {
        return a1 * ((Math.Pow(q, n1) - Math.Pow(q, n2 + 1)) / (1 - q));
    }

    // angle in degrees
    public static Vector2 RotateRef(this ref Vector2 vec, float angle)
    {
        float b = angle * DTR;

        return new Vector2(
            (MathF.Cos(b) * vec.X) - (MathF.Sin(b) * vec.Y),
            (MathF.Sin(b) * vec.X) + (MathF.Cos(b) * vec.Y)
        );
    }

    public static Matrix4x4 CreateOrthographicLeftHanded(float left, float right, float top, float bottom, float near, float far)
    {
        Matrix4x4 mat = Matrix4x4.Identity;

        mat.M11 = 2 / (right - left);
        mat.M22 = 2 / (top - bottom);
        mat.M33 = -2 / (far - near);
        mat.M41 = -(right + left) / (right - left);
        mat.M42 = -(top + bottom) / (top - bottom);
        mat.M43 = -(far + near) / (far - near);

        return mat;
    }

    public static Matrix4x4 CreateOrthographicLeftHanded(float width, float height, float near, float far)
    {
        Matrix4x4 mat = Matrix4x4.Identity;

        float right = 2.0f/width;
        float left = -2.0f / width;
        float top = 2.0f / height;
        float bottom = -2.0f / height;

        mat.M11 = 2 / (right - left);
        mat.M22 = 2 / (top - bottom);
        mat.M33 = -2 / (far - near);
        mat.M41 = -(right + left) / (right - left);
        mat.M42 = -(top + bottom) / (top - bottom);
        mat.M43 = -(far + near) / (far - near);

        return mat;
    }
}