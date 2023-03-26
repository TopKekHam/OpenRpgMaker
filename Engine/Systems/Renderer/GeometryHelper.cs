using System.Numerics;
using ThirdParty.OpenGL;

namespace SBEngine;

internal static unsafe class GeometryHelper
{

    /// <param name="buffer">buffer with min size of 6 vertices</param>
    public static void CalcVerticesForQuad(QuadVertex* buffer, Vector3 position, Vector2 size, Vector4 color,
        float sprite = -1)
    {
        Vector2 halfSize = size / 2;

        //left top
        buffer[0] = new QuadVertex()
        {
            Position = position + new Vector3(-halfSize.X, halfSize.Y, 0),
            UV = new Vector2(0, 1)
        };

        //right top
        buffer[1] = new QuadVertex()
        {
            Position = position + new Vector3(halfSize.X, halfSize.Y, 0),
            UV = new Vector2(1, 1)
        };

        //right bottom
        buffer[2] = new QuadVertex()
        {
            Position = position + new Vector3(halfSize.X, -halfSize.Y, 0),
            UV = new Vector2(1, 0)
        };

        //left top
        buffer[3] = new QuadVertex()
        {
            Position = position + new Vector3(-halfSize.X, halfSize.Y, 0),
            UV = new Vector2(0, 1)
        };

        //right bottom
        buffer[4] = new QuadVertex()
        {
            Position = position + new Vector3(halfSize.X, -halfSize.Y, 0),
            UV = new Vector2(1, 0)
        };

        //left bottom
        buffer[5] = new QuadVertex()
        {
            Position = position + new Vector3(-halfSize.X, -halfSize.Y, 0),
            UV = new Vector2(0, 0)
        };
        
        for (int i = 0; i < 6; i++)
        {
            buffer[i].Sprite = sprite;
            buffer[i].Color = color;
        }
    }

    public static void CalcVerticesForLineQuad(QuadVertex* buffer, Vector3 pos1, Vector3 pos2, float width, Vector4 color)
    {

        float length = (pos2 - pos1).Length();

        Vector3 dir = (pos2 - pos1) / length;
        Vector3 position = pos1 + (dir * (length / 2f));

        Vector2 halfSize = new Vector2(width, length) / 2f;

        var angle = MathF.Atan2(dir.X, dir.Y);
        
        //left top
        buffer[0] = new QuadVertex()
        {
            Position = position + new Vector3(RotateVec2(new Vector2(-halfSize.X, halfSize.Y), angle), 0),
            UV = new Vector2(0, 1)
        };

        //right top
        buffer[1] = new QuadVertex()
        {
            Position = position + new Vector3(RotateVec2(new Vector2(halfSize.X, halfSize.Y), angle), 0),
            UV = new Vector2(1, 1)
        };

        //right bottom
        buffer[2] = new QuadVertex()
        {
            Position = position + new Vector3(RotateVec2(new Vector2(halfSize.X, -halfSize.Y), angle), 0),
            UV = new Vector2(1, 0)
        };

        //left top
        buffer[3] = new QuadVertex()
        {
            Position = position + new Vector3(RotateVec2(new Vector2(-halfSize.X, halfSize.Y), angle), 0),
            UV = new Vector2(0, 1)
        };

        //right bottom
        buffer[4] = new QuadVertex()
        {
            Position = position + new Vector3(RotateVec2(new Vector2(halfSize.X, -halfSize.Y), angle), 0),
            UV = new Vector2(1, 0)
        };

        //left bottom
        buffer[5] = new QuadVertex()
        {
            Position = position + new Vector3(RotateVec2(new Vector2(-halfSize.X, -halfSize.Y), angle), 0),
            UV = new Vector2(0, 0)
        };
        
        for (int i = 0; i < 6; i++)
        {
            buffer[i].Sprite = -1;
            buffer[i].Color = color;
        }
    }

    static Vector2 RotateVec2(Vector2 pos, float angle)
    {
        float _cos = MathF.Cos(angle);
        float _sin = MathF.Sin (angle);
 
        float x = pos.X * _cos - pos.Y * _sin;
        float y = pos.X * _sin + pos.Y * _cos;

        return new Vector2(x, y);
    }
    
    /// <param name="buffer">buffer with min size of 6 vertices</param>
    public static void CalcVerticesForQuad(QuadVertex* buffer, ref Vector3 position, ref Vector2 size, ref Vector4 color,
        ref Vector2 uvBottomLeft, ref Vector2 uvTopRight, float sprite = -1)
    {
        Vector2 halfSize = size / 2;

        //left top
        buffer[0] = new QuadVertex()
        {
            Position = position + new Vector3(-halfSize.X, halfSize.Y, 0),
            UV = new Vector2(uvBottomLeft.X, uvTopRight.Y),
        };

        //right top
        buffer[1] = new QuadVertex()
        {
            Position = position + new Vector3(halfSize.X, halfSize.Y, 0),
            UV = uvTopRight
        };

        //right bottom
        buffer[2] = new QuadVertex()
        {
            Position = position + new Vector3(halfSize.X, -halfSize.Y, 0),
            UV = new Vector2(uvTopRight.X, uvBottomLeft.Y)
        };

        //left top
        buffer[3] = new QuadVertex()
        {
            Position = position + new Vector3(-halfSize.X, halfSize.Y, 0),
            UV = new Vector2(uvBottomLeft.X, uvTopRight.Y)
        };

        //right bottom
        buffer[4] = new QuadVertex()
        {
            Position = position + new Vector3(halfSize.X, -halfSize.Y, 0),
            UV = new Vector2(uvTopRight.X, uvBottomLeft.Y)
        };

        //left bottom
        buffer[5] = new QuadVertex()
        {
            Position = position + new Vector3(-halfSize.X, -halfSize.Y, 0),
            UV = uvBottomLeft
        };
        
        for (int i = 0; i < 6; i++)
        {
            buffer[i].Sprite = sprite;
            buffer[i].Color = color;
        }
    }
    
    public static void SetAttribSetupForQuad(GLVertexAttribSetup setup)
    {
        setup.AddAttrib(sizeof(Vector3));
        setup.AddAttrib(sizeof(Vector2));
        setup.AddAttrib(sizeof(Vector4));
        setup.AddAttrib(sizeof(float));
    }
}