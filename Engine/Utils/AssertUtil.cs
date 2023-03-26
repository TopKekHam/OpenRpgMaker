using System.Runtime.CompilerServices;

namespace SBEngine;

public class AssertUtil
{
    
    public static void Assert(bool condition)
    {
        if (condition == false)
        {
            throw new Exception("Assertion failed!");
        }
    }
    
    public static void Assert(bool condition, string message)
    {
        if (condition == false)
        {
            throw new Exception($"Assertion failed! - {message}");
        }
    }
}