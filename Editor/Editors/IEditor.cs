namespace SBEngine.Editor;

public interface IEditor
{
    void OnGui(IMGUI imgui);
    void OnGameRender();
}