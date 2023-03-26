using StbImageSharp;
using static ThirdParty.OpenGL.OpenGL;
using SBEngine;

namespace ThirdParty.OpenGL;

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
    public bool reverse;

    GL internalFormat;

    OpenGL gl;

    public GLTexture(OpenGL gl)
    {
        this.gl = gl;
    }

    public GLTexture(OpenGL gl, int _width, int _height, byte* data, GL format, GL type, GL internalFormat = GL.RGBA8)
    {
        this.gl = gl;

        Load(_width, _height, data, format, type, internalFormat);
    }

    public GLTexture(OpenGL gl, int _width, int _height, byte[] data, GL format, GL type, GL internalFormat = GL.RGBA8)
    {
        this.gl = gl;

        fixed (byte* ptr = data)
        {
            Load(_width, _height, ptr, format, type, internalFormat);
        }
    }

    void Load(int _width, int _height, byte* data, GL format, GL type, GL internalFormat)
    {
        this.internalFormat = internalFormat;

        uint textureMap;
        gl.glGenTextures(1, &textureMap);
        gl.glBindTexture(GL.TEXTURE_2D, textureMap);

        gl.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, GL.LINEAR);
        gl.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, GL.LINEAR);
        gl.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, GL.CLAMP_TO_BORDER);
        gl.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, GL.CLAMP_TO_BORDER);

        gl.glTexImage2D(GL.TEXTURE_2D, 0, internalFormat, (uint)_width, (uint)_height, 0, format, type, data);

        gl.glBindTexture(GL.TEXTURE_2D, 0);

        width = _width;
        height = _height;
        handle = textureMap;
        reverse = false;
    }

    public static GLTexture LoadFromFile(OpenGL gl, string path)
    {
        var stream = File.OpenRead(path);
        var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        var flipedData = new byte[image.Data.Length];

        int stride = image.Width * sizeof(int);

        for (int i = 0; i < image.Height; i++)
        {
            Array.Copy(image.Data, i * stride, flipedData, (image.Height - i - 1) * stride, stride);
        }

        fixed (byte* data = flipedData)
        {
            GLTexture texture = new GLTexture(gl, image.Width, image.Height, data, GL.RGBA, GL.UNSIGNED_BYTE);
            stream.Close();
            return texture;
        }
    }

    public static GLTexture LoadFromRawBytes(OpenGL gl, byte[] bytes, int width, int height)
    {
        fixed (byte* data = bytes)
        {
            GLTexture texture = new GLTexture(gl, width, height, data, GL.RGBA, GL.UNSIGNED_BYTE);
            return texture;
        }
    }

    public static GLTexture LoadFromRawBytes(OpenGL gl, byte* bytes, int width, int height)
    {
        GLTexture texture = new GLTexture(gl, width, height, bytes, GL.RGBA, GL.UNSIGNED_BYTE);
        return texture;
    }
    
    public static GLTexture LoadFromBytes(OpenGL gl, byte[] bytes)
    {
        var image = ImageResult.FromMemory(bytes, ColorComponents.RedGreenBlueAlpha);
        return LoadFromRawBytes(gl, image.Data, image.Width, image.Height);
    }

    public static GLTexture LoadFromPointer(OpenGL gl, void* ptr, int length)
    {
        UnmanagedMemoryStream stream = new UnmanagedMemoryStream((byte*)ptr, length);
        var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        fixed (byte* data = image.Data)
        {
            GLTexture texture = new GLTexture(gl, image.Width, image.Height, data, GL.RGBA, GL.UNSIGNED_BYTE);
            return texture;
        }
    }

    public void GenerateMipmap()
    {
        Bind();
        gl.glGenerateMipmap(GL.TEXTURE_2D);
    }

    public void SetMinMagFilter(GL min, GL mag)
    {
        Bind();
        gl.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MIN_FILTER, min);
        gl.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_MAG_FILTER, mag);
    }

    public void SetWrapST(GL wrapS, GL wrapT)
    {
        Bind();
        gl.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_S, wrapS);
        gl.glTexParameteri(GL.TEXTURE_2D, GL.TEXTURE_WRAP_T, wrapT);
    }

    public void Bind()
    {
        gl.glBindTexture(GL.TEXTURE_2D, handle);
    }

    public void BindAtLocation(uint location)
    {
        gl.glActiveTexture(GL.TEXTURE0 + location);
        Bind();
    }

    public static void BindAtLocation(OpenGL openGL, uint texHandle, uint location)
    {
        openGL.glActiveTexture(GL.TEXTURE0 + location);
        openGL.glBindTexture(GL.TEXTURE_2D, texHandle);
    }

    public void Free()
    {
        uint b = handle;
        gl.glDeleteTextures(1, &b);
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
        void* data = gl.glMapNamedBuffer(handle, GL.TEXTURE_2D);

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
        return (T*)gl.glMapNamedBuffer(handle, ops);
    }

    public void Unmap()
    {
        gl.glUnmapNamedBuffer(handle);
    }
}