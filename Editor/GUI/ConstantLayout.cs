using System.Numerics;

namespace SBEngine.Editor;

public class ConstantLayout : ILayoutHandler
{
    private Box _box;

    public ConstantLayout(Box box)
    {
        _box = box;
    }

    public Box CalcBoxForSize(Vector2 preferredSize)
    {
        return _box;
    }

    public Vector2 GetMaxSize()
    {
        return _box.Size;
    }
}