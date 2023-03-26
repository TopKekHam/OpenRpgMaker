using System.Numerics;

namespace SBEngine;

public struct Box
{

    public Vector2 Position;
    public Vector2 Size;

    public float HalfWidth => Size.X / 2f;
    public float HalfHeight => Size.Y / 2f;
    
    public float Right => Position.X + (Size.X / 2f);
    public float Left => Position.X - (Size.X / 2f);
    public float Top => Position.Y + (Size.Y / 2f);
    public float Bottom => Position.Y - (Size.Y / 2f);

    public Vector2 TopLeft => new Vector2(Left, Top);
    
    public Box(Vector2 position, Vector2 size)
    {
        Position = position;
        Size = size;
    }
    
    public void ChangeSideLength(float length, BoxSide side)
    {

        if ((side & BoxSide.HORIZONTAL) == BoxSide.HORIZONTAL)
        {
            Size.X += length;
        }
        
        if ((side & BoxSide.VERTICAL) == BoxSide.VERTICAL)
        {
            Size.Y += length;
        }

        if ((side & BoxSide.LEFT) == BoxSide.LEFT)
        {
            Size.X += length;
            Position.X -= length / 2f;
        }
        
        if ((side & BoxSide.RIGHT) == BoxSide.RIGHT)
        {
            Size.X += length;
            Position.X += length / 2f;
        }
        
        if ((side & BoxSide.TOP) == BoxSide.TOP)
        {
            Size.Y += length;
            Position.Y += length / 2f;
        }
        
        if ((side & BoxSide.BOTTOM) == BoxSide.BOTTOM)
        {
            Size.Y += length;
            Position.Y -= length / 2f;
        }
    }

    public Box AlignInside(Box box, Alignment alignment)
    {
        AlignInside(ref box, alignment);
        return box;
    }
    
    public void AlignInside(ref Box box, Alignment alignment)
    {
        
        if ((alignment & Alignment.HCENTER) == Alignment.HCENTER)
        {
            box.Position.X = Position.X;
        }
        
        if ((alignment & Alignment.VCENTER) == Alignment.VCENTER)
        {
            box.Position.Y = Position.Y;
        }
        
        if ((alignment & Alignment.TOP) == Alignment.TOP)
        {
            box.Position.Y = Top - box.HalfHeight;
        }

        if ((alignment & Alignment.BOTTOM) == Alignment.BOTTOM)
        {
            box.Position.Y = Bottom + box.HalfHeight;
        }
        
        if ((alignment & Alignment.RIGHT) == Alignment.RIGHT)
        {
            box.Position.X = Right - box.HalfWidth;
        }

        if ((alignment & Alignment.LEFT) == Alignment.LEFT)
        {
            box.Position.X = Left + box.HalfWidth;
        }
    }

    public bool IsPointInside(Vector2 point)
    {
        var halfSize = (point - Position).Abs();
        return halfSize.X <= HalfWidth && halfSize.Y <= HalfHeight;
    }
    
    public void ExpandToContain(Vector2 point)
    {

        if (point.X > Right)
        {
            ChangeSideLength(MathF.Abs(point.X - Right), BoxSide.RIGHT);
        }
     
        else if (point.X < Left)
        {
            ChangeSideLength(MathF.Abs(point.X - Left), BoxSide.LEFT);
        }
        
        if (point.Y > Top)
        {
            ChangeSideLength(MathF.Abs(point.Y - Top), BoxSide.TOP);
        }
        
        else if (point.Y < Bottom)
        {
            ChangeSideLength(MathF.Abs(point.Y - Bottom), BoxSide.BOTTOM);
        }
        
    }
    
}