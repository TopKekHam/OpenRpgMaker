
// Format: rgba, 4 channels, 8 bits per channel.
public class Texture
{

    public int Width, Height;
    public byte[] Data;

    public Texture(int width, int height, byte[] data)
    {
        Width = width;
        Height = height;
        Data = data;
    }
}

