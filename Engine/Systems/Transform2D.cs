using System.Numerics;

namespace SBEngine;

public struct Transform2D
{
    public Vector2 Position;
    public float Rotation;
    public Vector2 Scale;

    public Transform2D()
    {
        Position = Vector2.Zero;
        Rotation = 0;
        Scale = Vector2.One;
    }
    
    public Transform2D(Vector2 position)
    {
        Position = position;
        Rotation = 0;
        Scale = Vector2.One;
    }

    public Matrix4x4 GetMatrix4x4()
    {
        Matrix4x4 mat = Matrix4x4.CreateTranslation(Position.X, Position.Y, 0);
        mat *= Matrix4x4.CreateRotationZ(Rotation);
        mat *= Matrix4x4.CreateScale(Scale.X, Scale.Y, 1);

        return mat;
    }
}