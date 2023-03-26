using System.Numerics;

namespace SBEngine.Editor;

public class CapLayout : ILayoutHandler
{
    private IMGUI _imgui;
    private int _times;
    private Vector2 _size;
    private ILayoutHandler _parentLayout;

    public CapLayout(IMGUI imgui, Vector2 size, ILayoutHandler parentLayout, int times)
    {
        _size = size;
        _parentLayout = parentLayout;
        _imgui = imgui;
        _times = times;
    }

    public Box CalcBoxForSize(Vector2 preferredSize)
    {
        var size = Vector2.Min(_parentLayout.GetMaxSize(), preferredSize);

        size = MinIfNotZero(size, _size);

        Box box = _parentLayout.CalcBoxForSize(size);

        _times -= 1;
        
        if (_times == 0)
        {
            _imgui.EndLayout();
        }

        return box;
    }

    public Vector2 GetMaxSize()
    {
        var size = _parentLayout.GetMaxSize();

        size = MinIfNotZero(size, _size);

        return size;
    }

    Vector2 MinIfNotZero(Vector2 vec, Vector2 vec2)
    {
        if (vec2.X > 0)
        {
            vec.X = MathF.Min(vec2.X, vec.X);
        }
        
        if (vec2.Y > 0)
        {
            vec.Y = MathF.Min(vec2.Y, vec.Y);
        }

        return vec;
    }
}