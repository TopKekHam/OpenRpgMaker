using System.Numerics;

namespace SBEngine.Serialization;

public unsafe class BinarySerializer : IDataSerializer
{
    private Stream _stream;
    
    public BinarySerializer(Stream stream)
    {
        _stream = stream;
    }


    public void WriteInt(int value)
    {
        _stream.WriteUnmanaged(value);
    }

    public void WriteFloat(float value)
    {
        _stream.WriteUnmanaged(value);
    }

    public void WriteVector2(Vector2 value)
    {
        _stream.WriteUnmanaged(value);
    }

    public void WriteVector3(Vector3 value)
    {
        _stream.WriteUnmanaged(value);
    }

    public void WriteArraySize(int size)
    {
        _stream.WriteUnmanaged(size);
    }

    public void WriteUnmanagedArray<T>(T[] array) where T : unmanaged
    {
        _stream.WriteUnmanaged(array.Length);
        
        fixed (T* ptr = array)
        {
            _stream.WriteFromPointer(ptr, array.Length);
        }
    }

    public void WriteString(string value)
    {
        _stream.WriteUnmanaged(value.Length);
        
        fixed (char* ptr = value)
        {
            _stream.WriteFromPointer(ptr, value.Length);
        }
    }
}