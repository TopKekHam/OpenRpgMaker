namespace SBEngine;

public class Scene
{

    public string Name;
    public ArrayList<TileMapLayer> TileLayers;

    private bool _loaded;
    
    public Scene(string name = "Scene")
    {
        TileLayers = new ArrayList<TileMapLayer>();
        Name = name;
        _loaded = false;
    }

    public void Load(AssetsDatabase assetsDatabase)
    {
        if (_loaded) return;
        
        for (int i = 0; i < TileLayers.Count; i++)
        {
            TileLayers[i].Load(assetsDatabase);
        }

        _loaded = true;
    }
    
}