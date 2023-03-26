namespace SBEngine;

public interface IDrawCallMultiple
{
    uint DrawCount { get; }
    ArrayList<int> First {get;}
    ArrayList<uint> Count { get; }
}

public class DrawCall
{
    public IDrawableMesh Mesh;
    public Material Material;
    public Transform2D Transform;
    public int VertexCount;
    public int VertexOffset;

    public IDrawCallMultiple? Multiple;

    public DrawCall(IDrawableMesh mesh, Material material, Transform2D transform)
    {
        Mesh = mesh;
        Material = material;
        Transform = transform;

        VertexCount = mesh.VertexCount;
        VertexOffset = 0;
    }
    
}