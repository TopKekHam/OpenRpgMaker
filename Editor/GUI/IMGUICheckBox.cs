using System.Numerics;

namespace SBEngine.Editor;

public class IMGUICheckBox
{
    public AnimationFader<Vector4> BackgroundColorFader;
    public IMGUIElementState State;
    
    public IMGUICheckBox()
    {
        BackgroundColorFader = new AnimationFader<Vector4>(Vector4.Lerp);
    }
}