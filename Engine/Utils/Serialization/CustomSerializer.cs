using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace SBEngine;

public class SerializedFieldAttribute : Attribute
{
}

public class CustomSerializer
{
    // Cache state
    private Dictionary<Type, ArrayList<FieldInfo>> _serializedFields;

    // State to clear after serialization
    private Queue<object> _objectsToSerialize;
    private Dictionary<int, bool> _serializedObjects;
    private StringBuilder _sb;
    private StringBuilder _sbType;
    private int _nextTypeIndex;

    public CustomSerializer()
    {
        _serializedFields = new Dictionary<Type, ArrayList<FieldInfo>>();
        _objectsToSerialize = new Queue<object>();
        _serializedObjects = new Dictionary<int, bool>();

        _sbType = new StringBuilder();
        _sb = new StringBuilder();

        _nextTypeIndex = -1;
    }

    public string Serialize(object obj)
    {
        var hash = obj.GetHashCode();
        QueueReferenceObject(hash, obj);

        while (_objectsToSerialize.TryDequeue(out var serializable))
        {
            SerializeReferenceObject(serializable);
            _sb.Append('\n');
        }

        string str = _sbType.ToString() + '\n' + _sb.ToString();

        // clear state
        _sb.Clear();
        _sbType.Clear();
        _objectsToSerialize.Clear();
        _serializedObjects.Clear();
        _nextTypeIndex = -1;

        return str;
    }

    public void SerializeReferenceObject(object obj)
    {
        var hash = obj.GetHashCode();
        var objectType = obj.GetType();

        _sb.Append('@');
        _sb.Append(hash);

        _sb.Append(' ');

        _sb.Append(RegisterType(objectType));

        _sb.Append('\n');

        if (objectType == typeof(string))
        {
            string? stringVal = obj as string;
            WriteString(stringVal);
            _sb.Append('\n');
        }
        else if (objectType.IsArray)
        {
            if (objectType.IsSZArray)
            {
                Array arr = obj as Array;
                Type typeOfElement = objectType.GetElementType();

                _sb.Append(arr.Length);
                _sb.Append(':');

                int end = arr.Length - 1;
                for (int i = 0; i <= end; i++)
                {
                    _sb.Append(' ');
                    SerializeObject(arr.GetValue(i), typeOfElement);
                }
                
                _sb.Append('\n');
            }
            else // multi size array..
            {
                throw new Exception("Multi dimensional arrays not supported right now.");
            }
        }
        else if (objectType.IsClass)
        {
            var serializedFields = GetSerializedFields(objectType);

            foreach (var field in serializedFields)
            {
                SerializeField(obj, field);
                _sb.Append('\n');
            }
        }
        else
        {
            throw new Exception($"Unsupported type: {objectType.FullName}");
        }

        _sb.Append('@');
        _sb.Append('\n');
    }

    int RegisterType(Type type)
    {
        _nextTypeIndex++;

        _sbType.Append(_nextTypeIndex);
        _sbType.Append(':');
        
        if (type.IsGenericType)
        {
            WriteGenericDefinition(type);
        }
        else
        {
            _sbType.Append(type.FullName);
        }
        
        _sbType.Append('\n');

        return _nextTypeIndex;
    }
    
    void WriteGenericDefinition(Type type)
    {
        var genericArgs = type.GetGenericArguments();
        
        _sbType.Append(type.Namespace);
        _sbType.Append('.');
        _sbType.Append(type.Name);
        
        _sbType.Append('<');

        int end = genericArgs.Length - 1;
        for (int i = 0; i < genericArgs.Length; i++)
        {
            
            if (genericArgs[i].IsGenericType)
            {
                WriteGenericDefinition(genericArgs[i]);
            }
            else
            {
                _sbType.Append(genericArgs[i].FullName);
            }

            if (i != end) _sbType.Append(',');
        }
        _sbType.Append('>');
    }
    
    ArrayList<FieldInfo> GetSerializedFields(Type type)
    {
        if (_serializedFields.TryGetValue(type, out var list) == false)
        {
            list = new ArrayList<FieldInfo>();
            _serializedFields.Add(type, list);

            var fields = type.GetRuntimeFields();

            foreach (var field in fields)
            {
                if (field.IsPublic)
                {
                    list.Add(field);
                }
                else
                {
                    var attr = field.GetCustomAttribute<SerializedFieldAttribute>();

                    if (attr != null)
                    {
                        list.Add(field);
                    }
                }
            }
        }

        return list;
    }

    void SerializeField(object obj, FieldInfo field)
    {
        _sb.Append(field.Name);
        _sb.Append(':');

        SerializeObject(field.GetValue(obj), field.FieldType);
    }

    void SerializeObject(object? value, Type type)
    {
        if (type.IsValueType == false)
        {
            if (value == null)
            {
                _sb.Append('-');
                return;
            }

            WriteReferenceObject(value);
            return;
        }
        else
        {
            if (type == typeof(int))
            {
                int? intVal = value as int?;
                WriteInt(intVal.Value);
                return;
            }

            if (type == typeof(float))
            {
                float? floatVal = value as float?;
                WriteFloat(floatVal.Value);
                return;
            }

            if (type.IsStruct())
            {
                WriteValueObject(value, type);
                return;
            }
        }

        throw new Exception($"Unsupported type: {type.Name}.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteReferenceObject(object? obj)
    {
        if (obj == null)
        {
            _sb.Append('-');
        }
        else
        {
            var hash = obj.GetHashCode();
            QueueReferenceObject(hash, obj);

            _sb.Append('@');
            _sb.Append(hash);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void QueueReferenceObject(int hash, object obj)
    {
        if (_serializedObjects.ContainsKey(hash) == false)
        {
            _objectsToSerialize.Enqueue(obj);
            _serializedObjects.Add(hash, true);
        }
    }

    void WriteValueObject(object obj, Type type)
    {
        _sb.Append('{');

        var fields = GetSerializedFields(type);

        foreach (var field in fields)
        {
            _sb.Append(' ');
            SerializeField(obj, field);
        }

        _sb.Append(' ');
        _sb.Append('}');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteString(string? value)
    {
        if (value == null)
        {
            _sb.Append('-');
        }
        else
        {
            _sb.Append(value.Length);
            _sb.Append(':');
            _sb.Append('"');
            _sb.Append(value);
            _sb.Append('"');
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteInt(int value)
    {
        _sb.Append(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void WriteFloat(float value)
    {
        _sb.Append(value);
    }
}