using System.Numerics;
using SBEngine;
using ThirdParty.OpenGL;

namespace SBEngine.Editor;

public class StartUp
{
    static void Main(string[] args)
    {
        var editor = new Editor();
        editor.Run();
    }
}

public class DefaultAssets
{
    public string DefaultTilemapSpriteAtlas = ".\\Resources\\Assets\\DefaultTilemapSpriteAtlas.atlas";


    public void Load(AssetsDatabase database)
    {
        var atlas = new TileMapSpriteAtlas();

        atlas.Texture = database.GetAssetRef<GLTexture>(".\\Resources\\Textures\\DefaultTileset.png");

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
                atlas.Sprites.Add(new TileMapSprite()
                {
                    UvBottomLeft = new Vector2(x / 8f, y / 8f),
                    UvTopLeft = new Vector2((x + 1) / 8f, (y + 1) / 8f)
                });
        }


        database.SaveAsset(DefaultTilemapSpriteAtlas, atlas);
        database.SetDefaultAsset<TileMapSpriteAtlas>(DefaultTilemapSpriteAtlas);
    }
}

public class Editor
{
    public Game Game;
    public Engine Engine;
    public InputEvents InputEvents;
    public DefaultAssets DefaultAssets;

    public Queue<string> BottomBarTextLabels;

    private IMGUI _imgui;
    private SceneEditor _sceneEditor;
    private TileMapSpriteAtlasEditor _tileMapSpriteAtlasEditor;
    private IEditor _activeEditor;

    public Editor()
    {
        BottomBarTextLabels = new Queue<string>();

        Engine = new Engine();
        InputEvents = new InputEvents(Engine);

        DefaultAssets = new DefaultAssets();
        DefaultAssets.Load(Engine.AssetsDatabase);

        string testScenePath = ".\\Resources\\Scenes\\Test_Scene.scene";

        if (Engine.AssetsDatabase.TryGetAsset<Scene>(testScenePath, out Scene scene) == false)
        {
            scene = new Scene();
            scene.Name = "Test_Scene";

            Engine.AssetsDatabase.SaveAsset(testScenePath, scene);
        }

        Game = new Game(Engine, scene);

        _imgui = new IMGUI(Engine, InputEvents);

        _sceneEditor = new SceneEditor(this);
        _tileMapSpriteAtlasEditor = new TileMapSpriteAtlasEditor();

        _activeEditor = _sceneEditor;
    }

    void UnLoad()
    {
    }

    public void Run()
    {
        bool running = true;

        while (running)
        {
            InputEvents.Clear();
            Engine.BeginFrame();
            InputEvents.Update(Engine.Input);

            foreach (var ev in Engine.PlatformEvents.Events)
            {
                if (ev.Type == PlatformEventType.QUIT) running = false;
            }

            Engine.Renderer.ClearColor(VColors.Black);

            _activeEditor.OnGameRender();
            Game.Render();
            
            _imgui.BeginFrame();

            GuiMain();

            _imgui.EndFrame();

            Engine.EndFrame();
        }

        UnLoad();
    }

    void GuiMain()
    {
        BottomBarTextLabels.Enqueue($"fps: {1f / Engine.Time.DeltaTime:N0}");

        var max = _imgui.Layout.GetMaxSize();

        Random random = new Random(0);

        _imgui.BeginRows(max.Y, ContentAlignment.START);
        {
            var barSize = _imgui.Style.FontSize * 1.5f;

            _imgui.CapHeight(barSize);
            TopBar();

            _imgui.CapHeight(max.Y - (barSize * 2));
            _activeEditor.OnGui(_imgui);

            _imgui.CapHeight(barSize);
            BottomBar();
        }

        _imgui.EndLayout();
    }

    void TopBar()
    {
        _imgui.BeginColumns();

        if (_imgui.Button("editor_top_bar_scene_panel", "Scene"))
        {
            _activeEditor = _sceneEditor;
        }

        _imgui.Gap(new Vector2(0.125f, 0));

        if (_imgui.Button("editor_top_bar_sprite_atlas_panel", "Sprite Atlas"))
        {
            _activeEditor = _tileMapSpriteAtlasEditor;
        }

        _imgui.EndLayout();
    }

    void BottomBar()
    {
        _imgui.BeginColumns();

        while (BottomBarTextLabels.TryDequeue(out string text))
        {
            _imgui.Text(text, VColors.Green, 0, 1.25f);
        }

        _imgui.EndLayout();
    }
}