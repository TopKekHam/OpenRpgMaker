using System.Numerics;
using ThirdParty.OpenGL;

namespace SBEngine.Editor;

public class AtlasTileBoxButton
{
    public IMGUIElementState State;
}

public class SpriteAtlasSpriteSelector
{
    public TileMapSpriteAtlas ActiveAtlas;

    private QuadMeshBuffer _buffer;
    private Material _material;
    private Renderer _renderer;

    public SpriteAtlasSpriteSelector(Engine engine)
    {
        _renderer = engine.Renderer;
        _buffer = new QuadMeshBuffer(engine.Window.GetOpenGLReference());
        _material = new Material(engine.AssetsDatabase.GetAsset<GLShader>(".\\Resources\\Shaders\\uber.glsl"));
    }

    public void Update(string id, IMGUI imgui, InputEvents inputEvents, ref int selectedSpriteIndex, float z = 0)
    {
        imgui.BeginLayers(2);

        Box panelBox = imgui.Layout.CalcBoxForSize(imgui.Layout.GetMaxSize());
        imgui.Box(panelBox, VColors.DarkGrey);

        if (ActiveAtlas == null)
        {
            imgui.Gap(Vector2.Zero);
            return;
        }

        _buffer.Clear();
        float tileSize = 1f;
        var spritesInWidth = (int)(imgui.Layout.GetMaxSize().X / tileSize);

        int spriteIdx = 0;
        imgui.BeginRows(0, ContentAlignment.START);

        var updateState = panelBox.IsPointInside(imgui.MousePosition);

        while (spriteIdx < ActiveAtlas.Sprites.Count)
        {
            imgui.CapHeight(tileSize);
            imgui.BeginColumns(0, ContentAlignment.START);

            for (int i = 0; i < spritesInWidth && spriteIdx < ActiveAtlas.Sprites.Count; i++)
            {
                // do single sprite selector

                var box = imgui.Layout.CalcBoxForSize(new Vector2(tileSize));

                var selectorId = $"{id}_{spriteIdx}";
                imgui.GetData<AtlasTileBoxButton>(selectorId, out var data);

                if (updateState)
                {
                    if (box.IsPointInside(imgui.MousePosition) &&
                        inputEvents.TryGetMouseButtonEvent(MouseButton.LEFT, InputState.DOWN, out var mouseEvent))
                    {
                        selectedSpriteIndex = spriteIdx;
                        mouseEvent.Use();
                    }
                }

                if (selectedSpriteIndex == spriteIdx)
                {
                    data.State |= IMGUIElementState.SELECTED;
                }
                else
                {
                    data.State = IMGUIElementState.NONE;
                }
                
                var color = VColors.White;
                
                if (data.State.Flag(IMGUIElementState.SELECTED))
                {
                    color = VColors.Grey;
                    selectedSpriteIndex = spriteIdx;
                }

                ref TileMapSprite sprite = ref ActiveAtlas.Sprites.GetRef(spriteIdx);

                Vector3 pos = new Vector3(box.Position, z);
                
                _buffer.AddTexturedQuad(ref pos, ref box.Size, ref color,
                    ref sprite.UvBottomLeft, ref sprite.UvTopLeft, 0);

                spriteIdx++;
            }

            imgui.EndLayout();
        }

        imgui.EndLayout();

        _material.Variables["uTexture"] = VariableValueUnion.Texture(0, ActiveAtlas.Texture.Value);

        _renderer.QueueDrawCall(new DrawCall(_buffer, _material, new Transform2D()), imgui.DrawLayer + 20);
    }
}