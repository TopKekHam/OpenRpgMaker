public class Utils
{

    public static Optional<string> ReadFile(string path)
    {
        if(File.Exists(path))
        {
            return new Optional<string>(File.ReadAllText(path));
        }

        return new Optional<string>();
    }

}

