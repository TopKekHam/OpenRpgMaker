using System.Numerics;

namespace SBEngine;

public struct Transform
{
    public static readonly Transform identity = new Transform()
    {
        position = Vector3.Zero,
        rotation = Quaternion.Identity,
        scale = Vector3.One
    };

    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;


    public Transform(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }

    public Matrix4x4 ToMatrix4x4()
    {
        Matrix4x4 mat = Matrix4x4.CreateFromQuaternion(rotation);
        mat = mat * Matrix4x4.CreateScale(1f);
        mat = mat * Matrix4x4.CreateTranslation(position);

        return mat;
    }
}
