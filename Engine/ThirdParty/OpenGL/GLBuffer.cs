
using ThirdParty.SDL;

namespace ThirdParty.OpenGL;

public unsafe class GLBuffer
{
    public GL Type { get; private set; }
    public uint Handle { get; private set; }

    OpenGL _gl;

    public GLBuffer(OpenGL gl, GL type, string name = "Buffer")
    {
        this.Type = type;
        this._gl = gl;
        
        uint handle;
        gl.glCreateBuffers(1, &handle);
        Handle = handle;

        gl.hfNameObject(GL.BUFFER, Handle, name);
    }

    public void Bind()
    {
        _gl.glBindBuffer(Type, Handle);
    }

    public void SetData<T>(T[] data, GL usage = GL.STATIC_DRAW) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            SetData(ptr, data.Length * sizeof(T), usage);
            //gl.glBufferData(Type, (uint)(data.Length * sizeof(T)), ptr, usage);
        }
    }

    public void SetData(void* ptr, int size, GL usage = GL.STATIC_DRAW)
    {
        _gl.glNamedBufferData(Handle, (uint)(size), ptr, usage);
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
        _gl.glNamedBufferSubData(Handle, offset, size, ptr);
    }

    public void SetSubData(void* ptr, int offset, int size, GL usage = GL.STATIC_DRAW)
    {
        _gl.glNamedBufferSubData(Handle, (uint)offset, (uint)size, ptr);
    }

    public void SetSubData<T>(T[] data, uint offset, GL usage = GL.STATIC_DRAW) where T : unmanaged
    {
        fixed (T* ptr = data)
        {
            uint size = (uint)(data.Length * sizeof(T));
            _gl.glNamedBufferSubData(Handle, offset, size, ptr);
        }
    }

    public T* Map<T>(GL ops = GL.WRITE_ONLY) where T : unmanaged
    {
        return (T*)_gl.glMapNamedBuffer(Handle, ops);
    }

    public void Unmap()
    {
        _gl.glUnmapNamedBuffer(Handle);
    }

}
