using System.Numerics;

namespace SBEngine.Serialization;

public interface IDataSerializer
{
    void WriteInt(int value);
    void WriteFloat(float value);
    void WriteVector2(Vector2 value);
    void WriteVector3(Vector3 value);
    void WriteArraySize(int size);
    void WriteUnmanagedArray<T>(T[] array) where T : unmanaged;
    void WriteString(string value);
}