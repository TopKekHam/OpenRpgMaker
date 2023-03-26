namespace SBEngine;

public class TileMapLayer
{
    public string Name;
    public Vector2I Size;
    public Tile[,] Tiles;
    public AssetRef<TileMapSpriteAtlas> TileMapAtlas;
    public int SortLayer;
    
    public TileMapLayer(AssetsDatabase assetsDatabase)
    {
        TileMapAtlas = assetsDatabase.GetDefaultAssetRef<TileMapSpriteAtlas>();
    }

    public void Load(AssetsDatabase assetsDatabase)
    {
        TileMapAtlas.Load(assetsDatabase);
    }

    public void Resize(Vector2I newSize)
    {
        if (Size == newSize) return;

        var newTiles = new Tile[newSize.X, newSize.Y];

        Vector2I min = new Vector2I(Math.Min(newSize.X, Size.X), Math.Min(newSize.Y, Size.Y));

        //@OPTIMIZE, we can copy each row into the new array.
        for (int y = 0; y < min.Y; y++)
        {
            for (int x = 0; x < min.X; x++)
            {
                newTiles[x, y] = Tiles[x, y];
            }
        }

        Tiles = newTiles;
        Size = newSize;
    }
}