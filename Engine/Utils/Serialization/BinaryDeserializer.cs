using System.Numerics;

namespace SBEngine.Serialization;

public unsafe class BinaryDeserializer : IDataDeserializer
{
    private Stream _stream;
    
    public BinaryDeserializer(Stream stream)
    {
        _stream = stream;
    }

    public int ReadInt()
    {
        int value;
        _stream.ReadUnmanaged(&value);
        return value;
    }

    public float ReadFloat()
    {
        float value;
        _stream.ReadUnmanaged(&value);
        return value;
    }

    public Vector2 ReadVector2()
    {
        Vector2 value;
        _stream.ReadUnmanaged(&value);
        return value;
    }

    public Vector3 ReadVector3()
    {
        Vector3 value;
        _stream.ReadUnmanaged(&value);
        return value;
    }

    public int ReadArraySize()
    {
        int value;
        _stream.ReadUnmanaged(&value);
        return value;
    }

    public T[] ReadUnmanagedArray<T>() where T : unmanaged
    {
        int size = 0;
        _stream.ReadUnmanaged(&size);
        T[] array = new T[size];
        
        fixed (T* ptr = array)
        {
            _stream.ReadToPointer(ptr, size);
        }

        return array;
    }

    public string ReadString()
    {
        return _stream.ReadString(ReadArraySize());
    }
}