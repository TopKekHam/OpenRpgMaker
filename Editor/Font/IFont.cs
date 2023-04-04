using System.Numerics;

public interface IFont
{
    void GetCharacterUvs(ref Vector2 uv0, ref Vector2 uv1);
    void CalcCharacterQuad(ref Vector2 position, char c, float size, ref Vector2 bottom, ref Vector2 top);

}
