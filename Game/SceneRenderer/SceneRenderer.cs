using ThirdParty.OpenGL;

namespace SBEngine;

public class SceneRenderer
{
    private Scene _scene;
    private List<TileMapLayerRenderer> _tileMapLayerPool;
    private Engine _engine;
    
    private GLShader uberShader;
    
    public SceneRenderer(Engine engine)
    {
        _engine = engine;
        _tileMapLayerPool = new List<TileMapLayerRenderer>();

        uberShader = engine.AssetsDatabase.GetAsset<GLShader>(".\\Resources\\Shaders\\uber.glsl");
    }

    public void LoadScene(Scene scene)
    {
        _scene = scene;
    }

    public void QueueDrawCalls()
    {

        if (_tileMapLayerPool.Count < _scene.TileLayers.Count)
        {
            _tileMapLayerPool.Add(new TileMapLayerRenderer(_engine.Window.GetOpenGLReference(), uberShader, _engine.AssetsDatabase));
        }
        
        for (int i = 0; i < _scene.TileLayers.Count; i++)
        {
            var renderer = _tileMapLayerPool[i];
            var layer = _scene.TileLayers[i];
            
            renderer.UploadData(layer, layer.TileMapAtlas.Value);
            
            _engine.Renderer.QueueDrawCall(renderer.GetDrawCall(new Transform2D()), layer.SortLayer);
        }
        
    }

}