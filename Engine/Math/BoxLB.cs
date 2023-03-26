using System.Numerics;

namespace SBEngine;

// bottom left aligned
[Serializable]
public struct BoxLB
{
    public float left, bottom, width, height;
    public float top { get => bottom + height; set => bottom = value - height; }
    public float right { get => left + width; set => left = value - width; }
    public Vector2 topLeft { get => new Vector2(left, top); }
    public Vector2 topRight { get => new Vector2(right, top); }
    public Vector2 bottomRight { get => new Vector2(right, bottom); }
    public Vector2 bottomLeft { get => new Vector2(left, bottom); }
    public float centerH => left + (width / 2f);
    public float centerV => bottom + (height / 2f);
    public Vector2 center => new Vector2(centerH, centerV);
    public Vector3 centerV3 => new Vector3(centerH, centerV, 0);
    public Vector2 size { get => new Vector2(width, height); }
    public Vector2 halfSize { get => new Vector2(width / 2f, height / 2f); }

    public float aspectRatio => width / height;

    public BoxLB(float _left, float _bottom, float _width, float _height)
    {
        left = _left;
        bottom = _bottom;
        width = _width;
        height = _height;
    }

    public BoxLB(float _width, float _height)
    {
        left = 0;
        bottom = 0;
        width = _width;
        height = _height;
    }

    public BoxLB(Vector2 size)
    {
        left = 0;
        bottom = 0;
        width = size.X;
        height = size.Y;
    }

    public BoxLB(Vector2 _center, Vector2 _half_size)
    {
        left = _center.X - _half_size.X;
        bottom = _center.Y - _half_size.Y;
        width = _half_size.X * 2f;
        height = _half_size.Y * 2f;
    }

    public void Scale(float scale)
    {
        left *= scale;
        top *= scale;
        width *= scale;
        height *= scale;
    }

    public void StrechV(float _top, float _bottom)
    {
        bottom = _bottom;
        height = _top - _bottom;
    }

    public void StrechV(BoxLB boxLb)
    {
        bottom = boxLb.bottom;
        height = boxLb.height;
    }

    public void StrechH(float _left, float _right)
    {
        left = _left;
        width = _right - _left;
    }

    public void StrechH(BoxLB boxLb)
    {
        left = boxLb.left;
        width = boxLb.width;
    }

    public void ExpandToContain(float x, float y)
    {
        if (x < left)
        {
            width += MathF.Abs(x - left);
            left = x;
        }
        else if (right < x)
        {
            width = MathF.Abs(x - left);
        }

        if (y < bottom)
        {
            height += MathF.Abs(y - bottom);
            bottom = y;
        }
        else if (top < y)
        {
            height = MathF.Abs(y - bottom);
        }
    }

    // Sets to left side of split
    // Returns the right size of the split
    public BoxLB SplitH(float newWidth)
    {
        BoxLB rightSide = this;
        rightSide.ResizeSide(BoxSide.LEFT, -newWidth);

        this.width = newWidth;

        return rightSide;
    }

    public void ExpandToContain(Vector2 vec)
    {
        ExpandToContain(vec.X, vec.Y);
    }

    public void ExpandToContainBox(BoxLB boxLb)
    {
        ExpandToContain(boxLb.topLeft);
        ExpandToContain(boxLb.topRight);
        ExpandToContain(boxLb.bottomLeft);
        ExpandToContain(boxLb.bottomRight);
    }

    public void Move(float x, float y)
    {
        left += x;
        bottom += y;
    }

    public void Move(Vector2 vec)
    {
        left += vec.X;
        bottom += vec.Y;
    }

    public bool ContainsPoint(Vector2 vec)
    {
        return (left <= vec.X && right >= vec.X && bottom <= vec.Y && top >= vec.Y);
    }

    public BoxSide ContainsPointOnBorder(Vector2 point, float border_size)
    {
        bool left_border = point.X >= left && point.X <= left + border_size;
        bool right_border = point.X <= right && point.X >= right - border_size;
        bool top_border = point.Y <= top && point.Y >= top - border_size;
        bool bottom_border = point.Y >= bottom && point.Y <= bottom + border_size;

        BoxSide sides = BoxSide.NONE;

        if (ContainsPoint(point))
        {
            if (left_border) sides |= BoxSide.LEFT;
            if (right_border) sides |= BoxSide.RIGHT;
            if (top_border) sides |= BoxSide.TOP;
            if (bottom_border) sides |= BoxSide.BOTTOM;
        }

        return sides;
    }

