namespace SBEngine;

public interface IDrawableMesh
{
    int VertexCount { get; }
    void Bind();
}