
namespace ThirdParty.OpenGL;

public unsafe class GLVAO
{
    public uint Handle { get; private set; }
    OpenGL _gl;

    public GLVAO(OpenGL gl, string name = "unknown vao")
    {
        this._gl = gl;

        uint handle;
        gl.glCreateVertexArrays(1, &handle);
        Handle = handle;
        
        gl.hfNameObject(GL.VERTEX_ARRAY, Handle, name);
    }

    public void Bind()
    {
        _gl.glBindVertexArray(Handle);
    }
}