    public void AlignTo(BoxLB boxLb, Alignment alignment)
    {
        if (alignment.HasFlag(Alignment.LEFT))
        {
            left = boxLb.left;
        }
        else if (alignment.HasFlag(Alignment.HCENTER))
        {
            left = (boxLb.left + (boxLb.width / 2f)) - (width / 2f);
        }
        else if (alignment.HasFlag(Alignment.RIGHT))
        {
            right = boxLb.right;
        }

        if (alignment.HasFlag(Alignment.TOP))
        {
            top = boxLb.top;
        }
        else if (alignment.HasFlag(Alignment.VCENTER))
        {
            bottom = (boxLb.bottom + (boxLb.height / 2f)) - (height / 2f);
        }
        else if (alignment.HasFlag(Alignment.BOTTOM))
        {
            bottom = boxLb.bottom;
        }
    }

    public void ResizeSide(BoxSide side, float amount)
    {

        if (side.HasFlag(BoxSide.RIGHT))
        {
            width += amount;
        }

        if (side.HasFlag(BoxSide.LEFT))
        {
            width += amount;
            left -= amount;
        }

        if (side.HasFlag(BoxSide.BOTTOM))
        {
            height += amount;
            bottom -= amount;
        }

        if (side.HasFlag(BoxSide.TOP))
        {
            height += amount;
        }
    }

    //does not work like ResizeSide(BoxSide, float)
    public void ResizeSide(BoxSide side, Vector2 amount)
    {

        if (side.HasFlag(BoxSide.RIGHT))
        {
            width -= amount.X;
        }

        if (side.HasFlag(BoxSide.LEFT))
        {
            width += amount.X;
            left -= amount.X;
        }

        if (side.HasFlag(BoxSide.BOTTOM))
        {
            height += amount.Y;
            bottom -= amount.Y;
        }

        if (side.HasFlag(BoxSide.TOP))
        {
            height += amount.Y;
        }
    }

    public float ResizeSideClamped(BoxSide side, float amount, float min, float max)
    {
        if (side.HasFlag(BoxSide.LEFT))
        {
            float prev = width;
            width += amount;
            width = MathEx.Clamp(width, min, max);

            amount = width - prev;
            left -= amount;

            return amount;
        }

        if (side.HasFlag(BoxSide.RIGHT))
        {
            float prev = width;
            width += amount;
            width = MathEx.Clamp(width, min, max);

            amount = width - prev;

            return amount;
        }

        if (side.HasFlag(BoxSide.TOP))
        {
            float prev = height;
            height += amount;
            height = MathEx.Clamp(height, min, max);

            amount = height - prev;

            return amount;
        }

        if (side.HasFlag(BoxSide.BOTTOM))
        {
            float prev = height;
            height += amount;
            height = MathEx.Clamp(height, min, max);

            amount = height - prev;
            bottom -= amount;

            return amount;
        }

        return 0;
    }

    public bool ContainsBox(BoxLB boxLb)
    {
        var c = center;
        var cw = width / 2f;
        var ch = height / 2f;

        var c2 = boxLb.center;
        var cw2 = boxLb.width / 2f;
        var ch2 = boxLb.height / 2f;

        return !((MathF.Abs(c.X - c2.X) > (cw + cw2)) ||
               (MathF.Abs(c.Y - c2.Y) > (ch + ch2)));
    }

    public void MaximizeInRatio(BoxLB boxLb)
    {
        if (aspectRatio == boxLb.aspectRatio)
        {
            width = boxLb.width;
            height = boxLb.height;
        }
        else if (aspectRatio > boxLb.aspectRatio)
        {
            height = boxLb.width / aspectRatio;
            width = boxLb.width;
        }
        else
        {
            width = boxLb.height * aspectRatio;
            height = boxLb.height;
        }

    }

}
