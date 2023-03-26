using System.Collections;
using ThirdParty.SDL;

namespace SBEngine;

public unsafe class NativeList<T> : IReadOnlyList<T> where T : unmanaged
{

    public T* Data => _dataPtr;
    public int SizeInBytes => _populatedLength * sizeof(T);
    
    private T* _dataPtr;
    private int _length;
    private int _populatedLength;
    private SDLMemoryOps _memoryOps;
    
    public NativeList(int length = 32)
    {
        _memoryOps = new SDLMemoryOps();
        _dataPtr = (T*)_memoryOps.Malloc((ulong)(sizeof(T) * length));
        _populatedLength = 0;
        _length = length;
    }

    public void Add(T item)
    {
        Add(ref item);
    }

    public void Add(ref T item)
    {
        if (_length == _populatedLength)
        {
            Resize();
        }

        _dataPtr[_populatedLength] = item;
        _populatedLength++;
    }
    
    public void RemoveAt(int index)
    {
        if (index == _populatedLength - 1)
        {
            _populatedLength -= 1;
            return;
        }
        else
        {
            int byteToCopy = (_populatedLength - index - 1) * sizeof(T);
            Buffer.MemoryCopy(_dataPtr + index + 1, _dataPtr + index, byteToCopy + sizeof(T), byteToCopy);
            _populatedLength -= 1;
        }
    }

    public void RemoveAtUnordered(int index)
    {
        if (index == _populatedLength - 1)
        {
            _populatedLength -= 1;
            return;
        }
        else
        {
            _populatedLength -= 1;
            _dataPtr[index] = _dataPtr[_populatedLength];
        }
    }

    public void SetSize(int elementCount)
    {
        _length = elementCount;
        _dataPtr = (T*)_memoryOps.Realloc(_dataPtr, (ulong)(sizeof(T) * _length));
        if (_dataPtr == null) throw new Exception("Couldn't Realloc list.");

        if (_populatedLength > _length)
        {
            _populatedLength = _length;
        }
    }

    public void Clear()
    {
        _populatedLength = 0;
    }
    
    void Resize()
    {
        _length *= 2;
        _dataPtr = (T*)_memoryOps.Realloc(_dataPtr, (ulong)(sizeof(T) * _length));
        if (_dataPtr == null) throw new Exception("Couldn't Realloc list.");
    }

    
    // IReadOnlyList

    public int Count => _populatedLength;

    public T this[int index]
    {
        get => _dataPtr[index];
        set => _dataPtr[index] = value;
    }

    public class NativeListEnumerator<T> : IEnumerator<T> where T : unmanaged
    {
        NativeList<T> _list;
        private int _current = 0;
        
        public NativeListEnumerator(NativeList<T> list)
        {
            _list = list;
            _current = 0;
        }
        
        public bool MoveNext()
        {
            _current += 1;

            return _current > _list._populatedLength;
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
    
    public IEnumerator<T> GetEnumerator()
    {
        return new NativeListEnumerator<T>(this);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new NativeListEnumerator<T>(this);
    }
}