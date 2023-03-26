using System.Numerics;
using ThirdParty.OpenGL;

namespace SBEngine;

public unsafe class TileMapLayerRenderer
{
    public float TileSize = 1f;
    public int Layer = 0;
    
    private QuadMeshBuffer _buffer;
    private Material _material;
    private AssetsDatabase _assetsDatabase;
    
    public TileMapLayerRenderer(OpenGL gl, GLShader uberShader, AssetsDatabase assetsDatabase)
    {
        _assetsDatabase = assetsDatabase;
        _buffer = new QuadMeshBuffer(gl);
        _material = new Material(uberShader);
    }
    
    public void UploadData(TileMapLayer tileMapLayer, TileMapSpriteAtlas atlas)
    {
        _buffer.Clear();
        
        var size = tileMapLayer.Size;

        Vector2 tileSize = new Vector2(TileSize);
        Vector4 tileColor = VColors.White;

        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
            {
                Vector3 position = new Vector3(x + 0.5f, y + 0.5f, 0);
                ref TileMapSprite sprite = ref atlas.Sprites.GetRef(tileMapLayer.Tiles[x, y].spriteIdx);

                _buffer.AddTexturedQuad(ref position, ref tileSize, ref tileColor,
                    ref sprite.UvBottomLeft, ref sprite.UvTopLeft, 0);
            }
        }

        _material.Variables["uTexture"] = VariableValueUnion.Texture(0, atlas.Texture.Value);
    }

    public DrawCall GetDrawCall(Transform2D transform)
    {
        return new DrawCall(_buffer, _material, transform);
    }
}