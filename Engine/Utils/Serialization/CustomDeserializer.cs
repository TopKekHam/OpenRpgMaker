using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using static SBEngine.AssertUtil;

namespace SBEngine.Serialization;

public interface ISetter
{
    void Set(object value);
}

public struct FieldSetter : ISetter
{
    private object _obj;
    private FieldInfo _fieldInfo;

    public FieldSetter(object obj, FieldInfo fieldInfo)
    {
        _obj = obj;
        _fieldInfo = fieldInfo;
    }

    public void Set(object value)
    {
        _fieldInfo.SetValue(_obj, value);
    }
}

public struct ArraySetter : ISetter
{
    public ArraySetter(Array array, int index)
    {
        _array = array;
        _index = index;
    }

    private Array _array;
    private int _index;

    public void Set(object value)
    {
        _array.SetValue(value, _index);
    }
}

public class CustomDeserializer
{
    // Cache state
    private Dictionary<Type, Dictionary<string, FieldInfo>> _objectFields;
    private Assembly[] _assemblies;

    // State to clear after deserialization
    private Dictionary<int, object> _deserializedObjects;
    private Dictionary<int, List<ISetter>> _fieldsToResolve;
    private Dictionary<int, Type> _indexToType;
    private int _i;
    private string _src;

    public CustomDeserializer()
    {
        // Caching
        
        _objectFields = new Dictionary<Type, Dictionary<string, FieldInfo>>();
        _assemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        // Clearing state
        _i = 0;
        _deserializedObjects = new Dictionary<int, object>();
        _fieldsToResolve = new Dictionary<int, List<ISetter>>();
        _indexToType = new Dictionary<int, Type>();
    }

    public T Deserialize<T>(string src) where T : class
    {
        //clear state
        _i = 0;
        _fieldsToResolve.Clear();
        _deserializedObjects.Clear();
        _indexToType.Clear();
        
        _src = src;

        DeserializeTypeTable();
        
        RemoveDelimiters();
        int temp = _i;
        int rootHash = ObjectHash();
        _i = temp;

        while (_src.Length > _i)
        {
            DeserializeObject();
            RemoveDelimiters();
        }

        return _deserializedObjects[rootHash] as T;
    }

    void DeserializeTypeTable()
    {
        while (_src.Length > _i && _src[_i] != '@')
        {
            RemoveDelimiters();

            int index = GetNextInt();
            RemoveDelimiters();
            Type type = DeserializeType();
            
            _indexToType.Add(index, type);
            
            RemoveDelimiters();
        }
    }
    
    Type DeserializeType()
    {
        int start = _i;

        while (_src.Length > _i && IsDelimiter(_src[_i]) == false
                                && _src[_i] != '<' && _src[_i] != '>'
                                && _src[_i] != '[' && _src[_i] != ']')
        {
            _i++;
        }

        string name = _src.Substring(start, _i - start);
        var type = GetTypeFromName(name);
        
        RemoveDelimiters();

        if (_src[_i] == '<') // generic type
        {
            _i++;

            List<Type> args = new List<Type>();

            while (_src.Length > _i && _src[_i] != '>')
            {
                RemoveDelimiters();
                var argType = DeserializeType();
                args.Add(argType);
                RemoveDelimiters();
            }

            type = type.MakeGenericType(args.ToArray());
            _i++;
        }
        else if (_src[_i] == '[') // array type
        {
            _i++;
            RemoveDelimiters();
#if DEBUG
            Assert(_src[_i] == ']');
#endif

            _i++;

            type = type.MakeArrayType();
        }

        return type;
    }

