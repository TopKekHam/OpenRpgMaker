using System.Numerics;
using ThirdParty.OpenGL;

namespace SBEngine;

public unsafe class Mesh : IDrawableMesh
{

    public int VertexCount { get => Vertices.Count; }
    
    public NativeList<float> Vertices;
    public GLVertexAttribSetup Setup;
     
    private GLBuffer _vertexBuffer;
    
    public Mesh(OpenGL gl)
    {
        Vertices = new NativeList<float>();
        Setup = new GLVertexAttribSetup(gl);
        _vertexBuffer = new GLBuffer(gl, GL.ARRAY_BUFFER);
    }

    public void Bind()
    {
        _vertexBuffer.SetData(Vertices.Data, Vertices.SizeInBytes);
        _vertexBuffer.Bind();
        Setup.Bind();
    }

    public static Mesh Quad(OpenGL gl, Vector3 position, Vector2 size, Vector4 color)
    {
        Mesh mesh = new Mesh(gl);

        QuadVertex* data = stackalloc QuadVertex[6];
        GeometryHelper.CalcVerticesForQuad(data, position, size, color);
        
        int length = sizeof(QuadVertex) * 6 / sizeof(float);
        float* ptr = (float*)data;
        
        for (int i = 0; i < length; i++)
        {
            mesh.Vertices.Add(*(ptr + i));
        }
        
        GeometryHelper.SetAttribSetupForQuad(mesh.Setup);

        return mesh;
    }
}