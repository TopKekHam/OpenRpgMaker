using System.Numerics;

namespace SBEngine;

public struct HSLColor
{
    public float hue;
    public float saturation;
    public float lightness;
}

public struct HSVColor
{
    // all values normalized
    public float hue;
    public float saturation;
    public float value;

    public HSVColor(float _hue, float _saturation, float _value)
    {
        hue = _hue;
        saturation = _saturation;
        value = _value;
    }

    public static bool operator ==(HSVColor c1, HSVColor c2)
    {
        return c1.hue == c2.hue && c1.saturation == c2.saturation && c1.value == c2.value;
    }

    public static bool operator !=(HSVColor c1, HSVColor c2)
    {
        return !(c1 == c2);
    }
}

public static class VColors
{
    public static readonly Vector4 Red = new Vector4(1, 0, 0, 1);
    public static readonly Vector4 Green = new Vector4(0, 1, 0, 1);
    public static readonly Vector4 Blue = new Vector4(0, 0, 1, 1);
    public static readonly Vector4 Black = new Vector4(0, 0, 0, 1);
    public static readonly Vector4 White = new Vector4(1, 1, 1, 1);
    public static readonly Vector4 LightGrey = new Vector4(0.85f, 0.85f, 0.85f, 1);
    public static readonly Vector4 Grey = new Vector4(0.5f, 0.5f, 0.5f, 1);
    public static readonly Vector4 DarkGrey = new Vector4(0.15f, 0.15f, 0.15f, 1);
    public static readonly Vector4 LightBlue = new Vector4(0.5f, 0.7f, 1, 1);
    public static readonly Vector4 Orange = new Vector4(1.0f, 0.5f, 0.0f, 1);
    public static readonly Vector4 Magenta = new Vector4(1, 0, 1, 1);

    public static HSLColor RGBToHSL(Vector3 rgb)
    {
        HSLColor color = new HSLColor();

        float cmax = MathF.Max(MathF.Max(rgb.X, rgb.Y), rgb.Z);
        float cmin = MathF.Min(MathF.Min(rgb.X, rgb.Y), rgb.Z);

        float delta = cmax - cmin;

        // hue

        if (delta == 0)
        {
            color.hue = 0;
        }
        else if (cmax == rgb.X)
        {
            color.hue = ((rgb.Y - rgb.Z) / delta) % 6f;
        }
        else if (cmax == rgb.Y)
        {
            color.hue = ((rgb.Z - rgb.X) / delta) + 2f;
        }
        else
        {
            color.hue = ((rgb.X - rgb.Y) / delta) + 4f;
        }

        color.hue *= 60;

        // lightness

        color.lightness = (cmax - cmin) / 2f;

        // saturation

        if (delta == 0)
        {
            color.saturation = 0;
        }
        else
        {
            color.saturation = delta / (1f - MathF.Abs(2f * color.lightness - 1));
        }

        return color;
    }

    public static HSVColor RGBToHSV(Vector3 rgb)
    {
        HSVColor color = new HSVColor();

        float cmax = MathF.Max(MathF.Max(rgb.X, rgb.Y), rgb.Z);
        float cmin = MathF.Min(MathF.Min(rgb.X, rgb.Y), rgb.Z);

        float delta = cmax - cmin;

        // hue

        if (delta == 0)
        {
            color.hue = 0;
        }
        else if (cmax == rgb.X)
        {
            color.hue = ((rgb.Y - rgb.Z) / delta) % 6f;
        }
        else if (cmax == rgb.Y)
        {
            color.hue = ((rgb.Z - rgb.X) / delta) + 2f;
        }
        else
        {
            color.hue = ((rgb.X - rgb.Y) / delta) + 4f;
        }

        color.hue *= 60;

        // value

        color.value = cmax;

        // saturation

        if (delta == 0)
        {
            color.saturation = 0;
        }
        else
        {
            color.saturation = delta / cmax;
        }

        return color;
    }

    public static Vector3 HSVToRGB(HSVColor color)
    {

        float c = color.value * color.saturation;

        float x = c * (1f - MathF.Abs(color.hue / (60f / 360f) % 2 - 1f));

        float m = color.value - c;

        Vector3 rgb_color = Vector3.Zero;

        if (0 <= color.hue && color.hue <= 60f / 360f)
        {
            rgb_color = new Vector3(c, x, 0);
        }
        else if (60f / 360f <= color.hue && color.hue <= 120f / 360f)
        {
            rgb_color = new Vector3(x, c, 0);
        }
        else if (120f / 360f <= color.hue && color.hue <= 180f / 360f)
        {
            rgb_color = new Vector3(0, c, x);
        }
        else if (180f / 360f <= color.hue && color.hue <= 240f / 360f)
        {
            rgb_color = new Vector3(0, x, c);
        }
        else if (240f / 360f <= color.hue && color.hue <= 300f / 360f)
        {
            rgb_color = new Vector3(x, 0, c);
        }
        else if (300f / 360f <= color.hue && color.hue <= 360f / 360f)
        {
            rgb_color = new Vector3(c, 0, x);
        }

        return rgb_color + new Vector3(m);
    }

    public static Vector4 HSVToRGBA(HSVColor color)
    {
        return new Vector4(HSVToRGB(color), 1f);
    }

    public static Vector4 Lerp(Vector4 color1, Vector4 color2, float normal, bool backwords = false)
    {
        if (backwords)
        {
            return Vector4.Lerp(color2, color1, normal);
        }
        else
        {
            return Vector4.Lerp(color1, color2, normal);
        }
    }
}