    Type GetTypeFromName(string name)
    {
        for (int i = 0; i < _assemblies.Length; i++)
        {

            Type? type = _assemblies[i].GetType(name);

            if (type != null)
            {
                return type;
            }
        }

        throw new Exception($"Type {name} not found");
    }

    
    Dictionary<string, FieldInfo> GetObjectFields(Type type)
    {
        if (_objectFields.TryGetValue(type, out var objectFields) == false)
        {
            objectFields = new Dictionary<string, FieldInfo>();
            _objectFields.Add(type, objectFields);

            var fields = type.GetRuntimeFields();

            foreach (var field in fields)
            {
                if (field.IsPublic)
                {
                    objectFields.Add(field.Name, field);
                }
                else
                {
                    var attr = field.GetCustomAttribute<SerializedFieldAttribute>();

                    if (attr != null)
                    {
                        objectFields.Add(field.Name, field);
                    }
                }
            }
        }

        return objectFields;
    }
    
    object DeserializeObject()
    {
        int hash = DeserializeObjectHeader(out var type);

        object value = null;

        if (type == typeof(string))
        {
            RemoveDelimiters();
            int stringLength = GetNextInt();

            RemoveDelimiters();
            string? stringVal = _src.Substring(_i, stringLength);
            _i += stringLength;
            RemoveDelimiters();

            value = stringVal;
        }
        else if (type.IsArray)
        {
            value = DeserializeArray(type);
        }
        else if (type.IsClass)
        {
            value = DeserializeClass(type);
        }
        else
        {
            throw new Exception($"Unsupported type {type.Name}.");
        }

#if DEBUG
        Assert(_src[_i] == '@');
#endif

        _i++;

        if (_fieldsToResolve.TryGetValue(hash, out var listToResolve))
        {
            for (int i = 0; i < listToResolve.Count; i++)
            {
                listToResolve[i].Set(value);
            }

            // should we remove the items from the hashmap? no
            //_fieldsToResolve.Remove(hash);
        }

        _deserializedObjects.Add(hash, value);
        
        return value;
    }

    int DeserializeObjectHeader(out Type type)
    {
        type = null;

        int hash = ObjectHash();
        RemoveDelimiters();

        type = _indexToType[GetNextInt()];
        RemoveDelimiters();

        return hash;
    }
    
    object DeserializeClass(Type type)
    {
        object obj = FormatterServices.GetUninitializedObject(type);
        var fields = GetObjectFields(type);
        RemoveDelimiters();

        while (_src.Length > _i &&
               _src[_i] != '@')
        {
            var fieldName = GetNextFieldName();

            _i++;

            if (fields.TryGetValue(fieldName, out var field))
            {
                DeserializeObjectField(obj, field);
            }
            else
            {
                throw new Exception($"Object missing field {fieldName}.");
            }

            RemoveDelimiters();
        }

        return obj;
    }

    object DeserializeArray(Type type)
    {
        if (type.IsSZArray)
        {
            RemoveDelimiters();
            int arrayLength = GetNextInt();

            Type typeOfElement = type.GetElementType();
            Array arr = Array.CreateInstance(typeOfElement, arrayLength);

            for (int i = 0; i < arrayLength; i++)
            {
                RemoveDelimiters();
                DeserializeArrayCell(arr, i, typeOfElement);
            }

            RemoveDelimiters();

            return arr;
        }
        else // multi size array..
        {
            throw new Exception("Multi dimensional arrays not supported right now.");
        }
    }

    void DeserializeObjectField<T>(T obj, FieldInfo field)
    {
        var fieldType = field.FieldType;
        RemoveDelimiters();

        if (fieldType.IsValueType)
        {
            var val = DeserializeValueType(fieldType);
            field.SetValue(obj, val);
        }
        else
        {
            int? hash = DeserializeRefLink(fieldType, out var val);

            if (val != null)
            {
                field.SetValue(obj, val);
            }
            else if (hash.HasValue)
            {
                AddMarkToResolveAdnAddSetter(hash.Value, fieldType, new FieldSetter(obj, field));
            }
        }
    }

