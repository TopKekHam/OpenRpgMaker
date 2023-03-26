using System.Collections;

namespace SBEngine;


public struct KeyValueArrayListElement<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
    
    public KeyValueArrayListElement(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

public struct KeyValueArrayListEnumerator<TKey, TValue> : IEnumerator<KeyValueArrayListElement<TKey, TValue>> where TKey : IEquatable<TKey>
{
    private KeyValueArrayList<TKey, TValue> _list;
    private int _current;
    
    public KeyValueArrayListEnumerator(KeyValueArrayList<TKey, TValue> list)
    {
        _list = list;
        _current = -1;
    }

    public bool MoveNext()
    {
        _current++;
        return (_current < _list.Keys.Count);
    }

    public void Reset()
    {
        _current = 0;
    }

    public KeyValueArrayListElement<TKey, TValue> Current { get => 
        new KeyValueArrayListElement<TKey, TValue>(_list.Keys[_current], _list.Values[_current]); 
    }

    object IEnumerator.Current => Current;

    public void Dispose()
    {
    }
}

public class KeyValueArrayList<TKey, TValue> : IEnumerable<KeyValueArrayListElement<TKey, TValue>> where TKey : IEquatable<TKey>
{

    public ArrayList<TKey> Keys;
    public ArrayList<TValue> Values;

    public KeyValueArrayList()
    {
        Keys = new ArrayList<TKey>();
        Values = new ArrayList<TValue>();
    }

    public TValue this[TKey key]
    {
        get => Get(key);
        set => Set(key, value);
    }

    public TValue Get(TKey key)
    {
        for (int i = 0; i < Keys.Count; i++)
        {
            if (Keys[i].Equals(key))
            {
                return Values[i];
            }
        }

        throw new Exception($"Value for key {key} not found.");
    }
    
    public void Set(TKey key, TValue value)
    {
        for (int i = 0; i < Keys.Count; i++)
        {
            if (Keys[i].Equals(key))
            {
                Values[i] = value;
                return;
            }
        }

        Add(key, value);
    }

    public void Add(TKey key, TValue value)
    {
        for (int i = 0; i < Keys.Count; i++)
        {
            if (Keys[i].Equals(key))
            {
                throw new Exception("Key taken.");
            }
        }

        Keys.Add(key);
        Values.Add(value);
    }
    
    public IEnumerator<KeyValueArrayListElement<TKey, TValue>> GetEnumerator()
    {
        return new KeyValueArrayListEnumerator<TKey, TValue>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}