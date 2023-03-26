namespace SBEngine;

public interface IRef
{
    void SetRefObject(object obj);
}

public class AssetRef<T> : IRef where T :  class
{
    public T Value { get => _value; }
    public string Path { get =>_path; }

    private T _value;
    [SerializedField] private string _path;

    public AssetRef() { }

    public AssetRef(string path)
    {
        _path = path;
    }

    public void SetRefObject(object obj)
    {
        _value = obj as T;
    }

    public void Load(AssetsDatabase database)
    {
        database.RefAsset(this);
    }
}