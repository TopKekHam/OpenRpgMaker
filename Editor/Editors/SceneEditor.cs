using System.Diagnostics;
using System.Numerics;
using ThirdParty.OpenGL;

namespace SBEngine.Editor;

public class SceneEditor : IEditor
{
    public int SelectedTile;
    
    private Editor _editor;
    private QuadMeshBuffer _buffer;
    private Material _material;

    private Vector2 _mouseWorldPosition;
    private Vector2 _mouseWorldPositionDelta;
    private Vector2I _worldTileIndex;

    private SpriteAtlasSpriteSelector _selector;
    private SceneTilemapLayerEditor _tilemapLayerEditor;
    private TileMapLayer? _selectedLayer;
    private bool _drawTile;

    private float leftPanelSize;
    private float rightPanelSize;
    
    public SceneEditor(Editor editor)
    {
        _editor = editor;

        _material = new Material(editor.Engine.AssetsDatabase.GetAsset<GLShader>(".\\Resources\\Shaders\\uber.glsl"));

        _buffer = new QuadMeshBuffer(editor.Engine.Window.GetOpenGLReference());

        _selector = new SpriteAtlasSpriteSelector(_editor.Engine);

        _tilemapLayerEditor = new SceneTilemapLayerEditor();
    }

    public void OnGui(IMGUI imgui)
    {
        {
            //_editor.BottomBarTextLabels.Enqueue($"mwp: {_mouseWorldPosition:N2}");
            //_editor.BottomBarTextLabels.Enqueue($"mwpd: {_mouseWorldPositionDelta:N2}");
            _editor.BottomBarTextLabels.Enqueue($"ti: {_worldTileIndex}");
        }

        if (_selectedLayer != null)
        {
            _selector.ActiveAtlas = _selectedLayer.TileMapAtlas.Value;
        }
        
        var maxBox = imgui.Layout.CalcBoxForSize(imgui.Layout.GetMaxSize());

        leftPanelSize = leftPanelSize == 0 ? 2 : leftPanelSize;
        rightPanelSize = rightPanelSize == 0 ? 3 : rightPanelSize;

        Box[] boxes = new Box[3];

        boxes[0] = maxBox;
        boxes[0].ChangeSideLength(-(maxBox.Size.X - leftPanelSize), BoxSide.RIGHT);

        boxes[1] = maxBox;
        boxes[1].ChangeSideLength(-leftPanelSize, BoxSide.LEFT);
        boxes[1].ChangeSideLength(-rightPanelSize, BoxSide.RIGHT);

        boxes[2] = maxBox;
        boxes[2].ChangeSideLength(-(maxBox.Size.X - rightPanelSize), BoxSide.LEFT);

        imgui.SplitHorizontalPanels("scene_editor_split_panels", boxes);
        
        leftPanelSize = boxes[0].Size.X;
        rightPanelSize = boxes[2].Size.X;
        
        imgui.BeginConstant(boxes[0]);
        LeftSide(imgui);
        imgui.EndLayout();

        imgui.BeginConstant(boxes[1]);
        imgui.Gap(maxBox.Size);
        imgui.EndLayout();
        
        imgui.BeginConstant(boxes[2]);
        Right(imgui);
        imgui.EndLayout();

    }

    void LeftSide(IMGUI imgui)
    {
       _tilemapLayerEditor.Update("scene_editor_tilemap_layer_editor" ,_editor , this, imgui, ref _selectedLayer);
    }

    void Right(IMGUI imgui)
    {
        _selector.Update("scene_editor_tile_selector", imgui, _editor.InputEvents, ref SelectedTile);
    }

    public void OnGameRender()
    {
        var inputs = _editor.InputEvents;
        var camera = _editor.Game.ActiveCamera;
        camera.Size = _editor.Engine.Window.GetWindowSize().ToVector2() / 80f;

        _buffer.Clear();

        UpdateMousePosition(camera);
        if (inputs.TryGetMouseButtonEvent(MouseButton.MIDDLE, InputState.ACTIVE, out var mouseEvent))
        {
            camera.Position += _mouseWorldPositionDelta * -1;
            mouseEvent.Use();
        }
        else if (inputs.TryGetMouseButtonEvent(MouseButton.LEFT, InputState.DOWN, out var leftMouseEvent))
        {
            if (_selectedLayer != null)
            {
                _drawTile = true;
                leftMouseEvent.Use();
            }
        }

        if (inputs.TryGetMouseButtonEvent(MouseButton.RIGHT, InputState.DOWN, out var medr))
        {
            if (MouseInSideTileMapLayerRange())
            {
                SelectedTile = _selectedLayer.Tiles[_worldTileIndex.X, _worldTileIndex.Y].spriteIdx;
                medr.Use();
            }
        }
        
        if (inputs.TryGetMouseButtonEvent(MouseButton.LEFT, InputState.UP, out var leftMouseUpEvent))
        {
            _drawTile = false;
        }

        if (_drawTile)
        {
            if (SelectedTile != -1 && MouseInSideTileMapLayerRange())
            {
                _selectedLayer.Tiles[_worldTileIndex.X, _worldTileIndex.Y].spriteIdx = SelectedTile;
            }
        }

        DrawGrid(camera);

        var renderer = _editor.Engine.Renderer;
        var tran = new Transform2D();
        renderer.QueueDrawCall(new DrawCall(_buffer, _material, tran), 100);
    }

    bool MouseInSideTileMapLayerRange()
    {
        if (_selectedLayer == null) return false;
        return _worldTileIndex.InRange(Vector2I.Zero, _selectedLayer.Size - 1);
    }
    
    void UpdateMousePosition(Camera2D camera)
    {
        var windowSpace = _editor.Engine.Window.GetSpace();
        var cameraSpace = camera.GetSpace();

        _mouseWorldPosition = _editor.Engine.Input.MousePosition.MovePointToSpace(windowSpace, cameraSpace);
        _mouseWorldPositionDelta = _editor.Engine.Input.MousePositionDelta.MoveScalarToSpace(windowSpace, cameraSpace);
        _worldTileIndex = new Vector2I(_mouseWorldPosition);
    }

    void DrawGrid(Camera2D camera)
    {
        Vector2I from = new Vector2I(camera.Position - (camera.Size / 2)) - 1;
        Vector2I to = new Vector2I(camera.Position + (camera.Size / 2)) + 1;

        for (int x = from.X; x < to.X; x++)
        {
            _buffer.AddColorLine(new Vector3(x, from.Y, 100), new Vector3(x, to.Y, 100), 0.0125f, VColors.Magenta);
        }

        for (int y = from.Y; y < to.Y; y++)
        {
            _buffer.AddColorLine(new Vector3(from.X, y, 100), new Vector3(to.X, y, 100), 0.0125f, VColors.Magenta);
        }
    }
}