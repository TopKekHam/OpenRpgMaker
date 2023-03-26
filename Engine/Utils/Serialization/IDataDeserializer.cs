using System.Numerics;

namespace SBEngine.Serialization;

public interface IDataDeserializer
{
    int ReadInt();
    float ReadFloat();
    Vector2 ReadVector2();
    Vector3 ReadVector3();
    int ReadArraySize();
    T[] ReadUnmanagedArray<T>() where T : unmanaged;
    string ReadString();
}