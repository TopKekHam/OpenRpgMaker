using System.Numerics;
using SBEngine.Serialization;
using ThirdParty.OpenGL;

namespace SBEngine;

public class Asset
{
    public string Path;
    public object AssetData;
    public ArrayList<IRef> Refs;

    public Asset(string path, object assetData)
    {
        Path = path;
        AssetData = assetData;
        Refs = new ArrayList<IRef>();
    }

    public void UpdateRefs()
    {
        for (int i = 0; i < Refs.Count; i++)
        {
            Refs[i].SetRefObject(AssetData);
        }
    }
}

public unsafe class AssetsDatabase
{
    private Dictionary<string, GLTexture> _textures;
    private Dictionary<string, GLShader> _shaders;
    private Dictionary<string, Asset> _assets;
    private Dictionary<Type, Asset> _defaultAssets; 

    private OpenGL _gl;

    private GLTexture _defaultTexture;
    private string _workDirectory;
    private CustomDeserializer _customDeserializer;
    private CustomSerializer _customSerializer;

    public AssetsDatabase(Window window, string workDirectory)
    {
        _customDeserializer = new CustomDeserializer();
        _customSerializer = new CustomSerializer();

        _textures = new Dictionary<string, GLTexture>();
        _shaders = new Dictionary<string, GLShader>();
        _assets = new Dictionary<string, Asset>();
        _defaultAssets = new Dictionary<Type, Asset>();

        _gl = window.GetOpenGLReference();
        _workDirectory = workDirectory;

        LoadDefaultTexture();
        LoadTextures(Path.Join(_workDirectory, "Resources\\Textures"));
        
        LoadShaders(Path.Join(_workDirectory, "Resources\\Shaders"));
    }

    void LoadDefaultTexture()
    {
        int* texture = stackalloc int[32 * 32];

        int color1 = 0;
        int color2 = (255 << 24) + (255 << 16) + 255;

        for (int y = 0; y < 32; y++)
        {
            int check = (y / 4) % 2;

            for (int x = 0; x < 32; x++)
            {
                texture[x + (32 * y)] = (x / 4) % 2 == check ? color2 : color1;
            }
        }

        _defaultTexture = GLTexture.LoadFromRawBytes(_gl, (byte*)texture, 32, 32);
    }

    void LoadTextures(string path)
    {
        var dirs = Directory.GetDirectories(path);
        var files = Directory.GetFiles(path);

        for (int i = 0; i < dirs.Length; i++)
        {
            LoadTextures(dirs[i]);
        }

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".png") == false) continue;

            var filePath = files[i];

            var texture = GLTexture.LoadFromFile(_gl, filePath);
            texture.SetMinMagFilter(GL.NEAREST, GL.NEAREST);
            _assets.Add(filePath, new Asset(filePath, texture));
            
            Console.WriteLine($"[AssetsDatabase] Loaded Texture: {filePath}");
        }
    }

    void LoadShaders(string path)
    {
        var files = Directory.GetFiles(path);

        GLShaderLoader _shaderLoader = new GLShaderLoader(_gl);

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].EndsWith(".glsl") == false) continue;

            var shader = _shaderLoader.LoadShaderFromSource(File.ReadAllText(files[i]), files[i]);
            
            _assets.Add(files[i], new Asset(files[i], shader));
            
            Console.WriteLine($"[AssetsDatabase] Loaded Shader: {files[i]}");
        }
    }

    public T GetDefaultAsset<T>() where T : class
    {
        var asset = GetGetDefaultAssetInternal<T>();
        return asset.AssetData as T;
    }
    
    public AssetRef<T> GetDefaultAssetRef<T>() where T : class
    {
        var asset = GetGetDefaultAssetInternal<T>();
        
        var assetRef = new AssetRef<T>(asset.Path);
        assetRef.Load(this);
        
        return assetRef;
    }

    Asset GetGetDefaultAssetInternal<T>()
    {
        if (_defaultAssets.TryGetValue(typeof(T), out var asset))
        {
            return asset;
        }

        throw new Exception($"No default value found for asset type {typeof(T).Name}.");
    }
    
    public void SetDefaultAsset<T>(string path) where T : class
    {
        var key = typeof(T);

        var asset = GetAssetInternal<T>(path);

        if (_defaultAssets.ContainsKey(key))
        {
            _defaultAssets[key] = asset;
        }
        else
        {
            _defaultAssets.Add(key, asset);
        }
    }

    public void SaveAsset<T>(string path, T data)
    {
        Asset asset;
        
        if (_assets.ContainsKey(path))
        {
            asset = _assets[path];
        }
        else
        {
            asset = new Asset(path, data);
            _assets.Add(path, asset);
        }

        asset.AssetData = data;
        asset.UpdateRefs();
        
        var text = _customSerializer.Serialize(data);

        FileStream stream;

        if (File.Exists(path) == false)
        {
            FileInfo info = new FileInfo(path);
            info.Directory.Create();
            stream = File.Create(path);
        }
        else
        {
            stream = File.OpenWrite(path);
        }

        StreamWriter writer = new StreamWriter(stream);
        writer.Write(text);
        writer.Flush();

        stream.Close();
    }

    public bool TryGetAsset<T>(string path, out T data) where T : class
    {
        if (TryGetAsset<T>(path, out Asset asset))
        {
            data = asset.AssetData as T;
            return true;
        }
        else
        {
            data = null;
            return false;
        }
    }

    public void RefAsset<T>(AssetRef<T> assetRef) where T : class
    {
        var asset = GetAssetInternal<T>(assetRef.Path);
        assetRef.SetRefObject(asset.AssetData);
        asset.Refs.Add(assetRef);
    }

    public T GetAsset<T>(string path) where T : class
    {
        var asset = GetAssetInternal<T>(path);
        return asset.AssetData as T;
    }

    public AssetRef<T> GetAssetRef<T>(string path) where T : class
    {
        Asset asset = GetAssetInternal<T>(path);
        
        var assetRef = new AssetRef<T>(path);
        assetRef.Load(this);
        
        return assetRef;
    }
    
    Asset GetAssetInternal<T>(string path) where T : class
    {
        if (TryGetAsset<T>(path, out Asset asset))
        {
            return asset;
        }
        
        throw new Exception($"Asset {path} not found.");
    }
    
    bool TryGetAsset<T>(string path, out Asset asset) where T : class
    {
        if (_assets.TryGetValue(path, out asset))
        {
            return true;
        }
        else if (File.Exists(path))
        {
            var data = LoadAssetFromFile<T>(path);
            Console.WriteLine($"[AssetsDatabase] Loaded Asset: {path}");
            asset = new Asset(path, data);
            _assets.Add(path, asset);
            return true;
        }
        else
        {
            asset = null;
            return false;
        }
    }
    
    T LoadAssetFromFile<T>(string path) where T : class
    {
        var fileData = File.ReadAllText(path);
        return _customDeserializer.Deserialize<T>(fileData);
    }
}