using System.Numerics;
using ThirdParty.OpenGL;

namespace SBEngine;

public unsafe class QuadMeshBuffer : IDrawableMesh
{
    public int VertexCount { get => _quadCount * 6; }
    
    private GLBuffer _vbo;
    private NativeList<QuadVertex> _vertexBuffer;
    private GLVertexAttribSetup _setup;
    private int _quadCount;
    
    public QuadMeshBuffer(OpenGL gl)
    {
        _vbo = new GLBuffer(gl, GL.ARRAY_BUFFER, "quad mesh buffer vbo");
        _vertexBuffer = new NativeList<QuadVertex>();
        _setup = new GLVertexAttribSetup(gl);
        GeometryHelper.SetAttribSetupForQuad(_setup);
        _quadCount = 0;
    }

    public void AddColorQuad(Vector3 position, Vector2 size, Vector4 color)
    {
        _quadCount++;
        
        QuadVertex* data = stackalloc QuadVertex[6];
        GeometryHelper.CalcVerticesForQuad(data, position, size, color);

        for (int i = 0; i < 6; i++)
        {
            _vertexBuffer.Add(*(data + i));
        }
    }

    public void AddTexturedQuad(Vector3 position, Vector2 size, Vector4 color, int textureIndex)
    {
        _quadCount++;
        
        QuadVertex* data = stackalloc QuadVertex[6];
        GeometryHelper.CalcVerticesForQuad(data, position, size, color, textureIndex);

        for (int i = 0; i < 6; i++)
        {
            _vertexBuffer.Add(*(data + i));
        }
    }
    
    public void AddTexturedQuad(ref Vector3 position, ref Vector2 size, ref Vector4 color, 
        ref Vector2 uvBottomLeft, ref Vector2 uvTopRight, int textureIndex)
    {
        _quadCount++;
        
        QuadVertex* data = stackalloc QuadVertex[6];
        GeometryHelper.CalcVerticesForQuad(data, ref position, ref size, ref color, ref uvBottomLeft, ref uvTopRight,  textureIndex);

        for (int i = 0; i < 6; i++)
        {
            _vertexBuffer.Add(*(data + i));
        }
    }

    public void AddColorLine(Vector3 pos1, Vector3 pos2, float width, Vector4 color)
    {
        _quadCount++;
        
        QuadVertex* data = stackalloc QuadVertex[6];
        GeometryHelper.CalcVerticesForLineQuad(data, pos1, pos2, width, color);

        for (int i = 0; i < 6; i++)
        {
            _vertexBuffer.Add(*(data + i));
        }
    }
    
    public void Clear()
    {
        _vertexBuffer.Clear();
        _quadCount = 0;
    }
    
    public void Bind()
    {
        _vbo.SetData(_vertexBuffer.Data, _vertexBuffer.SizeInBytes);
        _vbo.Bind();
        _setup.Bind();
    }
}