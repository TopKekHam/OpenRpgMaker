using System.Drawing;
using static OpenGL;

public unsafe class GLBuffer
{

    public GL type { get; private set; }
    public uint handle { get; private set; }

    public GLBuffer(GL type, string name = "Buffer")
    {
        this.type = type;
        handle = glGenBuffer(name);
    }

    public void Bind()
    {
        glBindBuffer(type, handle);
    }


    public void BindAndSetData<T>(T[] data, GL usage = GL.STATIC_DRAW) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            glBindBuffer(type, handle);
            glBufferData(type, (uint)(data.Length * sizeof(T)), ptr, usage);
        }
    }

    public void BindAndSetData(void* ptr, uint size, GL usage = GL.STATIC_DRAW)
    {
        glBindBuffer(type, handle);
        glBufferData(type, size, ptr, usage);
    }

    public void SetSubData<T>(T data, int offset, GL usage = GL.STATIC_DRAW) where T : unmanaged
    {
        SetSubData(&data, offset, sizeof(T), usage);
    }

    public void SetSubData<T>(T[] data, GL usage = GL.STATIC_DRAW) where T : unmanaged
    {
        SetSubData(data, 0, usage);
    }

    public void SetSubData<T>(T[] data, int offset, GL usage = GL.STATIC_DRAW) where T : unmanaged
    {
        SetSubData(data, offset, usage);
    }

    public void SetSubData(void* ptr, int size, GL usage = GL.STATIC_DRAW)
    {
        SetSubData(ptr, 0, (uint)size, usage);
    }

    public void SetSubData(void* ptr, uint size, GL usage = GL.STATIC_DRAW)
    {
        SetSubData(ptr, 0, size, usage);
    }

    public void SetSubData(void* ptr, uint offset, uint size, GL usage = GL.STATIC_DRAW)
    {
        glNamedBufferSubData(handle, offset, size, ptr);
    }

    public void SetSubData(void* ptr, int offset, int size, GL usage = GL.STATIC_DRAW)
    {
        glNamedBufferSubData(handle, (uint)offset, (uint)size, ptr);
    }

    public void SetSubData<T>(T[] data, uint offset, GL usage = GL.STATIC_DRAW) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            uint size = (uint)(data.Length * sizeof(T));
            glNamedBufferSubData(handle, offset, size, ptr);
        }
    }

    public T* Map<T>(GL ops = GL.WRITE_ONLY) where T : unmanaged
    {
        return (T*)glMapNamedBuffer(handle, ops);
    }

    public void Unmap()
    {
        glUnmapNamedBuffer(handle);
    }

}
