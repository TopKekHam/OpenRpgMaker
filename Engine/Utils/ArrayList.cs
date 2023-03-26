using System.Collections;

namespace SBEngine;

public struct ArrayListEnumerator<T> : IEnumerator<T>
{
    private ArrayList<T> _list;
    private int _current;
    
    public ArrayListEnumerator(ArrayList<T> list)
    {
        _list = list;
        _current = -1;
    }

    public bool MoveNext()
    {
        _current++;
        return (_current < _list.Count);
    }

    public void Reset()
    {
        _current = 0;
    }

    public T Current { get => _list[_current]; }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
        
    }
}

public class ArrayList<T> : IReadOnlyList<T>
{
    public T[] Array;
    [SerializedField] private int _populatedItems;

    public int Count => _populatedItems;
    
    public T this[int index]
    {
        get => Array[index];
        set => Array[index] = value;
    }

    public ArrayList(int capacity = 4)
    {
        Array = new T[capacity];
    }

    public ref T GetRef(int idx)
    {
        return ref Array[idx];
    }
    
    public int Add(T item)
    {
        // grow if needed
        if (Array.Length == _populatedItems)
        {
            System.Array.Resize(ref Array, Array.Length * 2);
        }

        Array[_populatedItems] = item;
        _populatedItems += 1;

        return _populatedItems - 1;
    }

    public void Clear()
    {
        _populatedItems = 0;
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        return new ArrayListEnumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}