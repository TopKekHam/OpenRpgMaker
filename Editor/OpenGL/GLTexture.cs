using StbImageSharp;
using static OpenGL;

public unsafe struct TextureMapData
{
    public void* ptr;
    public int left, top, width, height;
    public int stride;
}

public unsafe class GLTexture
{
    public uint handle;
    public int width, height;

    GL internalFormat;

    public GLTexture()
    {

    }

    public GLTexture(int _width, int _height, byte* data, GL format, GL type, GL internalFormat = GL.RGBA8)
    {
        Load(_width, _height, data, format, type, internalFormat);
    }

    public GLTexture(int _width, int _height, byte[] data, GL format, GL type, GL internalFormat = GL.RGBA8)
    {
        fixed (byte* ptr = data)
        {
            Load(_width, _height, ptr, format, type, internalFormat);
        }
    }

    void Load(int _width, int _height, byte* data, GL format, GL type, GL internalFormat)
    {
        this.internalFormat = internalFormat;

        uint textureMap;
        glGenTextures(1, &textureMap);
        glBindTexture(GL.TEXTURE_2D, textureMap);

        glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
        glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
        glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.CLAMP_TO_BORDER);
        glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.CLAMP_TO_BORDER);

        glTexImage2D(GL.TEXTURE_2D, 0, internalFormat, (uint)_width, (uint)_height, 0, format, type, data);

        glBindTexture(GL.TEXTURE_2D, 0);

        width = _width;
        height = _height;
        handle = textureMap;
    }

    public static GLTexture LoadFromFile(OpenGL gl, string path)
    {
        var stream = File.OpenRead(path);
        var image = PNGReader.Read(stream);

        var flipedData = new byte[image.Data.Length];

        int stride = image.Width * sizeof(int);

        for (int i = 0; i < image.Height; i++)
        {
            Array.Copy(image.Data, i * stride, flipedData, (image.Height - i - 1) * stride, stride);
        }

        fixed (byte* data = flipedData)
        {
            GLTexture texture = new GLTexture(image.Width, image.Height, data, GL.RGBA, GL.UNSIGNED_BYTE);
            stream.Close();
            return texture;
        }
    }

    public static GLTexture LoadFromRawBytes(byte[] bytes, int width, int height)
    {
        fixed (byte* data = bytes)
        {
            GLTexture texture = new GLTexture(width, height, data, GL.RGBA, GL.UNSIGNED_BYTE);
            return texture;
        }
    }

    public static GLTexture LoadFromBytes(byte[] bytes)
    {
        var image = ImageResult.FromMemory(bytes, ColorComponents.RedGreenBlueAlpha);
        return LoadFromRawBytes(image.Data, image.Width, image.Height);
    }

    public static GLTexture LoadFromPointer(void* ptr, int length)
    {
        UnmanagedMemoryStream stream = new UnmanagedMemoryStream((byte*)ptr, length);
        var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        fixed (byte* data = image.Data)
        {
            GLTexture texture = new GLTexture(image.Width, image.Height, data, GL.RGBA, GL.UNSIGNED_BYTE);
            return texture;
        }
    }

    public void GenerateMipmap()
    {
        Bind();
        glGenerateMipmap(GL.TEXTURE_2D);
    }

    public void SetMinMagFilter(GL min, GL mag)
    {
        Bind();
        glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, min);
        glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, mag);
    }

    public void SetWrapFilter(GL wrapS, GL wrapT)
    {
        Bind();
        glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, wrapS);
        glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, wrapT);
    }

    public void Bind()
    {
        glBindTexture(GL.TEXTURE_2D, handle);
    }

    public void BindAtLocation(uint location)
    {
        glActiveTexture(GL.TEXTURE0 + location);
        Bind();
    }

    public static void BindAtLocation(uint texHandle, uint location)
    {
        glActiveTexture(GL.TEXTURE0 + location);
        glBindTexture(GL.TEXTURE_2D, texHandle);
    }

    public void Free()
    {
        uint b = handle;
        glDeleteTextures(1, &b);
    }

    int GetImageFormatPixelSize()
    {
        if (internalFormat == GL.RGBA8)
        {
            return 4;
        }

        throw new Exception($"Unsupported type: {internalFormat}");
    }

    public TextureMapData Map()
    {
        void* data = glMapNamedBuffer(handle, GL.TEXTURE_2D);

        TextureMapData map_data = new TextureMapData()
        {
            left = 0,
            top = 0,
            width = width,
            height = height,
            stride = GetImageFormatPixelSize() * width
        };

        return map_data;
    }

    public T* Map<T>(GL ops = GL.WRITE_ONLY) where T : unmanaged
    {
        return (T*)glMapNamedBuffer(handle, ops);
    }

    public void Unmap()
    {
        glUnmapNamedBuffer(handle);
    }
}