using System.Numerics;

namespace SBEngine.Editor;

public class IMGUIStringField
{
    public IMGUIElementState State;
    public AnimationFader<Vector4> BackgroundColorFader;
    public TextFieldCursorLine cursor;
    public IMGUIStringField()
    {
        BackgroundColorFader = new AnimationFader<Vector4>(Vector4.Lerp);
        cursor = new TextFieldCursorLine();
    }
}