    void DeserializeArrayCell(Array array, int index, Type elementType)
    {
        if (elementType.IsValueType)
        {
            var val = DeserializeValueType(elementType);
            array.SetValue(val, index);
        }
        else
        {
            int? hash = DeserializeRefLink(elementType, out var val);

            if (val != null)
            {
                array.SetValue(val, index);
            }
            else if (hash.HasValue)
            {
                AddMarkToResolveAdnAddSetter(hash.Value, elementType, new ArraySetter(array, index));
            }
        }
    }

    void AddMarkToResolveAdnAddSetter(int hash, Type type, ISetter setter)
    {
        //_hashToType.TryAdd(hash, type);
        if (_fieldsToResolve.TryGetValue(hash, out var list) == false)
        {
            list = new List<ISetter>();
            _fieldsToResolve.Add(hash, list);
        }

        list.Add(setter);
    }

    object DeserializeValueType(Type type)
    {
        RemoveDelimiters();

        if (type == typeof(int))
        {
            int intVal = GetNextInt();
            return intVal;
        }
        else if (type == typeof(float))
        {
            float? floatVal = GetNextFloat();
            return floatVal;
        }
        else if (type.IsStruct())
        {
            object obj = FormatterServices.GetUninitializedObject(type);
            var fields = GetObjectFields(type);

            RemoveDelimiters();
            // remove {
            _i++;

            while (_src.Length > _i && _src[_i] != '}')
            {
                RemoveDelimiters();

                string fieldName = GetNextFieldName();

                if (fields.TryGetValue(fieldName, out FieldInfo field))
                {
                    DeserializeObjectField(obj, field);
                }
                else
                {
                    throw new Exception($"Object missing field {fieldName}.");
                }

                RemoveDelimiters();
            }

            // remove }
            _i++;

            return obj;
        }

        return null;
    }

    int? DeserializeRefLink(Type type, out object val)
    {
        RemoveDelimiters();

        if (_src[_i] == '-')
        {
            _i++;
            val = null;
            return null;
        }

        int hash = ObjectHash();
        _deserializedObjects.TryGetValue(hash, out val);

        return hash;
    }

    int ObjectHash()
    {
        // remove @
#if DEBUG
        Assert(_src[_i] == '@', GetCurrentLine());
#endif

        _i++;

        return GetNextInt();
    }

    int GetNextInt()
    {
        int start = _i;

        RemoveWhileNoDelimiter();

        var s = _src.AsSpan(start, _i - start);

        return int.Parse(s);
    }

    float GetNextFloat()
    {
        int start = _i;

        RemoveWhileNoDelimiter();

        var s = _src.AsSpan(start, _i - start);

        return float.Parse(s);
    }

    string GetNextFieldName()
    {
        RemoveWhiteSpace();

        int start = _i;

        while (_src.Length > _i && _src[_i] != ':')
        {
            _i++;
        }

        return _src.Substring(start, _i - start);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    bool IsDelimiter(char c)
    {
        return char.IsWhiteSpace(_src[_i]) || _src[_i] == ':' || _src[_i] == '"' || _src[_i] == ',';
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void RemoveWhileNoDelimiter()
    {
        while (_src.Length > _i && IsDelimiter(_src[_i]) == false)
        {
            _i++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void RemoveNotWhiteSpace()
    {
        while (_src.Length > _i && char.IsWhiteSpace(_src[_i]) == false)
        {
            _i++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void RemoveDelimiters()
    {
        while (_src.Length > _i && IsDelimiter(_src[_i]))
        {
            _i++;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void RemoveWhiteSpace()
    {
        while (_src.Length > _i && char.IsWhiteSpace(_src[_i]))
        {
            _i++;
        }
    }

    string GetCurrentLine()
    {
        int left = _i;

        while (left > 0)
        {
            left--;

            if (_src[left] == '\n')
            {
                left++;
                break;
            }
        }

        int right = _i;

        while (right < _src.Length && _src[right] == '\n')
        {
            left++;
        }

        return _src.Substring(left, right - left);
    }
}