using System.Numerics;

namespace SBEngine.Editor;

// Left to right only for now
public class RowLayout : ILayoutHandler
{
    private ContentAlignment _contentAlignment;
    private Box _totalArea;

    public RowLayout(Box area, ContentAlignment contentAlignment = ContentAlignment.MIDDLE)
    {
        _totalArea = area;
        _contentAlignment = contentAlignment;
    }

    public Box CalcBoxForSize(Vector2 preferredSize)
    {
        var box = new Box() { Size = preferredSize };
        box.Size.Y = MathF.Min(box.Size.Y, _totalArea.Size.Y);

        Alignment vAlignment = Alignment.VCENTER; // default middle
        
        if (_contentAlignment == ContentAlignment.START)
        {
            vAlignment = Alignment.TOP;
        }
        else if (_contentAlignment == ContentAlignment.END)
        {
            vAlignment = Alignment.BOTTOM;
        }

        _totalArea.AlignInside(ref box, Alignment.LEFT | vAlignment);

        _totalArea.ChangeSideLength(-box.Size.X, BoxSide.LEFT);

        return box;
    }

    public Vector2 GetMaxSize()
    {
        return _totalArea.Size;
    }
}