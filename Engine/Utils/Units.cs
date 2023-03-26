using System.Numerics;

namespace SBEngine;

public struct Space
{
    public Vector2 From;
    public Vector2 To;
    public Vector2 AxisDirection;

    public Vector2 PointToNormal(Vector2 vec)
    {
        vec = MathEx.Normalize(vec, From, To);

        var axis = new Vector2(AxisDirection.X == 1 ? 0 : 1f, AxisDirection.Y == 1 ? 0 : 1);
        
        vec = MathEx.Abs(axis - vec);
        
        return vec;
    }

    public Vector2 NormalToPoint(Vector2 vec)
    {
        var axis = new Vector2(AxisDirection.X == 1 ? 0 : 1f, AxisDirection.Y == 1 ? 0 : 1);
        vec = MathEx.Abs(axis - vec);
        vec = MathEx.Map(vec, From, To);

        return vec;
    }

    public Vector2 ScalarToNormal(Vector2 vec)
    {
        vec = MathEx.Normalize(vec, Vector2.Zero, To - From);
        vec = AxisDirection * vec;

        return vec;
    }
    
    public Vector2 NormalToScalar(Vector2 vec)
    {
        vec = AxisDirection * vec;
        vec = MathEx.Map(vec, Vector2.Zero, To - From);

        return vec;
    }
    
}

public static class Units
{
    public static Vector2 MovePointToSpace(this Vector2 value, Space fromSpace, Space toSpace)
    {
        var normal = fromSpace.PointToNormal(value);
        return toSpace.NormalToPoint(normal);
    }

    public static Vector2 MoveScalarToSpace(this Vector2 value, Space fromSpace, Space toSpace)
    {
        return toSpace.NormalToScalar(fromSpace.ScalarToNormal(value));
    }
    
}