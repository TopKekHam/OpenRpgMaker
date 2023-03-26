using ThirdParty.OpenGL;

namespace SBEngine;

public class TileMapSpriteAtlas
{
    public ArrayList<TileMapSprite> Sprites;
    public AssetRef<GLTexture> Texture;

    public TileMapSpriteAtlas()
    {
        Sprites = new ArrayList<TileMapSprite>();
    }
}