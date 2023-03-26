namespace SBEngine.Editor;

public class Parser
{
    public static bool ParseInt(string str, ref int value)
    {
        if (int.TryParse(str, out var val))
        {
            value = val;
            return true;
        }

        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            value = 0;
            return true;
        }
        
        return false;
    }
    
    public static bool ParseFloat(string str, ref float value)
    {
        if (float.TryParse(str, out var val))
        {
            value = val;
            return true;
        }

        if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str))
        {
            value = 0;
            return true;
        }
        
        return false;
    }

}