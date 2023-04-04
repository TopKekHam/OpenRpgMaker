using System.Numerics;
using static OpenGL;

public unsafe class Renderer
{

    static GLBuffer tempVertexBuffer;
    static GLShader quadShader;

    public static void Load()
    {
        tempVertexBuffer = new GLBuffer(GL.ARRAY_BUFFER, "Renderer temp buffer");
        
    }

    public static void Clear(Vector4 color)
    {
        glClearColor(color.X, color.Y, color.Z, color.W);
        glClear(GL.COLOR_BUFFER_BIT);
    }

    public static void DrawQuad(Vector2 position, Vector2 size, Vector4 tint)
    {

    }

    public static void DrawQuad(Vector2 position, Vector2 size, Vector4 tint, Vector2 uv0, Vector2 uv1, Texture texture)
    {

    }

    // position is the bottom left point of the text line
    public static void DrawTextSimple(ReadOnlySpan<char> text, Vector2 position, float fontSize, IFont font)
    {
        
    }

}
