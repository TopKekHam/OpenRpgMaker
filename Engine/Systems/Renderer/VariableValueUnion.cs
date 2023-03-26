using System.Numerics;
using System.Runtime.InteropServices;
using ThirdParty.OpenGL;

namespace SBEngine;

[StructLayout(LayoutKind.Explicit)]
public struct VariableValueUnion
{
    [FieldOffset(0)] public float ValueFloat;
    [FieldOffset(0)] public Vector2 ValueVector2;
    [FieldOffset(0)] public Vector3 ValueVector3;
    [FieldOffset(0)] public Vector4 ValueVector4;
    [FieldOffset(0)] public int ValueTextureSlot;
    [FieldOffset(4)] public int ValueTextureIdentifier;
    [FieldOffset(0)] public Matrix4x4 ValueMatrix4;

    public void Set(GLShader shader, GLShaderVariableType type, int uniform)
    {
        switch (type)
        {
            case GLShaderVariableType.Float:
                shader.SetVector1(uniform, ValueFloat);
                break;
            case GLShaderVariableType.Vector2:
                shader.SetVector2(uniform, ValueVector2);
                break;
            case GLShaderVariableType.Vector3:
                shader.SetVector3(uniform, ValueVector3);
                break;
            case GLShaderVariableType.Vector4:
                shader.SetVector4(uniform, ValueVector4);
                break;
            case GLShaderVariableType.Texture2D:
                shader.SetTexture2D(uniform, ValueTextureSlot, ValueTextureIdentifier);
                break;
            case GLShaderVariableType.Matrix4:
                shader.SetMatrix4x4(uniform, ValueMatrix4);
                break;
            default:
            {
                throw new Exception($"Type {type} not implemented.");
            }
        }
    }

    public static VariableValueUnion GetDefaultValue(GLShaderVariableType type)
    {
        switch (type)
        {
            case GLShaderVariableType.Float: return new VariableValueUnion() { ValueFloat = 0 };
            case GLShaderVariableType.Vector2: return new VariableValueUnion() { ValueVector2 = Vector2.Zero };
            case GLShaderVariableType.Vector3: return new VariableValueUnion() { ValueVector3 = Vector3.Zero };
            case GLShaderVariableType.Vector4: return new VariableValueUnion() { ValueVector4 = Vector4.Zero };
            case GLShaderVariableType.Texture2D: return new VariableValueUnion() { ValueTextureIdentifier = 0 };
            case GLShaderVariableType.Matrix4: return new VariableValueUnion() { ValueMatrix4 = Matrix4x4.Identity };
        }

        throw new Exception($"Type {type} not implemented.");
    }

    public static VariableValueUnion As(object obj, GLShaderVariableType type)
    {
        switch (type)
        {
            case GLShaderVariableType.Float: return new VariableValueUnion() { ValueFloat = (float)obj };
            case GLShaderVariableType.Vector2: return new VariableValueUnion() { ValueVector2 = (Vector2)obj };
            case GLShaderVariableType.Vector3: return new VariableValueUnion() { ValueVector3 = (Vector3)obj };
            case GLShaderVariableType.Vector4: return new VariableValueUnion() { ValueVector4 = (Vector4)obj };
            case GLShaderVariableType.Texture2D: return new VariableValueUnion() { ValueTextureIdentifier = (int)obj };
            case GLShaderVariableType.Matrix4: return new VariableValueUnion() { ValueMatrix4 = (Matrix4x4)obj };
        }

        throw new Exception($"Type {type} not implemented.");
    }

    public static VariableValueUnion Color(Vector4 color)
    {
        return new VariableValueUnion() { ValueVector4 = color };
    }

    public static VariableValueUnion Texture(int textureSlot, GLTexture texture)
    {
        return new VariableValueUnion()
            { ValueTextureSlot = textureSlot, ValueTextureIdentifier = (int)texture.handle };
    }

    public static VariableValueUnion Texture(int textureSlot, int textureIdentifier)
    {
        return new VariableValueUnion() { ValueTextureSlot = textureSlot, ValueTextureIdentifier = textureIdentifier };
    }
}