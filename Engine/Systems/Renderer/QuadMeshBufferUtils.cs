using System.Numerics;

namespace SBEngine;

public static class QuadMeshBufferUtils
{

    public static int AddTextQuads(this QuadMeshBuffer buffer, TrueTypeFont font, string text, Vector3 position, float size, Vector4 color, int textureSlot = 0)
    {
        float lineSize = (font.Ascent + font.LineGap - font.Descent) * size;
        Vector2 origin = position.XY() - new Vector2(0, font.Ascent * size);
        int numberOfQuads = 0;
        
        for (int i = 0; i < text.Length; i++)
        {
            
            char c = text[i];
            
            if (font.Glyphs.TryGetValue(c, out var glyph) == false)
            {
                Console.WriteLine($"{c} character doesn't exists in font.");
                continue;
            }

            // basic whitespace handling
            {
                if (c == ' ')
                {
                    origin.X += font.Glyphs[' '].AdvanceX * size;
                    continue;
                }
            
                if (c == '\t')
                {
                    origin.X += font.Glyphs[' '].AdvanceX * 4 * size;
                    continue;
                }
            
                if (c == '\n')
                {
                    origin.X = position.X;
                    origin.Y -= lineSize;
                    continue;
                }

                if (c == '\r')
                {
                    origin.X = position.X;
                    continue;
                }
            }
            
            var glyphSize = glyph.Size * size; 
            Vector2 halfSize = glyphSize / 2f;
            var posV2 = origin + (new Vector2(glyph.LeftSideBearing, glyph.BaseLineOffset) * size + halfSize);

            var pos = new Vector3(posV2, position.Z);
            
            buffer.AddTexturedQuad(ref pos, ref glyphSize, ref color, 
                ref glyph.UvBottomLeft, ref glyph.UvTopRight, textureSlot);

            numberOfQuads += 1;
            
            origin += new Vector2(glyph.AdvanceX * size, 0);
        }


        return numberOfQuads;
    }

    public static Box GetTextQuadsBoundingBox(TrueTypeFont font, string text, float size)
    {
        float lineSize = (font.Ascent + font.LineGap - font.Descent) * size;
        Vector2 position = Vector2.Zero;
        Vector2 origin = position - new Vector2(0, font.Ascent * size);

        Box boundingBox = new Box(Vector2.Zero, Vector2.Zero);

        for (int i = 0; i < text.Length; i++)
        {

            char c = text[i];

            if (font.Glyphs.TryGetValue(c, out var glyph) == false)
            {
                Console.WriteLine($"{c} character doesn't exists in font.");
                continue;
            }

            // basic whitespace handling
            {
                if (c == ' ')
                {
                    origin.X += font.Glyphs[' '].AdvanceX * size;
                    boundingBox.ExpandToContain(origin);
                    continue;
                }

                if (c == '\t')
                {
                    origin.X += font.Glyphs[' '].AdvanceX * 4 * size;
                    boundingBox.ExpandToContain(origin);
                    continue;
                }

                if (c == '\n')
                {
                    origin.X = position.X;
                    origin.Y += (-font.Ascent - font.LineGap + font.Descent) * size;
                    boundingBox.ExpandToContain(origin);
                    continue;
                }

                if (c == '\r')
                {
                    origin.X = position.X;
                    boundingBox.ExpandToContain(origin);
                    continue;
                }
            }

            var glyphSize = glyph.Size * size;
            Vector2 halfSize = glyphSize / 2f;
            var pos = origin + (new Vector2(glyph.LeftSideBearing, glyph.BaseLineOffset) * size + halfSize);
            var color = Vector4.One;

            Vector2 topLeft = pos + new Vector2(-halfSize.X, halfSize.Y);
            Vector2 topRight = pos + new Vector2(halfSize.X, halfSize.Y);
            Vector2 bottomLeft = pos + new Vector2(-halfSize.X, -halfSize.Y);
            Vector2 bottomRight = pos + new Vector2(halfSize.X, -halfSize.Y);

            boundingBox.ExpandToContain(topLeft);
            boundingBox.ExpandToContain(topRight);
            boundingBox.ExpandToContain(bottomLeft);
            boundingBox.ExpandToContain(bottomRight);

            origin += new Vector2(glyph.AdvanceX * size, 0);
        }

        return boundingBox;
    }
}