using System.Numerics;
using System.Runtime.InteropServices;
using ThirdParty.OpenGL;
using static StbTrueTypeSharp.StbTrueType;

namespace SBEngine;

[StructLayout(LayoutKind.Explicit)]
public struct RGBA8Color
{
    [FieldOffset(0)] public byte R;
    [FieldOffset(1)] public byte G;
    [FieldOffset(2)] public byte B;
    [FieldOffset(3)] public byte A;

    [FieldOffset(0)] public uint Data;

    public RGBA8Color(byte r, byte g, byte b, byte a)
    {
        Data = 0;
        R = r;
        G = g;
        B = b;
        A = a;
    }
}

public unsafe class TrueTypeFont
{
    public struct Glyph
    {
        public Vector2 UvBottomLeft, UvTopRight;
        public Vector2 Size;
        public int WidthInPixels, HeightInPixels;
        public float AdvanceX, LeftSideBearing, BaseLineOffset;
    }

    public GLTexture Texture { get; private set; }
    public Dictionary<char, Glyph> Glyphs { get; private set; }
    public float Ascent { get; private set; }
    public float Descent { get; private set; }
    public float LineGap { get; private set; }

    private stbtt_fontinfo _info;
    private byte[] _fileData;
    private OpenGL _gl;
    private float _scale;

    public TrueTypeFont(OpenGL gl, string path)
    {
        _gl = gl;
        _info = new stbtt_fontinfo();

        _fileData = File.ReadAllBytes(path);
        fixed (byte* ptr = _fileData)
        {
            var offset = stbtt_GetFontOffsetForIndex(ptr, 0);
            stbtt_InitFont(_info, ptr, offset);
        }

        Glyphs = new Dictionary<char, Glyph>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="charsToBake"></param>
    /// <param name="glyphSize"></param>
    /// <exception cref="Exception">Can throw exception if the texture is to small to hold all of the glyphs.</exception>
    public void BakeGlyphs(char[] charsToBake, int glyphSize)
    {
        Glyphs.Clear();

        _scale = stbtt_ScaleForPixelHeight(_info, glyphSize);

        int textureSize = 1024;
        uint* textureBuffer = (uint*)Marshal.AllocHGlobal(textureSize * textureSize * 4).ToPointer();

        int textureOffsetX = 0;
        int textureOffsetY = 0;

        int cellX = 0;
        int cellY = 0;

        Vector2 cellSizeInUv = new Vector2((float)textureSize / glyphSize);

        for (int i = 0; i < charsToBake.Length; i++)
        {
            char codePoint = charsToBake[i];

            int width = 0, height = 0, offX = 0, offY = 0;
            byte* buffer = stbtt_GetCodepointBitmap(_info, _scale, _scale, codePoint, &width, &height, &offX, &offY);

            int ascent = 0, descent = 0, lineGap = 0;
            stbtt_GetFontVMetrics(_info, &ascent, &descent, &lineGap);
            Ascent  = ascent  * _scale / glyphSize;
            Descent = descent * _scale / glyphSize;
            LineGap = lineGap * _scale / glyphSize;

            int advanceX = 0;
            int leftSideBearing = 0;
            stbtt_GetCodepointHMetrics(_info, codePoint, &advanceX, &leftSideBearing);

            Glyph glyph = new Glyph();
            glyph.AdvanceX = advanceX * _scale / glyphSize;
            glyph.LeftSideBearing = leftSideBearing * _scale / glyphSize;
            glyph.UvBottomLeft = new Vector2(cellX, cellY) / cellSizeInUv ;
            glyph.UvTopRight = new Vector2(cellX, cellY) / cellSizeInUv 
                               + new Vector2((float)width / textureSize, (float)height / textureSize);

            glyph.Size = new Vector2(width / (float)glyphSize, height / (float)glyphSize);
            glyph.WidthInPixels = width;
            glyph.HeightInPixels = height;

            if (height != MathF.Abs(offY))
            {
                int diff = height + offY;
                glyph.BaseLineOffset = diff * -1.0f / glyphSize; // for now this is 0 i dont remember how to calculate this value (Sadge).
            }

            Glyphs.Add(codePoint, glyph);

            int offset = textureOffsetX + (textureOffsetY * textureSize);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = x + (y * width);
                    byte val = buffer[idx];

                    RGBA8Color color = new RGBA8Color(val, val, val, val);

                    int idxT = offset + x + ((height - 1 - y) * textureSize);
                    textureBuffer[idxT] = color.Data;
                }
            }

            textureOffsetX += glyphSize;
            cellX += 1;

            if (textureSize - (textureOffsetX + glyphSize) < glyphSize)
            {
                textureOffsetX = 0;
                cellX = 0;

                textureOffsetY += glyphSize;
                cellY += 1;

                if (textureSize - (textureOffsetY + glyphSize) < glyphSize)
                {
                    throw new Exception("Texture is not big enough to hold all of the glyphs.");
                }
            }
        }

        Texture = GLTexture.LoadFromRawBytes(_gl, (byte*)textureBuffer, textureSize, textureSize);

        Marshal.FreeHGlobal(new IntPtr(textureBuffer));
    }

    public float GetKerning(char cPrev, char cNow)
    {
        return stbtt_GetCodepointKernAdvance(_info, cPrev, cNow) * _scale;
    }
}