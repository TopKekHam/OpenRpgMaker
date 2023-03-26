namespace SBEngine;


public class DrawCallMultiple : IDrawCallMultiple
{
    public uint DrawCount => (uint)First.Count;
    public ArrayList<int> First { get; private set; }
    public ArrayList<uint> Count { get; private set; }

    private int _first;
    
    public DrawCallMultiple()
    {
        First = new ArrayList<int>();
        Count = new ArrayList<uint>();
        _first = 0;
    }
    
    public void PushCall(int count)
    {
        First.Add(_first);
        Count.Add((uint)count);
        
        _first += count;
    }

    public void Clear()
    {
        First.Clear();
        Count.Clear();
        _first = 0;
    }
}