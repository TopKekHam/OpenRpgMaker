using System.Numerics;

namespace SBEngine.Editor;

// Top to bottom only for now.
public class ColumnLayout : ILayoutHandler
{
    private ContentAlignment _contentAlignment;
    private Box _totalArea;

    public ColumnLayout(Box area, ContentAlignment contentAlignment = ContentAlignment.MIDDLE)
    {
        _totalArea = area;
        _contentAlignment = contentAlignment;
    }

    public Box CalcBoxForSize(Vector2 preferredSize)
    {
        var box = new Box() { Size = preferredSize };
        box.Size.X = MathF.Min(box.Size.X, _totalArea.Size.X);

        Alignment hAlignment = Alignment.HCENTER; // default middle
        
        if (_contentAlignment == ContentAlignment.START)
        {
            hAlignment = Alignment.LEFT;
        }
        else if (_contentAlignment == ContentAlignment.END)
        {
            hAlignment = Alignment.RIGHT;
        }

        _totalArea.AlignInside(ref box, Alignment.TOP | hAlignment);

        _totalArea.ChangeSideLength(-box.Size.Y, BoxSide.TOP);

        return box;
    }

    public Vector2 GetMaxSize()
    {
        return _totalArea.Size;
    }
}