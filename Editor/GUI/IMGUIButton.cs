using System.Numerics;

namespace SBEngine.Editor;

public class IMGUIButton
{
    public IMGUIElementState State;
    public AnimationFader<Vector4> BackgroundColorFader;

    public IMGUIButton()
    {
        BackgroundColorFader = new AnimationFader<Vector4>(Vector4.Lerp);
    }
}