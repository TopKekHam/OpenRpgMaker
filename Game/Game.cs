using System.Numerics;
using SBEngine.Serialization;
using ThirdParty.OpenGL;

namespace SBEngine;

public class Startup
{
    static void Main(string[] args)
    {
        Game game = new Game(new Engine(), new Scene());
        game.Run();
    }
}

public class Game
{
    public Scene ActiveScene;
    public Camera2D ActiveCamera;
    
    private TileMapLayerRenderer _tileMapLayerRenderer;
    private Engine _engine;
    private SceneRenderer _sceneRenderer;

    public Game(Engine engine, Scene startScene)
    {
        _engine = engine;

        _sceneRenderer = new SceneRenderer(engine);

        ActiveCamera = new Camera2D()
        {
            Size = engine.Window.GetWindowSize().ToVector2() / 80f
        };
        
        LoadScene(startScene);
    }

    void LoadScene(Scene scene)
    {
        ActiveScene = scene;
        
        scene.Load(_engine.AssetsDatabase);
        
        _sceneRenderer.LoadScene(scene);
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            _engine.BeginFrame();

            foreach (var ev in _engine.PlatformEvents.Events)
            {
                if (ev.Type == PlatformEventType.QUIT) running = false;
            }

            Render();

            _engine.EndFrame();
        }
    }

    public void Update()
    {
    }

    public void Render()
    {
        
        _engine.Renderer.ClearDepth();
        _engine.Renderer.SetDepthTesting(true);
        
        _sceneRenderer.QueueDrawCalls();
        
        _engine.Renderer.Render(ActiveCamera);
    }
}