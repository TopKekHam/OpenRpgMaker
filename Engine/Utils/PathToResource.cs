namespace SBEngine;

public struct ResourcePath
{
    const string c_rootPath = "Resources";

    public static ResourcePath Root => new ResourcePath("");

    public string RelativePath;
    public string PathFromRoot => Path.Join(c_rootPath, RelativePath);

    public ResourcePath(string relativePath)
    {
        this.RelativePath = relativePath;
    }

    public static implicit operator ResourcePath(string relativePath)
    {
        return new ResourcePath(relativePath);
    }

    public static implicit operator string(ResourcePath resourcePath)
    {
        return resourcePath.RelativePath;
    }

    public ResourcePath Join(string path)
    {
        return new ResourcePath(Path.Join(RelativePath, path));
    }

    public override string ToString()
    {
        return RelativePath;
    }
}