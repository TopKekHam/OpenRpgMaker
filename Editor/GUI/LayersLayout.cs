using System.Numerics;

namespace SBEngine.Editor;

public class LayersLayout : ILayoutHandler
{
    private IMGUI _imgui;
    private Box _box;
    private int _times;
    
    public LayersLayout(IMGUI imgui, Box box, int times)
    {
        _imgui = imgui;
        _box = box;
        _times = times;
    }
    
    public Box CalcBoxForSize(Vector2 preferredSize)
    {
        _times--;

        var layerBox = _box;
        layerBox.Size = Vector2.Min(preferredSize, _box.Size);
        
        if (_times == 0)
        {
            _imgui.EndLayout();
        }

        return layerBox;
    }

    public Vector2 GetMaxSize()
    {
        return _box.Size;
    }
}