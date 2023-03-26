using System.Numerics;

namespace SBEngine;

public class Camera2D
{
    public Vector2 Position = Vector2.Zero;
    public Vector2 Size = Vector2.One;
    public Vector2 Offset = Vector2.Zero;

    public Matrix4x4 GetMatrix4x4()
    {
        var view = Matrix4x4.CreateOrthographic(Size.X, Size.Y, 0.001f, 100f);
        var pos = Matrix4x4.CreateTranslation(new Vector3(-Position + Offset, 0));
        
        return pos * view;
    }

    public Space GetSpace()
    {
        return new Space()
        {
            From = Position + Offset - (Size / 2f),
            To = Position + Offset + (Size / 2f),
            AxisDirection = Vector2.One
        };
    }
}