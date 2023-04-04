using StbImageSharp;

public class PNGReader
{

    public static Texture Read(Stream stream)
    {
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        return new Texture(image.Width, image.Height, image.Data);
    }

}

