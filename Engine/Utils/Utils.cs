using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections;
using System.Numerics;
using ThirdParty.SDL;

namespace SBEngine;

#if DEBUG
using System.Diagnostics;
#endif

public unsafe static class Utils
{
    public static Dictionary<string, Type> types = new Dictionary<string, Type>();

    static Utils()
    {
        foreach (Assembly assebly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type type in assebly.GetTypes())
            {
                if (types.ContainsKey(type.FullName) == false)
                {
                    types.Add(type.FullName, type);
                }
            }
        }
    }
    
    public static void AssignUnsafe<T, T2>(ref T from, ref T2 to)
        where T : unmanaged
        where T2 : unmanaged
    {
        var ptr = (T*)Unsafe.AsPointer<T>(ref from);
        to = *(T2*)ptr;
    }
    
    public static bool Is<T>(this int num, T enm) where T : Enum
    {
        var numt = Unsafe.As<int, T>(ref num);
        return Enum.Equals(numt, enm);
    }

    public static void Set<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
    {
        if (dic.ContainsKey(key))
        {
            dic[key] = value;
        }
        else
        {
            dic.Add(key, value);
        }
    }

    public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key)
    {
        if (dic.ContainsKey(key))
        {
            return dic[key];
        }
        else
        {
            var val = default(TValue);
            dic.Add(key, val);
            return val;
        }
    }

    public static bool Equals(this string str, byte* bytes)
    {
        for (int i = 0; i < str.Length; i++)
        {
            if ((byte)str[i] != bytes[i]) return false;
        }

        return false;
    }

    public static string GetExecutablePath()
    {
        string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        return Path.GetDirectoryName(strExeFilePath);
    }

    public static bool IsNumber(this string str)
    {
        if (string.IsNullOrEmpty(str)) return false;

        for (int i = 0; i < str.Length; i++)
        {
            if (char.IsNumber(str[i]) == false) return false;
        }

        return true;
    }

    public static bool Contains<T>(this IEnumerable<T> list, Func<T, bool> compare)
    {
        foreach (var item in list)
        {
            if (compare(item)) return true;
        }

        return false;
    }

    public static int Remove<T>(this List<T> list, Func<T, bool> compare)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (compare(list[i]))
            {
                list.RemoveAt(i);
                return i;
            }
        }

        return -1;
    }

    public static T[] Concat<T>(this T[] arr, T[] arr2)
    {
        var new_arr = new T[arr.Length + arr2.Length];
        arr.CopyTo(new_arr, 0);
        arr2.CopyTo(new_arr, arr.Length);
        return new_arr;
    }

    public static void RunTimedTest(Action action, string test_name = "test")
    {
        var start = SDL.SDL_GetPerformanceCounter();

        action();

        var end = SDL.SDL_GetPerformanceCounter();
        Console.WriteLine($"[{test_name}] Time: {((float)(end - start) / (float)SDL.SDL_GetPerformanceFrequency()):N2}ms");
    }

    public static T GetPrevItem<T>(this List<T> list, int idx)
    {
        if (idx == 0) return list[list.Count - 1];
        return list[idx - 1];
    }

    public static T GetNextItem<T>(this List<T> list, int idx)
    {
        if (idx == list.Count - 1) return list[0];
        return list[idx + 1];
    }

    public static T GetItemLooping<T>(this List<T> list, int idx)
    {
        return list[idx % (list.Count)];
    }

    public static void ReadUnmanaged<T>(this Stream stream, T* ptr) where T : unmanaged
    {
        var span = new Span<byte>(ptr, sizeof(T));
        stream.Read(span);
    }

    public static void ReadToPointer(this Stream stream, void* ptr, int size)
    {
        var span = new Span<byte>(ptr, size);
        stream.Read(span);
    }

    public static void ReadToPointer<T>(this Stream stream, T* ptr, int numberOfElements) where T : unmanaged
    {
        var span = new Span<byte>(ptr, numberOfElements * sizeof(T));
        stream.Read(span);
    }

    public static void ReadUnmanaged(this Stream stream, void* ptr, int size)
    {
        var span = new Span<byte>(ptr, size);
        stream.Read(span);
    }

    public static string ReadString(this Stream stream, int length)
    {
        char* chars = stackalloc char[length];

        stream.ReadToPointer(chars, length);

        return new string(chars, 0, length);
    }

    public static void WriteUnmanaged<T>(this Stream stream, T* ptr) where T : unmanaged
    {
        var span = new Span<byte>(ptr, sizeof(T));
        stream.Write(span);
    }

    public static void WriteUnmanaged<T>(this Stream stream, T data) where T : unmanaged
    {
        var span = new Span<byte>(&data, sizeof(T));
        stream.Write(span);
    }

    public static void WriteString(this Stream stream, string str)
    {
        fixed (char* ptr = str)
        {
            stream.WriteFromPointer(ptr, str.Length);
        }
    }

    public static void WriteFromPointer(this Stream stream, void* ptr, int size)
    {
        var span = new Span<byte>(ptr, size);
        stream.Write(span);
    }

    public static void WriteFromPointer<T>(this Stream stream, T* ptr, int numberOfElements) where T : unmanaged
    {
        var span = new Span<byte>(ptr, numberOfElements * sizeof(T));
        stream.Write(span);
    }

    public static int IndexOf<T>(this List<T> list, Func<T, bool> func)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (func(list[i]))
            {
                return i;
            }
        }

        return -1;
    }

    public static bool IsStruct(this Type type)
    {
        return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
    }

    public static void DoError(string str)
    {
#if DEBUG
        Console.WriteLine(str);
        Debugger.Break();
#else
        File.AppendAllText("log.txt", "\n"+str);
#endif
    }

    public static Type GetTypeByFullName(string type_full_name)
    {
        return types[type_full_name];
    }

    public static int BitCount(this int number)
    {
        int count = 0;

        for (int i = 0; i < 32; i++)
        {
            if ((number & 1 << i) > 0)
            {
                count++;
            }
        }

        return count;
    }

    public static string TryReadFile(string path)
    {
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }

        return null;
    }

    public static Vector4 NextColor(this Random rnd)
    {
        return new Vector4(rnd.NextSingle(), rnd.NextSingle(), rnd.NextSingle(), 1f);
    }

    public static bool Flag<T>(this T arg1, T arg2) where T : Enum
    {
        int* ptr1 = (int*)Unsafe.AsPointer(ref arg1);
        int* ptr2 = (int*)Unsafe.AsPointer(ref arg2);

        return (*ptr1 & *ptr2) > 0;
    }

}

public unsafe interface IMemoryOps
{
    public void* Malloc(ulong size);
    public void Free(void* ptr);
    public void* Realloc(void* ptr, ulong new_size);
}

public unsafe class CSharpMemoryOps : IMemoryOps
{
    public void* Malloc(ulong size)
    {
        return Marshal.AllocHGlobal((int)size).ToPointer();
    }

    public void Free(void* ptr)
    {
        Marshal.FreeHGlobal(new IntPtr(ptr));
    }

    public void* Realloc(void* ptr, ulong new_size)
    {
        return Marshal.ReAllocHGlobal(new IntPtr(ptr), new IntPtr((int)new_size)).ToPointer();
    }
}
