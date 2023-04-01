using static OpenGL;

public unsafe class GLVAO
{

    public uint handle;

    public GLVAO(string name = "unknown vao")
    {
        handle = glGenVertexArray(name);
    }

    public void Bind()
    {
        glBindVertexArray(handle);
    }
}
