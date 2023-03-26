namespace SBEngine;

public class Engine
{
    public Window Window;
    public Renderer Renderer;
    public Input Input;
    public Audio Audio;
    public Time Time;
    public SceneManager SceneManager;
    public AssetsDatabase AssetsDatabase;
    public PlatformEvents PlatformEvents;
    
    public Engine(string workDirectory = ".\\")
    {
        Window = new Window(1280, 720, "Small Bytes Engine");

        Input = new Input();
        Audio = new Audio();
        Time = new Time();
        SceneManager = new SceneManager();
        
        AssetsDatabase = new AssetsDatabase(Window, workDirectory);
        PlatformEvents = new PlatformEvents(Input, Window);
        
        Renderer = new Renderer(Window, AssetsDatabase, PlatformEvents);
    }

    public void BeginFrame()
    {
        Input.Update();
        Time.Update();
        PlatformEvents.Update();
    }

    public void EndFrame()
    {
        Window.SwapGLBuffers();
    }
    
    
}