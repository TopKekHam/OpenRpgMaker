﻿using System.Runtime.InteropServices;
using ThirdParty.SDL;
using static ThirdParty.SDL.SDL;
using System.Text;

namespace ThirdParty.OpenGL;

public enum OpenGLBuffering
{
    SINGLE,
    DOUBLE
}

public struct OpenGLConfig
{
    public int majorVersion;
    public int minorVersion;
    public OpenGLBuffering frameBuffering;
    public int depthBufferSize;
}

public struct Uniform
{
    public string name;
    public GL type;
    public int array_size;
    public int location;
}

public struct GLShaderAttribute
{
    public string name;
    public GL type;
    public int array_size;
    public int location;
}

public unsafe class OpenGL
{
    public delegate* unmanaged[Cdecl]<void*, void*, void> glDebugMessageCallback;
    public delegate* unmanaged[Cdecl]<float, float, float, float, void> glClearColor;
    public delegate* unmanaged[Cdecl]<GL, void> glClear;
    public delegate* unmanaged[Cdecl]<float, void> glClearDepth;
    public delegate* unmanaged[Cdecl]<uint> glCreateProgram;
    public delegate* unmanaged[Cdecl]<uint, uint, void> glAttachShader;
    public delegate* unmanaged[Cdecl]<uint, void> glCompileShader;
    public delegate* unmanaged[Cdecl]<GL, uint> glCreateShader;
    public delegate* unmanaged[Cdecl]<uint, void> glDeleteShader;
    public delegate* unmanaged[Cdecl]<uint, int, byte**, int*, void> glShaderSource;
    public delegate* unmanaged[Cdecl]<uint, void> glLinkProgram;
    public delegate* unmanaged[Cdecl]<uint, int, int*, byte*, void> glGetShaderInfoLog;
    public delegate* unmanaged[Cdecl]<uint, int, int*, byte*, void> glGetProgramInfoLog;
    public delegate* unmanaged[Cdecl]<uint, GL, int*, void> glGetShaderiv;
    public delegate* unmanaged[Cdecl]<uint, GL, int*, void> glGetProgramiv;
    public delegate* unmanaged[Cdecl]<int, uint*, void> glGenBuffers;
    public delegate* unmanaged[Cdecl]<GL, uint, void*, GL, void> glBufferData;
    public delegate* unmanaged[Cdecl]<uint, uint, void*, GL, void> glNamedBufferData;
    public delegate* unmanaged[Cdecl]<GL, uint, void> glBindBuffer;
    public delegate* unmanaged[Cdecl]<uint, void> glUseProgram;
    public delegate* unmanaged[Cdecl]<uint, void> glEnableVertexAttribArray;
    public delegate* unmanaged[Cdecl]<uint, void> glDisableVertexAttribArray;
    public delegate* unmanaged[Cdecl]<uint, int, GL, GL, int, void*, void> glVertexAttribPointer;
    public delegate* unmanaged[Cdecl]<uint, uint, void> glVertexAttribDivisor;
    public delegate* unmanaged[Cdecl]<GL, uint, GL, void*, void> glDrawElements;
    public delegate* unmanaged[Cdecl]<int, int, GL, float*, void> glUniformMatrix4fv;
    public delegate* unmanaged[Cdecl]<int, int, float*, void> glUniform4fv;
    public delegate* unmanaged[Cdecl]<int, int, float*, void> glUniform3fv;
    public delegate* unmanaged[Cdecl]<int, int, float*, void> glUniform2fv;
    public delegate* unmanaged[Cdecl]<int, float, void> glUniform1f;
    public delegate* unmanaged[Cdecl]<int, int, void> glUniform1i;
    public delegate* unmanaged[Cdecl]<uint, GL, void*> glMapNamedBuffer;

    public delegate* unmanaged[Cdecl]<uint, bool> glUnmapNamedBuffer;

    // location, count, value
    public delegate* unmanaged[Cdecl]<int, uint, uint*, void> glUniform1iv;
    public delegate* unmanaged[Cdecl]<uint, byte*, int> glGetUniformLocation;
    public delegate* unmanaged[Cdecl]<GL, void> glEnable;
    public delegate* unmanaged[Cdecl]<GL, void> glDisable;
    public delegate* unmanaged[Cdecl]<GL, GL, void> glHint;
    public delegate* unmanaged[Cdecl]<GL, GL, uint, void> glTexBuffer;
    public delegate* unmanaged[Cdecl]<GL, uint, void> glBindTexture;
    public delegate* unmanaged[Cdecl]<uint, uint*, void> glGenTextures;
    public delegate* unmanaged[Cdecl]<GL, int, GL, uint, uint, int, GL, GL, void*, void> glTexImage2D;
    public delegate* unmanaged[Cdecl]<GL, uint, GL, uint, uint, bool, void> glTexImage2DMultisample;
    public delegate* unmanaged[Cdecl]<GL, GL, GL, uint, int, void> glFramebufferTexture2D;
    public delegate* unmanaged[Cdecl]<GL, uint, void> glBindFramebuffer;
    public delegate* unmanaged[Cdecl]<uint, uint*, void> glGenFramebuffers;
    public delegate* unmanaged[Cdecl]<GL, GL, GL, void> glTexParameteri;
    public delegate* unmanaged[Cdecl]<GL, int, GL, int*, void> glGetTexLevelParameteriv;
    public delegate* unmanaged[Cdecl]<uint, uint, byte*, void> glBindAttribLocation;
    public delegate* unmanaged[Cdecl]<uint, byte*, int> glGetAttribLocation;
    public delegate* unmanaged[Cdecl]<GL, void> glActiveTexture;
    public delegate* unmanaged[Cdecl]<GL, uint, GL, void*, uint, void> glDrawElementsInstanced;
    public delegate* unmanaged[Cdecl]<uint, uint*, void> glGenVertexArrays;
    public delegate* unmanaged[Cdecl]<uint, void> glBindVertexArray;
    public delegate* unmanaged[Cdecl]<GL, uint, uint, uint, void> glDrawArraysInstanced;
    public delegate* unmanaged[Cdecl]<GL, int, uint, void> glDrawArrays;
    public delegate* unmanaged[Cdecl]<uint, uint*, void> glDeleteBuffers;
    public delegate* unmanaged[Cdecl]<GL> glGetError;
    public delegate* unmanaged[Cdecl]<GL, void> glDepthMask;
    public delegate* unmanaged[Cdecl]<GL, void> glDepthFunc;
    public delegate* unmanaged[Cdecl]<uint, uint*, void> glGenRenderbuffers;
    public delegate* unmanaged[Cdecl]<GL, uint, void> glBindRenderbuffer;
    public delegate* unmanaged[Cdecl]<GL, GL, uint, uint, void> glRenderbufferStorage;
    public delegate* unmanaged[Cdecl]<GL, GL, GL, uint, void> glFramebufferRenderbuffer;
    public delegate* unmanaged[Cdecl]<GL, uint, uint, byte*, void> glObjectLabel;
    public delegate* unmanaged[Cdecl]<GL, uint, GL, uint, uint, void> glRenderbufferStorageMultisample;
    public delegate* unmanaged[Cdecl]<uint, uint, uint, uint, uint, uint, uint, uint, GL, GL, void> glBlitFramebuffer;
    public delegate* unmanaged[Cdecl]<GL, void> glGenerateMipmap;
    public delegate* unmanaged[Cdecl]<int, int, uint, uint, void> glScissor;
    public delegate* unmanaged[Cdecl]<GL, int*, void> glGetIntegerv;

    //GLenum mode, GLenum type, const void* indirect, GLsizei drawcount, GLsizei stride
    public delegate* unmanaged[Cdecl]<GL, GL, void*, uint, uint, void> glMultiDrawElementsIndirect;

    //GLenum mode, const GLsizei* count, GLenum type, const GLvoid* const * indices, GLsizei drawcount, const GLint* basevertex
    public delegate* unmanaged[Cdecl]<GL, uint*, GL, void*, uint, int*, void> glMultiDrawElementsBaseVertex;

    //GLenum mode, const GLint* first, const GLsizei* count, GLsizei drawcount
    public delegate* unmanaged[Cdecl]<GL, int*, uint*, uint, void> glMultiDrawArrays;

    //GLenum mode, const GLsizei* count, GLenum type, const void* const * indices, GLsizei drawcount);
    public delegate* unmanaged[Cdecl]<GL, uint*, GL, void**, uint, void> glMultiDrawElements;

    //GLuint buffer, GLintptr offset, GLsizeiptr size, const void* data);
    public delegate* unmanaged[Cdecl]<uint, ulong, ulong, void*, void> glNamedBufferSubData;

    public delegate* unmanaged[Cdecl]<uint, uint*, void> glDeleteTextures;

    // program, index, bufSize, length, size, type, name
    public delegate* unmanaged[Cdecl]<uint, uint, uint, uint*, int*, GL*, byte*, void> glGetActiveAttrib;
    public delegate* unmanaged[Cdecl]<uint, uint, uint, uint*, int*, GL*, byte*, void> glGetActiveUniform;

    // x, y, width, height
    public delegate* unmanaged[Cdecl]<int, int, uint, uint, void> glViewport;

    //target, value, data
    public delegate* unmanaged[Cdecl]<GL, GL, int*, void> glGetBufferParameteriv;
    public delegate* unmanaged[Cdecl]<void> glFinish;
    public delegate* unmanaged[Cdecl]<uint, uint*, void> glDeleteFramebuffers;

    public delegate* unmanaged[Cdecl]<GL, GL, void> glBlendFunc;
    public delegate* unmanaged[Cdecl]<uint, GL, GL, void> glBlendFunci;
    public delegate* unmanaged[Cdecl]<GL, GL, GL, GL, void> glBlendFuncSeparate;
    public delegate* unmanaged[Cdecl]<uint, GL, GL, GL, GL, void> glBlendFuncSeparatei;
    public delegate* unmanaged[Cdecl]<GL, void> glBlendEquation;
    public delegate* unmanaged[Cdecl]<uint, GL, void> glBlendEquationi;
    public delegate* unmanaged[Cdecl]<GL, GL, void> glBlendEquationSeparate;
    public delegate* unmanaged[Cdecl]<uint, GL, GL, void> glBlendEquationSeparatei;

    public delegate* unmanaged[Cdecl]<GL, GL, void> glPolygonMode;
    public delegate* unmanaged[Cdecl]<float, void> glPointSize;
    public delegate* unmanaged[Cdecl]<float, void> glLineWidth;
    public delegate* unmanaged[Cdecl]<uint, uint, GL, uint, uint, void> glTextureStorage2D;
    public delegate* unmanaged[Cdecl]<uint, GL, uint, int, void> glNamedFramebufferTexture;
    public delegate* unmanaged[Cdecl]<GL, uint, GL, uint, uint, void> glTexStorage2D;
    public delegate* unmanaged[Cdecl]<GL, GL> glCheckFramebufferStatus;
    public delegate* unmanaged[Cdecl]<uint, GL*, void> glDrawBuffers;

    public delegate* unmanaged[Cdecl]<uint, uint*, void> glCreateBuffers;
    public delegate* unmanaged[Cdecl]<uint, uint*, void> glCreateVertexArrays;
    public delegate* unmanaged[Cdecl]<uint, uint*, void> glCreateFramebuffers;
    public delegate* unmanaged[Cdecl]<GL, void*, void> glDrawArraysIndirect;
    
    public IntPtr GlContext;
    DebugCallback _debugCallback;

    public OpenGL(IntPtr window, OpenGLConfig config, DebugCallback debugCallback = null)
    {
        int GL_CONTEXT_PROFILE_CORE = 0x0001;
        SDL_GL_SetAttribute(SDL_GL_Attr.CONTEXT_PROFILE_MASK, GL_CONTEXT_PROFILE_CORE);

#if DEBUG
        int SDL_GL_CONTEXT_DEBUG_FLAG = 0x0001;
        SDL_GL_SetAttribute(SDL_GL_Attr.CONTEXT_FLAGS, SDL_GL_CONTEXT_DEBUG_FLAG);
#endif

        SDL_GL_SetAttribute(SDL_GL_Attr.CONTEXT_MAJOR_VERSION, config.majorVersion);
        SDL_GL_SetAttribute(SDL_GL_Attr.CONTEXT_MINOR_VERSION, config.minorVersion);

        if (config.frameBuffering == OpenGLBuffering.DOUBLE)
        {
            SDL_GL_SetAttribute(SDL_GL_Attr.DOUBLEBUFFER, 1);
        }
        else
        {
            SDL_GL_SetAttribute(SDL_GL_Attr.DOUBLEBUFFER, 0);
        }

        SDL_GL_SetAttribute(SDL_GL_Attr.DEPTH_SIZE, config.depthBufferSize);

        GlContext = SDL_GL_CreateContext(window);

        var type = typeof(OpenGL);

        var fields = type.GetFields();

        foreach (var field in fields)
        {
            if (field.IsStatic == false)
            {
                void* ptr = SDL_GL_GetProcAddress(field.Name);

                if (ptr != null)
                {
                    field.SetValue(this, new IntPtr(ptr));
                }
            }
        }

        _debugCallback = debugCallback;

        if (_debugCallback == null)
        {
            _debugCallback = DebugCallback;
        }

        void* procPtr = Marshal.GetFunctionPointerForDelegate(this._debugCallback).ToPointer();
        glDebugMessageCallback(procPtr, null);

        SDL_GL_SetSwapInterval(SwapInterval.NONE);

#if DEBUG
        glEnable(GL.DEBUG_OUTPUT_SYNCHRONOUS);
#endif
    }

    public uint glGenVertexArray(string name = null)
    {
        uint vao;
        glGenVertexArrays(1, &vao);

        if (name == null)
        {
            name = $"buffer {vao}";
        }

        hfNameObject(GL.VERTEX_ARRAY, vao, name);

        return vao;
    }

    public uint glGenFramebuffer(string name = null)
    {
        uint framebuffer;
        glGenFramebuffers(1, &framebuffer);

        if (name == null)
        {
            name = $"framebuffer {framebuffer}";
        }

        hfNameObject(GL.FRAMEBUFFER, framebuffer, name);

        return framebuffer;
    }

    public uint glGenRenderbuffer(string name = null)
    {
        uint renderbuffer;
        glGenRenderbuffers(1, &renderbuffer);

        if (name == null)
        {
            name = $"renderbuffer {name}";
        }

        hfNameObject(GL.RENDERBUFFER, renderbuffer, name);

        return renderbuffer;
    }

    public uint glGenBuffer(string name = null)
    {
        uint buffer;
        glGenBuffers(1, &buffer);

        if (name == null)
        {
            name = $"buffer {buffer}";
        }

        hfNameObject(GL.BUFFER, buffer, name);

        return buffer;
    }

    public void glDeleteFramebuffer(uint framebuffer)
    {
        glDeleteFramebuffers(1, &framebuffer);
    }

    public void hfNameObject(GL identifier, uint id, string name)
    {
        var name_bytes = Encoding.ASCII.GetBytes(name + '\0');

        fixed (byte* name_ptr = name_bytes)
        {
            glObjectLabel(identifier, id, (uint)name_bytes.Length, name_ptr);
        }
    }

    public int hfGetMaxTextureImageUnits()
    {
        int texture_units;
        glGetIntegerv(GL.MAX_TEXTURE_IMAGE_UNITS, &texture_units);
        return texture_units;
    }

    //public void hfCheckErrors(string place)
    //{
    //    GL err = err = glGetError();

    //    while (err != GL.NO_ERROR)
    //    {
    //        Console.WriteLine($"[{place}] {err}");
    //        err = glGetError();
    //    }
    //}

    //public void hfClearErrors()
    //{
    //    while (glGetError() != GL.NO_ERROR) { }
    //}

    //public void hfPrintLastError(string place)
    //{
    //    GL err = err = glGetError();
    //    GL last_error = GL.NO_ERROR;

    //    while (err != GL.NO_ERROR)
    //    {
    //        last_error = err;
    //        err = glGetError();
    //    }

    //    if (last_error != GL.NO_ERROR)
    //    {
    //        Console.WriteLine($"[{place}] {err}");
    //    }
    //}

    const int NAME_BUFFER_SIZE = 128;

    public Uniform hfGetUniformInfo(uint program, uint uniform)
    {
        byte* ptr_name = stackalloc byte[NAME_BUFFER_SIZE];
        uint length;
        int type_size;
        GL type;
        glGetActiveUniform(program, uniform, NAME_BUFFER_SIZE, &length, &type_size, &type, ptr_name);

        return new Uniform()
        {
            name = CStrToString(ptr_name, (int)length),
            array_size = type_size,
            type = type
        };
    }

    public GLShaderAttribute hfGetAttributeInfo(uint program, uint attribute)
    {
        byte* ptr_name = stackalloc byte[NAME_BUFFER_SIZE];
        uint length;
        int type_size;
        GL type;
        glGetActiveAttrib(program, attribute, NAME_BUFFER_SIZE, &length, &type_size, &type, ptr_name);

        return new GLShaderAttribute()
        {
            name = CStrToString(ptr_name, (int)length),
            array_size = type_size,
            type = type
        };
    }

    void DebugCallback(GL source, GL type, uint id, GL severity, int length, byte* message_ptr, void* userParam)
    {
        if (severity == GL.DEBUG_SEVERITY_NOTIFICATION) return;

        if (severity == GL.DEBUG_SEVERITY_NOTIFICATION)
        {
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (severity == GL.DEBUG_SEVERITY_LOW)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        else if (severity == GL.DEBUG_SEVERITY_MEDIUM)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
        }
        else if (severity == GL.DEBUG_SEVERITY_HIGH)
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        var message = Marshal.PtrToStringAnsi(new IntPtr(message_ptr), length);

        var typeStr = type.ToString().Substring(11);

        Console.WriteLine($"[{source} | {typeStr} : {id}] {message}");

        Console.ForegroundColor = ConsoleColor.White;
    }

    public bool hfCreateShader(string source, GL type, ref uint rshader, string debug_name = null)
    {
        var shader = glCreateShader(type);

        if (debug_name == null)
        {
            debug_name = $"Shader {shader} [" + type.ToString() + "]";
        }
        else
        {
            debug_name += " [" + type.ToString() + "]";
        }

        if (hfCompileShader(source, shader, debug_name))
        {
            rshader = shader;
            hfNameObject(GL.SHADER, shader, debug_name);
            return true;
        }

        return false;
    }

    public bool hfCompileShader(string source, uint shader, string debug_name = "unknow shader")
    {
        int length = source.Length;

        byte[] bytes = source.ToCStr();

        fixed (byte* ptr = bytes)
        {
            glShaderSource(shader, 1, &ptr, null);
        }

        glCompileShader(shader);

        int compiled;
        glGetShaderiv(shader, GL.COMPILE_STATUS, &compiled);

        if (compiled <= 0)
        {
            byte* chars = stackalloc byte[1024];
            int true_length = 0;
            glGetShaderInfoLog(shader, 1024, &true_length, chars);
            Console.WriteLine($"------------- ERROR : {debug_name}-------------");
            Console.WriteLine(CStrToString(chars, true_length));
            glDeleteShader(shader);

            return false;
        }


        return true;
    }

    public bool hfLinkProgram(uint program, string debug_name = "unknow program")
    {
        glLinkProgram(program);

        int linked;
        glGetProgramiv(program, GL.LINK_STATUS, &linked);

        if (linked <= 0)
        {
            byte* chars = stackalloc byte[1024];
            int true_length = 0;
            glGetProgramInfoLog(program, 1024, &true_length, chars);
            Console.WriteLine($"------------- ERROR : {debug_name}-------------");
            Console.WriteLine(CStrToString(chars, true_length));

            return false;
        }

        return true;
    }

    public int hfGetUniformLocation(uint program, string uniform_name)
    {
        fixed (byte* ptr = uniform_name.ToCStr())
        {
            return glGetUniformLocation(program, ptr);
        }
    }

    public void hfBindAttribLocation(uint program, uint index, string variable_name)
    {
        fixed (byte* ptr = variable_name.ToCStr())
        {
            glBindAttribLocation(program, index, ptr);
        }
    }

    public int hfGetAttribLocation(uint program, string variable_name)
    {
        fixed (byte* ptr = variable_name.ToCStr())
        {
            return glGetAttribLocation(program, ptr);
        }
    }

    public void hfDrawBuffers(params GL[] attachments)
    {
        fixed (GL* ptr = attachments)
        {
            glDrawBuffers((uint)attachments.Length, ptr);
        }
    }

//    public void hfPrintError(string msg)
//    {
//        var error = glGetError();

    //        if (error != GL.NO_ERROR)
    //        {
    //#if DEBUG
    //            Console.WriteLine($"{msg} {error} ({(uint)error})");
    //            //Debugger.Launch();
    //            //Debugger.Break();
    //#endif
    //        }
    //    }

    public void glDebugPrintBufferContent<T>(GL target, uint buffer) where T : unmanaged
    {
        glBindBuffer(target, buffer);
        int size = 0;
        glGetBufferParameteriv(target, GL.BUFFER_SIZE, &size);

        T* ptr = (T*)glMapNamedBuffer(buffer, GL.READ_ONLY);

        StringBuilder builder = new();

        builder.Append($"size: {size} bytes, content : [");

        for (int i = 0; i < size / sizeof(T); i++)
        {
            builder.Append(ptr[i]);
            builder.Append(", ");
        }

        builder.Append(']');

        Console.WriteLine(builder.ToString());
        glUnmapNamedBuffer(buffer);
    }

    public void hfSetDebugOutSynchronous(bool value)
    {
        if (value)
        {
            glEnable(GL.DEBUG_OUTPUT_SYNCHRONOUS);
        }
        else
        {
            glEnable(GL.DEBUG_OUTPUT);
        }
    }
}

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public unsafe delegate void DebugCallback(GL source, GL type, uint id, GL severity, int length, byte* message,
    void* userParam);

[StructLayout(LayoutKind.Sequential)]
public struct DrawElementsIndirectCommand
{
    public uint count;
    public uint instanceCount;
    public uint firstIndex;
    public uint baseVertex;
    public uint baseInstance;
}

public enum GLTexParameter : uint
{
    GL_TEXTURE_MAG_FILTER = 0x2800,
    GL_TEXTURE_MIN_FILTER = 0x2801,
    GL_TEXTURE_WRAP_S = 0x2802,
    GL_TEXTURE_WRAP_T = 0x2803,
}

public enum GLBindTextureTarget : uint
{
    GL_TEXTURE_1D = 0x0DE0,
    GL_TEXTURE_2D = 0x0DE1,
    GL_TEXTURE_3D = 0x806F,
    GL_TEXTURE_1D_ARRAY = 0x8C18,
    GL_TEXTURE_2D_ARRAY = 0x8C1A,
    GL_TEXTURE_RECTANGLE = 0x84F5,
    GL_TEXTURE_CUBE_MAP = 0x8513,
    GL_TEXTURE_CUBE_MAP_ARRAY = 0x9009,
    GL_TEXTURE_BUFFER = 0x8C2A,
    GL_TEXTURE_2D_MULTISAMPLE = 0x9100,
    GL_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9102,
}

// not all values are here if you want you can add more, check out https://docs.gl/gl4/glTexImage2D
public enum GLTextImage2DTarget : uint
{
    GL_TEXTURE_2D = 0x0DE1,
    GL_TEXTURE_1D_ARRAY = 0x8C18,
    GL_TEXTURE_RECTANGLE = 0x84F5,
}

// not all values are here if you want you can add more, check out https://docs.gl/gl4/glTexImage2D
public enum GLInternalFormatTextImage2D : uint
{
    GL_RGBA = 0x1908,
    GL_RGBA8 = 0x8058,
}

// not all values are here if you want you can add more, check out https://docs.gl/gl4/glTexImage2D
public enum GLFormatTextImage2D : uint
{
    GL_RGBA = 0x1908,
    GL_BGRA = 0x80E1,
}

// not all values are here if you want you can add more, check out https://docs.gl/gl4/glTexImage2D
public enum GLTypeTextImage2D : uint
{
    GL_BYTE = 0x1400,
    GL_UNSIGNED_BYTE = 0x1401,
    GL_UNSIGNED_INT_8_8_8_8 = 0x8035,
}

public enum GLInternalFormat : uint
{
    GL_R8 = 0x8229,
    GL_R16 = 0x822A,
    GL_RG8 = 0x822B,
    GL_RG16 = 0x822C,
    GL_R16F = 0x822D,
    GL_R32F = 0x822E,
    GL_RG16F = 0x822F,
    GL_RG32F = 0x8230,
    GL_R8I = 0x8231,
    GL_R8UI = 0x8232,
    GL_R16I = 0x8233,
    GL_R16UI = 0x8234,
    GL_R32I = 0x8235,
    GL_R32UI = 0x8236,
    GL_RG8I = 0x8237,
    GL_RG8UI = 0x8238,
    GL_RG16I = 0x8239,
    GL_RG16UI = 0x823A,
    GL_RG32I = 0x823B,
    GL_RG32UI = 0x823C,
    GL_RGB32F = 0x8815,
    GL_RGBA32F = 0x8814,
    GL_RGBA16F = 0x881A,
    GL_RGB16F = 0x881B,
    GL_RGBA32UI = 0x8D70,
    GL_RGB32UI = 0x8D71,
    GL_RGBA16UI = 0x8D76,
    GL_RGB16UI = 0x8D77,
    GL_RGBA8UI = 0x8D7C,
    GL_RGB8UI = 0x8D7D,
    GL_RGBA32I = 0x8D82,
    GL_RGB32I = 0x8D83,
    GL_RGBA16I = 0x8D88,
    GL_RGB16I = 0x8D89,
    GL_RGBA8I = 0x8D8E,
    GL_RGB8I = 0x8D8F,
}

public enum GLEnableOptions : uint
{
    GL_BLEND = 0x0BE2,
    GL_CLIP_DISTANCE0 = 0x3000,
    GL_CLIP_DISTANCE1 = 0x3001,
    GL_CLIP_DISTANCE2 = 0x3002,
    GL_CLIP_DISTANCE3 = 0x3003,
    GL_CLIP_DISTANCE4 = 0x3004,
    GL_CLIP_DISTANCE5 = 0x3005,
    GL_CLIP_DISTANCE6 = 0x3006,
    GL_CLIP_DISTANCE7 = 0x3007,
    GL_COLOR_LOGIC_OP = 0x0BF2,
    GL_CULL_FACE = 0x0B44,
    GL_DEBUG_OUTPUT = 0x92E0,
    GL_DEBUG_OUTPUT_SYNCHRONOUS = 0x8242,
    GL_DEPTH_CLAMP = 0x864F,
    GL_DEPTH_TEST = 0x0B71,
    GL_DITHER = 0x0BD0,
    GL_FRAMEBUFFER_SRGB = 0x8DB9,
    GL_LINE_SMOOTH = 0x0B20,
    GL_MULTISAMPLE = 0x809D,
    GL_POLYGON_OFFSET_POINT = 0x2A01,
    GL_POLYGON_OFFSET_LINE = 0x2A02,
    GL_POLYGON_OFFSET_FILL = 0x8037,
    GL_POLYGON_SMOOTH = 0x0B41,
    GL_PRIMITIVE_RESTART = 0x8F9D,
    GL_PRIMITIVE_RESTART_FIXED_INDEX = 0x8D69,
    GL_RASTERIZER_DISCARD = 0x8C89,
    GL_SAMPLE_ALPHA_TO_COVERAGE = 0x809E,
    GL_SAMPLE_ALPHA_TO_ONE = 0x809F,
    GL_SAMPLE_COVERAGE = 0x80A0,
    GL_SAMPLE_SHADING = 0x8C36,
    GL_SAMPLE_MASK = 0x8E51,
    GL_SCISSOR_TEST = 0x0C11,
    GL_STENCIL_TEST = 0x0B90,
    GL_TEXTURE_CUBE_MAP_SEAMLESS = 0x884F,
    GL_PROGRAM_POINT_SIZE = 0x8642,
}

public enum GLDrawMode : uint
{
    GL_POINTS = 0x0000,
    GL_LINES = 0x0001,
    GL_LINE_LOOP = 0x0002,
    GL_LINE_STRIP = 0x0003,
    GL_LINES_ADJACENCY = 0x000A,
    GL_LINE_STRIP_ADJACENCY = 0x000B,
    GL_TRIANGLES = 0x0004,
    GL_TRIANGLE_STRIP = 0x0005,
    GL_TRIANGLE_FAN = 0x0006,
    GL_TRIANGLES_ADJACENCY = 0x000C,
    GL_TRIANGLE_STRIP_ADJACENCY = 0x000D,
    GL_PATCHES = 0x000E,
}

public enum GLBoolean : byte
{
    FALSE = 0,
    TRUE = 1
}

public enum GLType : uint
{
    GL_BYTE = 0x1400,
    GL_UNSIGNED_BYTE = 0x1401,
    GL_SHORT = 0x1402,
    GL_UNSIGNED_SHORT = 0x1403,
    GL_INT = 0x1404,
    GL_UNSIGNED_INT = 0x1405,
    GL_FLOAT = 0x1406,
}

public enum GLClearFlags : uint
{
    DEPTH_BUFFER_BIT = 0x00000100,
    STENCIL_BUFFER_BIT = 0x00000400,
    COLOR_BUFFER_BIT = 0x00004000
}

public enum GLShaderType : uint
{
    FRAGMENT_SHADER = 0x8B30,
    VERTEX_SHADER = 0x8B31,
    GL_COMPUTE_SHADER = 0x91B9,
    GL_TESS_CONTROL_SHADER = 0x8E88,
    GL_TESS_EVALUATION_SHADER = 0x8E87,
    GL_GEOMETRY_SHADER = 0x8DD9,
}

public enum GLGetShaderivNames : uint
{
    SHADER_TYPE = 0x8B4F,
    DELETE_STATUS = 0x8B80,
    COMPILE_STATUS = 0x8B81,
    INFO_LOG_LENGTH = 0x8B84,
    SHADER_SOURCE_LENGTH = 0x8B88,
}

public enum GLTargetType : uint
{
    ARRAY_BUFFER = 0x8892,
    GL_ATOMIC_COUNTER_BUFFER = 0x92C0,
    GL_COPY_READ_BUFFER = 0x8F36,
    GL_COPY_WRITE_BUFFER = 0x8F37,
    GL_DISPATCH_INDIRECT_BUFFER = 0x90EE,
    GL_DRAW_INDIRECT_BUFFER = 0x8F3F,
    ELEMENT_ARRAY_BUFFER = 0x8893,
    GL_PIXEL_PACK_BUFFER = 0x88EB,
    GL_PIXEL_UNPACK_BUFFER = 0x88EC,
    GL_QUERY_BUFFER = 0x9192,
    GL_SHADER_STORAGE_BUFFER = 0x90D2,
    GL_TEXTURE_BUFFER = 0x8C2A,
    GL_TRANSFORM_FEEDBACK_BUFFER = 0x8C8E,
    GL_UNIFORM_BUFFER = 0x8A11,
}

public enum GLUsageType
{
    GL_STREAM_DRAW = 0x88E0,
    GL_STREAM_READ = 0x88E1,
    GL_STREAM_COPY = 0x88E2,
    STATIC_DRAW = 0x88E4,
    GL_STATIC_READ = 0x88E5,
    GL_STATIC_COPY = 0x88E6,
    GL_DYNAMIC_DRAW = 0x88E8,
    GL_DYNAMIC_READ = 0x88E9,
    GL_DYNAMIC_COPY = 0x88EA,
}

public enum GL : uint
{
    STENCIL_BUFFER_BIT = 0x00000400,
    DEPTH_BUFFER_BIT = 0x0000100,
    COLOR_BUFFER_BIT = 0x00004000,
    FALSE = 0,
    TRUE = 1,
    POINTS = 0x0000,
    LINES = 0x0001,
    LINE_LOOP = 0x0002,
    LINE_STRIP = 0x0003,
    TRIANGLES = 0x0004,
    TRIANGLE_STRIP = 0x0005,
    TRIANGLE_FAN = 0x0006,
    QUADS = 0x0007,
    NEVER = 0x0200,
    LESS = 0x0201,
    EQUAL = 0x0202,
    LEQUAL = 0x0203,
    GREATER = 0x0204,
    NOTEQUAL = 0x0205,
    GEQUAL = 0x0206,
    ALWAYS = 0x0207,
    ZERO = 0,
    ONE = 1,
    SRC_COLOR = 0x0300,
    ONE_MINUS_SRC_COLOR = 0x0301,
    SRC_ALPHA = 0x0302,
    ONE_MINUS_SRC_ALPHA = 0x0303,
    DST_ALPHA = 0x0304,
    ONE_MINUS_DST_ALPHA = 0x0305,
    DST_COLOR = 0x0306,
    ONE_MINUS_DST_COLOR = 0x0307,
    SRC_ALPHA_SATURATE = 0x0308,
    NONE = 0,
    FRONT_LEFT = 0x0400,
    FRONT_RIGHT = 0x0401,
    BACK_LEFT = 0x0402,
    BACK_RIGHT = 0x0403,
    FRONT = 0x0404,
    BACK = 0x0405,
    LEFT = 0x0406,
    RIGHT = 0x0407,
    FRONT_AND_BACK = 0x0408,
    NO_ERROR = 0,
    INVALID_ENUM = 0x0500,
    INVALID_VALUE = 0x0501,
    INVALID_OPERATION = 0x0502,
    OUT_OF_MEMORY = 0x0505,
    CW = 0x0900,
    CCW = 0x0901,
    POINT_SIZE = 0x0B11,
    POINT_SIZE_RANGE = 0x0B12,
    POINT_SIZE_GRANULARITY = 0x0B13,
    LINE_SMOOTH = 0x0B20,
    LINE_WIDTH = 0x0B21,
    LINE_WIDTH_RANGE = 0x0B22,
    LINE_WIDTH_GRANULARITY = 0x0B23,
    POLYGON_MODE = 0x0B40,
    POLYGON_SMOOTH = 0x0B41,
    CULL_FACE = 0x0B44,
    CULL_FACE_MODE = 0x0B45,
    FRONT_FACE = 0x0B46,
    DEPTH_RANGE = 0x0B70,
    DEPTH_TEST = 0x0B71,
    DEPTH_WRITEMASK = 0x0B72,
    DEPTH_CLEAR_VALUE = 0x0B73,
    DEPTH_FUNC = 0x0B74,
    STENCIL_TEST = 0x0B90,
    STENCIL_CLEAR_VALUE = 0x0B91,
    STENCIL_FUNC = 0x0B92,
    STENCIL_VALUE_MASK = 0x0B93,
    STENCIL_FAIL = 0x0B94,
    STENCIL_PASS_DEPTH_FAIL = 0x0B95,
    STENCIL_PASS_DEPTH_PASS = 0x0B96,
    STENCIL_REF = 0x0B97,
    STENCIL_WRITEMASK = 0x0B98,
    VIEWPORT = 0x0BA2,
    DITHER = 0x0BD0,
    BLEND_DST = 0x0BE0,
    BLEND_SRC = 0x0BE1,
    BLEND = 0x0BE2,
    LOGIC_OP_MODE = 0x0BF0,
    DRAW_BUFFER = 0x0C01,
    READ_BUFFER = 0x0C02,
    SCISSOR_BOX = 0x0C10,
    SCISSOR_TEST = 0x0C11,
    COLOR_CLEAR_VALUE = 0x0C22,
    COLOR_WRITEMASK = 0x0C23,
    DOUBLEBUFFER = 0x0C32,
    STEREO = 0x0C33,
    LINE_SMOOTH_HINT = 0x0C52,
    POLYGON_SMOOTH_HINT = 0x0C53,
    UNPACK_SWAP_BYTES = 0x0CF0,
    UNPACK_LSB_FIRST = 0x0CF1,
    UNPACK_ROW_LENGTH = 0x0CF2,
    UNPACK_SKIP_ROWS = 0x0CF3,
    UNPACK_SKIP_PIXELS = 0x0CF4,
    UNPACK_ALIGNMENT = 0x0CF5,
    PACK_SWAP_BYTES = 0x0D00,
    PACK_LSB_FIRST = 0x0D01,
    PACK_ROW_LENGTH = 0x0D02,
    PACK_SKIP_ROWS = 0x0D03,
    PACK_SKIP_PIXELS = 0x0D04,
    PACK_ALIGNMENT = 0x0D05,
    MAX_TEXTURE_SIZE = 0x0D33,
    MAX_VIEWPORT_DIMS = 0x0D3A,
    SUBPIXEL_BITS = 0x0D50,
    TEXTURE_1D = 0x0DE0,
    TEXTURE_2D = 0x0DE1,
    TEXTURE_WIDTH = 0x1000,
    TEXTURE_HEIGHT = 0x1001,
    TEXTURE_BORDER_COLOR = 0x1004,
    DONT_CARE = 0x1100,
    FASTEST = 0x1101,
    NICEST = 0x1102,
    BYTE = 0x1400,
    UNSIGNED_BYTE = 0x1401,
    SHORT = 0x1402,
    UNSIGNED_SHORT = 0x1403,
    INT = 0x1404,
    UNSIGNED_INT = 0x1405,
    FLOAT = 0x1406,
    STACK_OVERFLOW = 0x0503,
    STACK_UNDERFLOW = 0x0504,
    CLEAR = 0x1500,
    AND = 0x1501,
    AND_REVERSE = 0x1502,
    COPY = 0x1503,
    AND_INVERTED = 0x1504,
    NOOP = 0x1505,
    XOR = 0x1506,
    OR = 0x1507,
    NOR = 0x1508,
    EQUIV = 0x1509,
    INVERT = 0x150A,
    OR_REVERSE = 0x150B,
    COPY_INVERTED = 0x150C,
    OR_INVERTED = 0x150D,
    NAND = 0x150E,
    SET = 0x150F,
    TEXTURE = 0x1702,
    COLOR = 0x1800,
    DEPTH = 0x1801,
    STENCIL = 0x1802,
    STENCIL_INDEX = 0x1901,
    DEPTH_COMPONENT = 0x1902,
    RED = 0x1903,
    GREEN = 0x1904,
    BLUE = 0x1905,
    ALPHA = 0x1906,
    RGB = 0x1907,
    RGBA = 0x1908,
    POINT = 0x1B00,
    LINE = 0x1B01,
    FILL = 0x1B02,
    KEEP = 0x1E00,
    REPLACE = 0x1E01,
    INCR = 0x1E02,
    DECR = 0x1E03,
    VENDOR = 0x1F00,
    RENDERER = 0x1F01,
    VERSION = 0x1F02,
    EXTENSIONS = 0x1F03,
    NEAREST = 0x2600,
    LINEAR = 0x2601,
    NEAREST_MIPMAP_NEAREST = 0x2700,
    LINEAR_MIPMAP_NEAREST = 0x2701,
    NEAREST_MIPMAP_LINEAR = 0x2702,
    LINEAR_MIPMAP_LINEAR = 0x2703,
    TEXTURE_MAG_FILTER = 0x2800,
    TEXTURE_MIN_FILTER = 0x2801,
    TEXTURE_WRAP_S = 0x2802,
    TEXTURE_WRAP_T = 0x2803,
    REPEAT = 0x2901,
    COLOR_LOGIC_OP = 0x0BF2,
    POLYGON_OFFSET_UNITS = 0x2A00,
    POLYGON_OFFSET_POINT = 0x2A01,
    POLYGON_OFFSET_LINE = 0x2A02,
    POLYGON_OFFSET_FILL = 0x8037,
    POLYGON_OFFSET_FACTOR = 0x8038,
    TEXTURE_BINDING_1D = 0x8068,
    TEXTURE_BINDING_2D = 0x8069,
    TEXTURE_INTERNAL_FORMAT = 0x1003,
    TEXTURE_RED_SIZE = 0x805C,
    TEXTURE_GREEN_SIZE = 0x805D,
    TEXTURE_BLUE_SIZE = 0x805E,
    TEXTURE_ALPHA_SIZE = 0x805F,
    DOUBLE = 0x140A,
    PROXY_TEXTURE_1D = 0x8063,
    PROXY_TEXTURE_2D = 0x8064,
    R3_G3_B2 = 0x2A10,
    RGB4 = 0x804F,
    RGB5 = 0x8050,
    RGB8 = 0x8051,
    RGB10 = 0x8052,
    RGB12 = 0x8053,
    RGB16 = 0x8054,
    RGBA2 = 0x8055,
    RGBA4 = 0x8056,
    RGB5_A1 = 0x8057,
    RGBA8 = 0x8058,
    RGB10_A2 = 0x8059,
    RGBA12 = 0x805A,
    RGBA16 = 0x805B,
    VERTEX_ARRAY = 0x8074,
    UNSIGNED_BYTE_3_3_2 = 0x8032,
    UNSIGNED_SHORT_4_4_4_4 = 0x8033,
    UNSIGNED_SHORT_5_5_5_1 = 0x8034,
    UNSIGNED_INT_8_8_8_8 = 0x8035,
    UNSIGNED_INT_10_10_10_2 = 0x8036,
    TEXTURE_BINDING_3D = 0x806A,
    PACK_SKIP_IMAGES = 0x806B,
    PACK_IMAGE_HEIGHT = 0x806C,
    UNPACK_SKIP_IMAGES = 0x806D,
    UNPACK_IMAGE_HEIGHT = 0x806E,
    TEXTURE_3D = 0x806F,
    PROXY_TEXTURE_3D = 0x8070,
    TEXTURE_DEPTH = 0x8071,
    TEXTURE_WRAP_R = 0x8072,
    MAX_3D_TEXTURE_SIZE = 0x8073,
    UNSIGNED_BYTE_2_3_3_REV = 0x8362,
    UNSIGNED_SHORT_5_6_5 = 0x8363,
    UNSIGNED_SHORT_5_6_5_REV = 0x8364,
    UNSIGNED_SHORT_4_4_4_4_REV = 0x8365,
    UNSIGNED_SHORT_1_5_5_5_REV = 0x8366,
    UNSIGNED_INT_8_8_8_8_REV = 0x8367,
    UNSIGNED_INT_2_10_10_10_REV = 0x8368,
    BGR = 0x80E0,
    BGRA = 0x80E1,
    MAX_ELEMENTS_VERTICES = 0x80E8,
    MAX_ELEMENTS_INDICES = 0x80E9,
    CLAMP_TO_EDGE = 0x812F,
    TEXTURE_MIN_LOD = 0x813A,
    TEXTURE_MAX_LOD = 0x813B,
    TEXTURE_BASE_LEVEL = 0x813C,
    TEXTURE_MAX_LEVEL = 0x813D,
    SMOOTH_POINT_SIZE_RANGE = 0x0B12,
    SMOOTH_POINT_SIZE_GRANULARITY = 0x0B13,
    SMOOTH_LINE_WIDTH_RANGE = 0x0B22,
    SMOOTH_LINE_WIDTH_GRANULARITY = 0x0B23,
    ALIASED_LINE_WIDTH_RANGE = 0x846E,
    TEXTURE0 = 0x84C0,
    TEXTURE1 = 0x84C1,
    TEXTURE2 = 0x84C2,
    TEXTURE3 = 0x84C3,
    TEXTURE4 = 0x84C4,
    TEXTURE5 = 0x84C5,
    TEXTURE6 = 0x84C6,
    TEXTURE7 = 0x84C7,
    TEXTURE8 = 0x84C8,
    TEXTURE9 = 0x84C9,
    TEXTURE10 = 0x84CA,
    TEXTURE11 = 0x84CB,
    TEXTURE12 = 0x84CC,
    TEXTURE13 = 0x84CD,
    TEXTURE14 = 0x84CE,
    TEXTURE15 = 0x84CF,
    TEXTURE16 = 0x84D0,
    TEXTURE17 = 0x84D1,
    TEXTURE18 = 0x84D2,
    TEXTURE19 = 0x84D3,
    TEXTURE20 = 0x84D4,
    TEXTURE21 = 0x84D5,
    TEXTURE22 = 0x84D6,
    TEXTURE23 = 0x84D7,
    TEXTURE24 = 0x84D8,
    TEXTURE25 = 0x84D9,
    TEXTURE26 = 0x84DA,
    TEXTURE27 = 0x84DB,
    TEXTURE28 = 0x84DC,
    TEXTURE29 = 0x84DD,
    TEXTURE30 = 0x84DE,
    TEXTURE31 = 0x84DF,
    ACTIVE_TEXTURE = 0x84E0,
    MULTISAMPLE = 0x809D,
    SAMPLE_ALPHA_TO_COVERAGE = 0x809E,
    SAMPLE_ALPHA_TO_ONE = 0x809F,
    SAMPLE_COVERAGE = 0x80A0,
    SAMPLE_BUFFERS = 0x80A8,
    SAMPLES = 0x80A9,
    SAMPLE_COVERAGE_VALUE = 0x80AA,
    SAMPLE_COVERAGE_INVERT = 0x80AB,
    TEXTURE_CUBE_MAP = 0x8513,
    TEXTURE_BINDING_CUBE_MAP = 0x8514,
    TEXTURE_CUBE_MAP_POSITIVE_X = 0x8515,
    TEXTURE_CUBE_MAP_NEGATIVE_X = 0x8516,
    TEXTURE_CUBE_MAP_POSITIVE_Y = 0x8517,
    TEXTURE_CUBE_MAP_NEGATIVE_Y = 0x8518,
    TEXTURE_CUBE_MAP_POSITIVE_Z = 0x8519,
    TEXTURE_CUBE_MAP_NEGATIVE_Z = 0x851A,
    PROXY_TEXTURE_CUBE_MAP = 0x851B,
    MAX_CUBE_MAP_TEXTURE_SIZE = 0x851C,
    COMPRESSED_RGB = 0x84ED,
    COMPRESSED_RGBA = 0x84EE,
    TEXTURE_COMPRESSION_HINT = 0x84EF,
    TEXTURE_COMPRESSED_IMAGE_SIZE = 0x86A0,
    TEXTURE_COMPRESSED = 0x86A1,
    NUM_COMPRESSED_TEXTURE_FORMATS = 0x86A2,
    COMPRESSED_TEXTURE_FORMATS = 0x86A3,
    CLAMP_TO_BORDER = 0x812D,
    BLEND_DST_RGB = 0x80C8,
    BLEND_SRC_RGB = 0x80C9,
    BLEND_DST_ALPHA = 0x80CA,
    BLEND_SRC_ALPHA = 0x80CB,
    POINT_FADE_THRESHOLD_SIZE = 0x8128,
    DEPTH_COMPONENT16 = 0x81A5,
    DEPTH_COMPONENT24 = 0x81A6,
    DEPTH_COMPONENT32 = 0x81A7,
    MIRRORED_REPEAT = 0x8370,
    MAX_TEXTURE_LOD_BIAS = 0x84FD,
    TEXTURE_LOD_BIAS = 0x8501,
    INCR_WRAP = 0x8507,
    DECR_WRAP = 0x8508,
    TEXTURE_DEPTH_SIZE = 0x884A,
    TEXTURE_COMPARE_MODE = 0x884C,
    TEXTURE_COMPARE_FUNC = 0x884D,
    BLEND_COLOR = 0x8005,
    BLEND_EQUATION = 0x8009,
    CONSTANT_COLOR = 0x8001,
    ONE_MINUS_CONSTANT_COLOR = 0x8002,
    CONSTANT_ALPHA = 0x8003,
    ONE_MINUS_CONSTANT_ALPHA = 0x8004,
    FUNC_ADD = 0x8006,
    FUNC_REVERSE_SUBTRACT = 0x800B,
    FUNC_SUBTRACT = 0x800A,
    MIN = 0x8007,
    MAX = 0x8008,
    BUFFER_SIZE = 0x8764,
    BUFFER_USAGE = 0x8765,
    QUERY_COUNTER_BITS = 0x8864,
    CURRENT_QUERY = 0x8865,
    QUERY_RESULT = 0x8866,
    QUERY_RESULT_AVAILABLE = 0x8867,
    ARRAY_BUFFER = 0x8892,
    ELEMENT_ARRAY_BUFFER = 0x8893,
    ARRAY_BUFFER_BINDING = 0x8894,
    ELEMENT_ARRAY_BUFFER_BINDING = 0x8895,
    VERTEX_ATTRIB_ARRAY_BUFFER_BINDING = 0x889F,
    READ_ONLY = 0x88B8,
    WRITE_ONLY = 0x88B9,
    READ_WRITE = 0x88BA,
    BUFFER_ACCESS = 0x88BB,
    BUFFER_MAPPED = 0x88BC,
    BUFFER_MAP_POINTER = 0x88BD,
    STREAM_DRAW = 0x88E0,
    STREAM_READ = 0x88E1,
    STREAM_COPY = 0x88E2,
    STATIC_DRAW = 0x88E4,
    STATIC_READ = 0x88E5,
    STATIC_COPY = 0x88E6,
    DYNAMIC_DRAW = 0x88E8,
    DYNAMIC_READ = 0x88E9,
    DYNAMIC_COPY = 0x88EA,
    SAMPLES_PASSED = 0x8914,
    SRC1_ALPHA = 0x8589,
    BLEND_EQUATION_RGB = 0x8009,
    VERTEX_ATTRIB_ARRAY_ENABLED = 0x8622,
    VERTEX_ATTRIB_ARRAY_SIZE = 0x8623,
    VERTEX_ATTRIB_ARRAY_STRIDE = 0x8624,
    VERTEX_ATTRIB_ARRAY_TYPE = 0x8625,
    CURRENT_VERTEX_ATTRIB = 0x8626,
    VERTEX_PROGRAM_POINT_SIZE = 0x8642,
    VERTEX_ATTRIB_ARRAY_POINTER = 0x8645,
    STENCIL_BACK_FUNC = 0x8800,
    STENCIL_BACK_FAIL = 0x8801,
    STENCIL_BACK_PASS_DEPTH_FAIL = 0x8802,
    STENCIL_BACK_PASS_DEPTH_PASS = 0x8803,
    MAX_DRAW_BUFFERS = 0x8824,
    DRAW_BUFFER0 = 0x8825,
    DRAW_BUFFER1 = 0x8826,
    DRAW_BUFFER2 = 0x8827,
    DRAW_BUFFER3 = 0x8828,
    DRAW_BUFFER4 = 0x8829,
    DRAW_BUFFER5 = 0x882A,
    DRAW_BUFFER6 = 0x882B,
    DRAW_BUFFER7 = 0x882C,
    DRAW_BUFFER8 = 0x882D,
    DRAW_BUFFER9 = 0x882E,
    DRAW_BUFFER10 = 0x882F,
    DRAW_BUFFER11 = 0x8830,
    DRAW_BUFFER12 = 0x8831,
    DRAW_BUFFER13 = 0x8832,
    DRAW_BUFFER14 = 0x8833,
    DRAW_BUFFER15 = 0x8834,
    BLEND_EQUATION_ALPHA = 0x883D,
    MAX_VERTEX_ATTRIBS = 0x8869,
    VERTEX_ATTRIB_ARRAY_NORMALIZED = 0x886A,
    MAX_TEXTURE_IMAGE_UNITS = 0x8872,
    FRAGMENT_SHADER = 0x8B30,
    VERTEX_SHADER = 0x8B31,
    MAX_FRAGMENT_UNIFORM_COMPONENTS = 0x8B49,
    MAX_VERTEX_UNIFORM_COMPONENTS = 0x8B4A,
    MAX_VARYING_FLOATS = 0x8B4B,
    MAX_VERTEX_TEXTURE_IMAGE_UNITS = 0x8B4C,
    MAX_COMBINED_TEXTURE_IMAGE_UNITS = 0x8B4D,
    SHADER_TYPE = 0x8B4F,
    FLOAT_VEC2 = 0x8B50,
    FLOAT_VEC3 = 0x8B51,
    FLOAT_VEC4 = 0x8B52,
    INT_VEC2 = 0x8B53,
    INT_VEC3 = 0x8B54,
    INT_VEC4 = 0x8B55,
    BOOL = 0x8B56,
    BOOL_VEC2 = 0x8B57,
    BOOL_VEC3 = 0x8B58,
    BOOL_VEC4 = 0x8B59,
    FLOAT_MAT2 = 0x8B5A,
    FLOAT_MAT3 = 0x8B5B,
    FLOAT_Matrix4x4 = 0x8B5C,
    SAMPLER_1D = 0x8B5D,
    SAMPLER_2D = 0x8B5E,
    SAMPLER_3D = 0x8B5F,
    SAMPLER_CUBE = 0x8B60,
    SAMPLER_1D_SHADOW = 0x8B61,
    SAMPLER_2D_SHADOW = 0x8B62,
    DELETE_STATUS = 0x8B80,
    COMPILE_STATUS = 0x8B81,
    LINK_STATUS = 0x8B82,
    VALIDATE_STATUS = 0x8B83,
    INFO_LOG_LENGTH = 0x8B84,
    ATTACHED_SHADERS = 0x8B85,
    ACTIVE_UNIFORMS = 0x8B86,
    ACTIVE_UNIFORM_MAX_LENGTH = 0x8B87,
    SHADER_SOURCE_LENGTH = 0x8B88,
    ACTIVE_ATTRIBUTES = 0x8B89,
    ACTIVE_ATTRIBUTE_MAX_LENGTH = 0x8B8A,
    FRAGMENT_SHADER_DERIVATIVE_HINT = 0x8B8B,
    SHADING_LANGUAGE_VERSION = 0x8B8C,
    CURRENT_PROGRAM = 0x8B8D,
    POINT_SPRITE_COORD_ORIGIN = 0x8CA0,
    LOWER_LEFT = 0x8CA1,
    UPPER_LEFT = 0x8CA2,
    STENCIL_BACK_REF = 0x8CA3,
    STENCIL_BACK_VALUE_MASK = 0x8CA4,
    STENCIL_BACK_WRITEMASK = 0x8CA5,
    PIXEL_PACK_BUFFER = 0x88EB,
    PIXEL_UNPACK_BUFFER = 0x88EC,
    PIXEL_PACK_BUFFER_BINDING = 0x88ED,
    PIXEL_UNPACK_BUFFER_BINDING = 0x88EF,
    FLOAT_MAT2x3 = 0x8B65,
    FLOAT_MAT2x4 = 0x8B66,
    FLOAT_MAT3x2 = 0x8B67,
    FLOAT_MAT3x4 = 0x8B68,
    FLOAT_Matrix4x4x2 = 0x8B69,
    FLOAT_Matrix4x4x3 = 0x8B6A,
    SRGB = 0x8C40,
    SRGB8 = 0x8C41,
    SRGB_ALPHA = 0x8C42,
    SRGB8_ALPHA8 = 0x8C43,
    COMPRESSED_SRGB = 0x8C48,
    COMPRESSED_SRGB_ALPHA = 0x8C49,
    COMPARE_REF_TO_TEXTURE = 0x884E,
    CLIP_DISTANCE0 = 0x3000,
    CLIP_DISTANCE1 = 0x3001,
    CLIP_DISTANCE2 = 0x3002,
    CLIP_DISTANCE3 = 0x3003,
    CLIP_DISTANCE4 = 0x3004,
    CLIP_DISTANCE5 = 0x3005,
    CLIP_DISTANCE6 = 0x3006,
    CLIP_DISTANCE7 = 0x3007,
    MAX_CLIP_DISTANCES = 0x0D32,
    MAJOR_VERSION = 0x821B,
    MINOR_VERSION = 0x821C,
    NUM_EXTENSIONS = 0x821D,
    CONTEXT_FLAGS = 0x821E,
    COMPRESSED_RED = 0x8225,
    COMPRESSED_RG = 0x8226,
    CONTEXT_FLAG_FORWARD_COMPATIBLE_BIT = 0x00000001,
    RGBA32F = 0x8814,
    RGB32F = 0x8815,
    RGBA16F = 0x881A,
    RGB16F = 0x881B,
    VERTEX_ATTRIB_ARRAY_INTEGER = 0x88FD,
    MAX_ARRAY_TEXTURE_LAYERS = 0x88FF,
    MIN_PROGRAM_TEXEL_OFFSET = 0x8904,
    MAX_PROGRAM_TEXEL_OFFSET = 0x8905,
    CLAMP_READ_COLOR = 0x891C,
    FIXED_ONLY = 0x891D,
    MAX_VARYING_COMPONENTS = 0x8B4B,
    TEXTURE_1D_ARRAY = 0x8C18,
    PROXY_TEXTURE_1D_ARRAY = 0x8C19,
    TEXTURE_2D_ARRAY = 0x8C1A,
    PROXY_TEXTURE_2D_ARRAY = 0x8C1B,
    TEXTURE_BINDING_1D_ARRAY = 0x8C1C,
    TEXTURE_BINDING_2D_ARRAY = 0x8C1D,
    R11F_G11F_B10F = 0x8C3A,
    UNSIGNED_INT_10F_11F_11F_REV = 0x8C3B,
    RGB9_E5 = 0x8C3D,
    UNSIGNED_INT_5_9_9_9_REV = 0x8C3E,
    TEXTURE_SHARED_SIZE = 0x8C3F,
    TRANSFORM_FEEDBACK_VARYING_MAX_LENGTH = 0x8C76,
    TRANSFORM_FEEDBACK_BUFFER_MODE = 0x8C7F,
    MAX_TRANSFORM_FEEDBACK_SEPARATE_COMPONENTS = 0x8C80,
    TRANSFORM_FEEDBACK_VARYINGS = 0x8C83,
    TRANSFORM_FEEDBACK_BUFFER_START = 0x8C84,
    TRANSFORM_FEEDBACK_BUFFER_SIZE = 0x8C85,
    PRIMITIVES_GENERATED = 0x8C87,
    TRANSFORM_FEEDBACK_PRIMITIVES_WRITTEN = 0x8C88,
    RASTERIZER_DISCARD = 0x8C89,
    MAX_TRANSFORM_FEEDBACK_INTERLEAVED_COMPONENTS = 0x8C8A,
    MAX_TRANSFORM_FEEDBACK_SEPARATE_ATTRIBS = 0x8C8B,
    INTERLEAVED_ATTRIBS = 0x8C8C,
    SEPARATE_ATTRIBS = 0x8C8D,
    TRANSFORM_FEEDBACK_BUFFER = 0x8C8E,
    TRANSFORM_FEEDBACK_BUFFER_BINDING = 0x8C8F,
    RGBA32UI = 0x8D70,
    RGB32UI = 0x8D71,
    RGBA16UI = 0x8D76,
    RGB16UI = 0x8D77,
    RGBA8UI = 0x8D7C,
    RGB8UI = 0x8D7D,
    RGBA32I = 0x8D82,
    RGB32I = 0x8D83,
    RGBA16I = 0x8D88,
    RGB16I = 0x8D89,
    RGBA8I = 0x8D8E,
    RGB8I = 0x8D8F,
    RED_INTEGER = 0x8D94,
    GREEN_INTEGER = 0x8D95,
    BLUE_INTEGER = 0x8D96,
    RGB_INTEGER = 0x8D98,
    RGBA_INTEGER = 0x8D99,
    BGR_INTEGER = 0x8D9A,
    BGRA_INTEGER = 0x8D9B,
    SAMPLER_1D_ARRAY = 0x8DC0,
    SAMPLER_2D_ARRAY = 0x8DC1,
    SAMPLER_1D_ARRAY_SHADOW = 0x8DC3,
    SAMPLER_2D_ARRAY_SHADOW = 0x8DC4,
    SAMPLER_CUBE_SHADOW = 0x8DC5,
    UNSIGNED_INT_Vector2 = 0x8DC6,
    UNSIGNED_INT_Vector3 = 0x8DC7,
    UNSIGNED_INT_Vector4 = 0x8DC8,
    INT_SAMPLER_1D = 0x8DC9,
    INT_SAMPLER_2D = 0x8DCA,
    INT_SAMPLER_3D = 0x8DCB,
    INT_SAMPLER_CUBE = 0x8DCC,
    INT_SAMPLER_1D_ARRAY = 0x8DCE,
    INT_SAMPLER_2D_ARRAY = 0x8DCF,
    UNSIGNED_INT_SAMPLER_1D = 0x8DD1,
    UNSIGNED_INT_SAMPLER_2D = 0x8DD2,
    UNSIGNED_INT_SAMPLER_3D = 0x8DD3,
    UNSIGNED_INT_SAMPLER_CUBE = 0x8DD4,
    UNSIGNED_INT_SAMPLER_1D_ARRAY = 0x8DD6,
    UNSIGNED_INT_SAMPLER_2D_ARRAY = 0x8DD7,
    QUERY_WAIT = 0x8E13,
    QUERY_NO_WAIT = 0x8E14,
    QUERY_BY_REGION_WAIT = 0x8E15,
    QUERY_BY_REGION_NO_WAIT = 0x8E16,
    BUFFER_ACCESS_FLAGS = 0x911F,
    BUFFER_MAP_LENGTH = 0x9120,
    BUFFER_MAP_OFFSET = 0x9121,
    DEPTH_COMPONENT32F = 0x8CAC,
    DEPTH32F_STENCIL8 = 0x8CAD,
    FLOAT_32_UNSIGNED_INT_24_8_REV = 0x8DAD,
    INVALID_FRAMEBUFFER_OPERATION = 0x0506,
    FRAMEBUFFER_ATTACHMENT_COLOR_ENCODING = 0x8210,
    FRAMEBUFFER_ATTACHMENT_COMPONENT_TYPE = 0x8211,
    FRAMEBUFFER_ATTACHMENT_RED_SIZE = 0x8212,
    FRAMEBUFFER_ATTACHMENT_GREEN_SIZE = 0x8213,
    FRAMEBUFFER_ATTACHMENT_BLUE_SIZE = 0x8214,
    FRAMEBUFFER_ATTACHMENT_ALPHA_SIZE = 0x8215,
    FRAMEBUFFER_ATTACHMENT_DEPTH_SIZE = 0x8216,
    FRAMEBUFFER_ATTACHMENT_STENCIL_SIZE = 0x8217,
    FRAMEBUFFER_DEFAULT = 0x8218,
    FRAMEBUFFER_UNDEFINED = 0x8219,
    DEPTH_STENCIL_ATTACHMENT = 0x821A,
    MAX_RENDERBUFFER_SIZE = 0x84E8,
    DEPTH_STENCIL = 0x84F9,
    UNSIGNED_INT_24_8 = 0x84FA,
    DEPTH24_STENCIL8 = 0x88F0,
    TEXTURE_STENCIL_SIZE = 0x88F1,
    TEXTURE_RED_TYPE = 0x8C10,
    TEXTURE_GREEN_TYPE = 0x8C11,
    TEXTURE_BLUE_TYPE = 0x8C12,
    TEXTURE_ALPHA_TYPE = 0x8C13,
    TEXTURE_DEPTH_TYPE = 0x8C16,
    UNSIGNED_NORMALIZED = 0x8C17,
    FRAMEBUFFER_BINDING = 0x8CA6,
    DRAW_FRAMEBUFFER_BINDING = 0x8CA6,
    RENDERBUFFER_BINDING = 0x8CA7,
    READ_FRAMEBUFFER = 0x8CA8,
    DRAW_FRAMEBUFFER = 0x8CA9,
    READ_FRAMEBUFFER_BINDING = 0x8CAA,
    RENDERBUFFER_SAMPLES = 0x8CAB,
    FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE = 0x8CD0,
    FRAMEBUFFER_ATTACHMENT_OBJECT_NAME = 0x8CD1,
    FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL = 0x8CD2,
    FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE = 0x8CD3,
    FRAMEBUFFER_ATTACHMENT_TEXTURE_LAYER = 0x8CD4,
    FRAMEBUFFER_COMPLETE = 0x8CD5,
    FRAMEBUFFER_INCOMPLETE_ATTACHMENT = 0x8CD6,
    FRAMEBUFFER_INCOMPLETE_MISSING_ATTACHMENT = 0x8CD7,
    FRAMEBUFFER_INCOMPLETE_DRAW_BUFFER = 0x8CDB,
    FRAMEBUFFER_INCOMPLETE_READ_BUFFER = 0x8CDC,
    FRAMEBUFFER_UNSUPPORTED = 0x8CDD,
    MAX_COLOR_ATTACHMENTS = 0x8CDF,
    COLOR_ATTACHMENT0 = 0x8CE0,
    COLOR_ATTACHMENT1 = 0x8CE1,
    COLOR_ATTACHMENT2 = 0x8CE2,
    COLOR_ATTACHMENT3 = 0x8CE3,
    COLOR_ATTACHMENT4 = 0x8CE4,
    COLOR_ATTACHMENT5 = 0x8CE5,
    COLOR_ATTACHMENT6 = 0x8CE6,
    COLOR_ATTACHMENT7 = 0x8CE7,
    COLOR_ATTACHMENT8 = 0x8CE8,
    COLOR_ATTACHMENT9 = 0x8CE9,
    COLOR_ATTACHMENT10 = 0x8CEA,
    COLOR_ATTACHMENT11 = 0x8CEB,
    COLOR_ATTACHMENT12 = 0x8CEC,
    COLOR_ATTACHMENT13 = 0x8CED,
    COLOR_ATTACHMENT14 = 0x8CEE,
    COLOR_ATTACHMENT15 = 0x8CEF,
    COLOR_ATTACHMENT16 = 0x8CF0,
    COLOR_ATTACHMENT17 = 0x8CF1,
    COLOR_ATTACHMENT18 = 0x8CF2,
    COLOR_ATTACHMENT19 = 0x8CF3,
    COLOR_ATTACHMENT20 = 0x8CF4,
    COLOR_ATTACHMENT21 = 0x8CF5,
    COLOR_ATTACHMENT22 = 0x8CF6,
    COLOR_ATTACHMENT23 = 0x8CF7,
    COLOR_ATTACHMENT24 = 0x8CF8,
    COLOR_ATTACHMENT25 = 0x8CF9,
    COLOR_ATTACHMENT26 = 0x8CFA,
    COLOR_ATTACHMENT27 = 0x8CFB,
    COLOR_ATTACHMENT28 = 0x8CFC,
    COLOR_ATTACHMENT29 = 0x8CFD,
    COLOR_ATTACHMENT30 = 0x8CFE,
    COLOR_ATTACHMENT31 = 0x8CFF,
    DEPTH_ATTACHMENT = 0x8D00,
    STENCIL_ATTACHMENT = 0x8D20,
    FRAMEBUFFER = 0x8D40,
    RENDERBUFFER = 0x8D41,
    RENDERBUFFER_WIDTH = 0x8D42,
    RENDERBUFFER_HEIGHT = 0x8D43,
    RENDERBUFFER_INTERNAL_FORMAT = 0x8D44,
    STENCIL_INDEX1 = 0x8D46,
    STENCIL_INDEX4 = 0x8D47,
    STENCIL_INDEX8 = 0x8D48,
    STENCIL_INDEX16 = 0x8D49,
    RENDERBUFFER_RED_SIZE = 0x8D50,
    RENDERBUFFER_GREEN_SIZE = 0x8D51,
    RENDERBUFFER_BLUE_SIZE = 0x8D52,
    RENDERBUFFER_ALPHA_SIZE = 0x8D53,
    RENDERBUFFER_DEPTH_SIZE = 0x8D54,
    RENDERBUFFER_STENCIL_SIZE = 0x8D55,
    FRAMEBUFFER_INCOMPLETE_MULTISAMPLE = 0x8D56,
    MAX_SAMPLES = 0x8D57,
    FRAMEBUFFER_SRGB = 0x8DB9,
    HALF_FLOAT = 0x140B,
    MAP_READ_BIT = 0x0001,
    MAP_WRITE_BIT = 0x0002,
    MAP_INVALIDATE_RANGE_BIT = 0x0004,
    MAP_INVALIDATE_BUFFER_BIT = 0x0008,
    MAP_FLUSH_EXPLICIT_BIT = 0x0010,
    MAP_UNSYNCHRONIZED_BIT = 0x0020,
    COMPRESSED_RED_RGTC1 = 0x8DBB,
    COMPRESSED_SIGNED_RED_RGTC1 = 0x8DBC,
    COMPRESSED_RG_RGTC2 = 0x8DBD,
    COMPRESSED_SIGNED_RG_RGTC2 = 0x8DBE,
    RG = 0x8227,
    RG_INTEGER = 0x8228,
    R8 = 0x8229,
    R16 = 0x822A,
    RG8 = 0x822B,
    RG16 = 0x822C,
    R16F = 0x822D,
    R32F = 0x822E,
    RG16F = 0x822F,
    RG32F = 0x8230,
    R8I = 0x8231,
    R8UI = 0x8232,
    R16I = 0x8233,
    R16UI = 0x8234,
    R32I = 0x8235,
    R32UI = 0x8236,
    RG8I = 0x8237,
    RG8UI = 0x8238,
    RG16I = 0x8239,
    RG16UI = 0x823A,
    RG32I = 0x823B,
    RG32UI = 0x823C,
    VERTEX_ARRAY_BINDING = 0x85B5,
    SAMPLER_2D_RECT = 0x8B63,
    SAMPLER_2D_RECT_SHADOW = 0x8B64,
    SAMPLER_BUFFER = 0x8DC2,
    INT_SAMPLER_2D_RECT = 0x8DCD,
    INT_SAMPLER_BUFFER = 0x8DD0,
    UNSIGNED_INT_SAMPLER_2D_RECT = 0x8DD5,
    UNSIGNED_INT_SAMPLER_BUFFER = 0x8DD8,
    TEXTURE_BUFFER = 0x8C2A,
    MAX_TEXTURE_BUFFER_SIZE = 0x8C2B,
    TEXTURE_BINDING_BUFFER = 0x8C2C,
    TEXTURE_BUFFER_DATA_STORE_BINDING = 0x8C2D,
    TEXTURE_RECTANGLE = 0x84F5,
    TEXTURE_BINDING_RECTANGLE = 0x84F6,
    PROXY_TEXTURE_RECTANGLE = 0x84F7,
    MAX_RECTANGLE_TEXTURE_SIZE = 0x84F8,
    R8_SNORM = 0x8F94,
    RG8_SNORM = 0x8F95,
    RGB8_SNORM = 0x8F96,
    RGBA8_SNORM = 0x8F97,
    R16_SNORM = 0x8F98,
    RG16_SNORM = 0x8F99,
    RGB16_SNORM = 0x8F9A,
    RGBA16_SNORM = 0x8F9B,
    SIGNED_NORMALIZED = 0x8F9C,
    PRIMITIVE_RESTART = 0x8F9D,
    PRIMITIVE_RESTART_INDEX = 0x8F9E,
    COPY_READ_BUFFER = 0x8F36,
    COPY_WRITE_BUFFER = 0x8F37,
    UNIFORM_BUFFER = 0x8A11,
    UNIFORM_BUFFER_BINDING = 0x8A28,
    UNIFORM_BUFFER_START = 0x8A29,
    UNIFORM_BUFFER_SIZE = 0x8A2A,
    MAX_VERTEX_UNIFORM_BLOCKS = 0x8A2B,
    MAX_GEOMETRY_UNIFORM_BLOCKS = 0x8A2C,
    MAX_FRAGMENT_UNIFORM_BLOCKS = 0x8A2D,
    MAX_COMBINED_UNIFORM_BLOCKS = 0x8A2E,
    MAX_UNIFORM_BUFFER_BINDINGS = 0x8A2F,
    MAX_UNIFORM_BLOCK_SIZE = 0x8A30,
    MAX_COMBINED_VERTEX_UNIFORM_COMPONENTS = 0x8A31,
    MAX_COMBINED_GEOMETRY_UNIFORM_COMPONENTS = 0x8A32,
    MAX_COMBINED_FRAGMENT_UNIFORM_COMPONENTS = 0x8A33,
    UNIFORM_BUFFER_OFFSET_ALIGNMENT = 0x8A34,
    ACTIVE_UNIFORM_BLOCK_MAX_NAME_LENGTH = 0x8A35,
    ACTIVE_UNIFORM_BLOCKS = 0x8A36,
    UNIFORM_TYPE = 0x8A37,
    UNIFORM_SIZE = 0x8A38,
    UNIFORM_NAME_LENGTH = 0x8A39,
    UNIFORM_BLOCK_INDEX = 0x8A3A,
    UNIFORM_OFFSET = 0x8A3B,
    UNIFORM_ARRAY_STRIDE = 0x8A3C,
    UNIFORM_MATRIX_STRIDE = 0x8A3D,
    UNIFORM_IS_ROW_MAJOR = 0x8A3E,
    UNIFORM_BLOCK_BINDING = 0x8A3F,
    UNIFORM_BLOCK_DATA_SIZE = 0x8A40,
    UNIFORM_BLOCK_NAME_LENGTH = 0x8A41,
    UNIFORM_BLOCK_ACTIVE_UNIFORMS = 0x8A42,
    UNIFORM_BLOCK_ACTIVE_UNIFORM_INDICES = 0x8A43,
    UNIFORM_BLOCK_REFERENCED_BY_VERTEX_SHADER = 0x8A44,
    UNIFORM_BLOCK_REFERENCED_BY_GEOMETRY_SHADER = 0x8A45,
    UNIFORM_BLOCK_REFERENCED_BY_FRAGMENT_SHADER = 0x8A46,
    INVALID_INDEX = 0xFFFFFFFFu,
    CONTEXT_CORE_PROFILE_BIT = 0x00000001,
    CONTEXT_COMPATIBILITY_PROFILE_BIT = 0x00000002,
    LINES_ADJACENCY = 0x000A,
    LINE_STRIP_ADJACENCY = 0x000B,
    TRIANGLES_ADJACENCY = 0x000C,
    TRIANGLE_STRIP_ADJACENCY = 0x000D,
    PROGRAM_POINT_SIZE = 0x8642,
    MAX_GEOMETRY_TEXTURE_IMAGE_UNITS = 0x8C29,
    FRAMEBUFFER_ATTACHMENT_LAYERED = 0x8DA7,
    FRAMEBUFFER_INCOMPLETE_LAYER_TARGETS = 0x8DA8,
    GEOMETRY_SHADER = 0x8DD9,
    GEOMETRY_VERTICES_OUT = 0x8916,
    GEOMETRY_INPUT_TYPE = 0x8917,
    GEOMETRY_OUTPUT_TYPE = 0x8918,
    MAX_GEOMETRY_UNIFORM_COMPONENTS = 0x8DDF,
    MAX_GEOMETRY_OUTPUT_VERTICES = 0x8DE0,
    MAX_GEOMETRY_TOTAL_OUTPUT_COMPONENTS = 0x8DE1,
    MAX_VERTEX_OUTPUT_COMPONENTS = 0x9122,
    MAX_GEOMETRY_INPUT_COMPONENTS = 0x9123,
    MAX_GEOMETRY_OUTPUT_COMPONENTS = 0x9124,
    MAX_FRAGMENT_INPUT_COMPONENTS = 0x9125,
    CONTEXT_PROFILE_MASK = 0x9126,
    DEPTH_CLAMP = 0x864F,
    QUADS_FOLLOW_PROVOKING_VERTEX_CONVENTION = 0x8E4C,
    FIRST_VERTEX_CONVENTION = 0x8E4D,
    LAST_VERTEX_CONVENTION = 0x8E4E,
    PROVOKING_VERTEX = 0x8E4F,
    TEXTURE_CUBE_MAP_SEAMLESS = 0x884F,
    MAX_SERVER_WAIT_TIMEOUT = 0x9111,
    OBJECT_TYPE = 0x9112,
    SYNC_CONDITION = 0x9113,
    SYNC_STATUS = 0x9114,
    SYNC_FLAGS = 0x9115,
    SYNC_FENCE = 0x9116,
    SYNC_GPU_COMMANDS_COMPLETE = 0x9117,
    UNSIGNALED = 0x9118,
    SIGNALED = 0x9119,
    ALREADY_SIGNALED = 0x911A,
    TIMEOUT_EXPIRED = 0x911B,
    CONDITION_SATISFIED = 0x911C,
    WAIT_FAILED = 0x911D,
    TIMEOUT_IGNORED = 0xFFFFFFFF,
    SYNC_FLUSH_COMMANDS_BIT = 0x00000001,
    SAMPLE_POSITION = 0x8E50,
    SAMPLE_MASK = 0x8E51,
    SAMPLE_MASK_VALUE = 0x8E52,
    MAX_SAMPLE_MASK_WORDS = 0x8E59,
    TEXTURE_2D_MULTISAMPLE = 0x9100,
    PROXY_TEXTURE_2D_MULTISAMPLE = 0x9101,
    TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9102,
    PROXY_TEXTURE_2D_MULTISAMPLE_ARRAY = 0x9103,
    TEXTURE_BINDING_2D_MULTISAMPLE = 0x9104,
    TEXTURE_BINDING_2D_MULTISAMPLE_ARRAY = 0x9105,
    TEXTURE_SAMPLES = 0x9106,
    TEXTURE_FIXED_SAMPLE_LOCATIONS = 0x9107,
    SAMPLER_2D_MULTISAMPLE = 0x9108,
    INT_SAMPLER_2D_MULTISAMPLE = 0x9109,
    UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE = 0x910A,
    SAMPLER_2D_MULTISAMPLE_ARRAY = 0x910B,
    INT_SAMPLER_2D_MULTISAMPLE_ARRAY = 0x910C,
    UNSIGNED_INT_SAMPLER_2D_MULTISAMPLE_ARRAY = 0x910D,
    MAX_COLOR_TEXTURE_SAMPLES = 0x910E,
    MAX_DEPTH_TEXTURE_SAMPLES = 0x910F,
    MAX_INTEGER_SAMPLES = 0x9110,
    VERTEX_ATTRIB_ARRAY_DIVISOR = 0x88FE,
    SRC1_COLOR = 0x88F9,
    ONE_MINUS_SRC1_COLOR = 0x88FA,
    ONE_MINUS_SRC1_ALPHA = 0x88FB,
    MAX_DUAL_SOURCE_DRAW_BUFFERS = 0x88FC,
    ANY_SAMPLES_PASSED = 0x8C2F,
    SAMPLER_BINDING = 0x8919,
    RGB10_A2UI = 0x906F,
    TEXTURE_SWIZZLE_R = 0x8E42,
    TEXTURE_SWIZZLE_G = 0x8E43,
    TEXTURE_SWIZZLE_B = 0x8E44,
    TEXTURE_SWIZZLE_A = 0x8E45,
    TEXTURE_SWIZZLE_RGBA = 0x8E46,
    TIME_ELAPSED = 0x88BF,
    TIMESTAMP = 0x8E28,
    INT_2_10_10_10_REV = 0x8D9F,
    SAMPLE_SHADING = 0x8C36,
    MIN_SAMPLE_SHADING_VALUE = 0x8C37,
    MIN_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5E,
    MAX_PROGRAM_TEXTURE_GATHER_OFFSET = 0x8E5F,
    TEXTURE_CUBE_MAP_ARRAY = 0x9009,
    TEXTURE_BINDING_CUBE_MAP_ARRAY = 0x900A,
    PROXY_TEXTURE_CUBE_MAP_ARRAY = 0x900B,
    SAMPLER_CUBE_MAP_ARRAY = 0x900C,
    SAMPLER_CUBE_MAP_ARRAY_SHADOW = 0x900D,
    INT_SAMPLER_CUBE_MAP_ARRAY = 0x900E,
    UNSIGNED_INT_SAMPLER_CUBE_MAP_ARRAY = 0x900F,
    DRAW_INDIRECT_BUFFER = 0x8F3F,
    DRAW_INDIRECT_BUFFER_BINDING = 0x8F43,
    GEOMETRY_SHADER_INVOCATIONS = 0x887F,
    MAX_GEOMETRY_SHADER_INVOCATIONS = 0x8E5A,
    MIN_FRAGMENT_INTERPOLATION_OFFSET = 0x8E5B,
    MAX_FRAGMENT_INTERPOLATION_OFFSET = 0x8E5C,
    FRAGMENT_INTERPOLATION_OFFSET_BITS = 0x8E5D,
    MAX_VERTEX_STREAMS = 0x8E71,
    DOUBLE_Vector2 = 0x8FFC,
    DOUBLE_Vector3 = 0x8FFD,
    DOUBLE_Vector4 = 0x8FFE,
    DOUBLE_MAT2 = 0x8F46,
    DOUBLE_MAT3 = 0x8F47,
    DOUBLE_Matrix4x4 = 0x8F48,
    DOUBLE_MAT2x3 = 0x8F49,
    DOUBLE_MAT2x4 = 0x8F4A,
    DOUBLE_MAT3x2 = 0x8F4B,
    DOUBLE_MAT3x4 = 0x8F4C,
    DOUBLE_Matrix4x4x2 = 0x8F4D,
    DOUBLE_Matrix4x4x3 = 0x8F4E,
    ACTIVE_SUBROUTINES = 0x8DE5,
    ACTIVE_SUBROUTINE_UNIFORMS = 0x8DE6,
    ACTIVE_SUBROUTINE_UNIFORM_LOCATIONS = 0x8E47,
    ACTIVE_SUBROUTINE_MAX_LENGTH = 0x8E48,
    ACTIVE_SUBROUTINE_UNIFORM_MAX_LENGTH = 0x8E49,
    MAX_SUBROUTINES = 0x8DE7,
    MAX_SUBROUTINE_UNIFORM_LOCATIONS = 0x8DE8,
    NUM_COMPATIBLE_SUBROUTINES = 0x8E4A,
    COMPATIBLE_SUBROUTINES = 0x8E4B,
    PATCHES = 0x000E,
    PATCH_VERTICES = 0x8E72,
    PATCH_DEFAULT_INNER_LEVEL = 0x8E73,
    PATCH_DEFAULT_OUTER_LEVEL = 0x8E74,
    TESS_CONTROL_OUTPUT_VERTICES = 0x8E75,
    TESS_GEN_MODE = 0x8E76,
    TESS_GEN_SPACING = 0x8E77,
    TESS_GEN_VERTEX_ORDER = 0x8E78,
    TESS_GEN_POINT_MODE = 0x8E79,
    ISOLINES = 0x8E7A,
    FRACTIONAL_ODD = 0x8E7B,
    FRACTIONAL_EVEN = 0x8E7C,
    MAX_PATCH_VERTICES = 0x8E7D,
    MAX_TESS_GEN_LEVEL = 0x8E7E,
    MAX_TESS_CONTROL_UNIFORM_COMPONENTS = 0x8E7F,
    MAX_TESS_EVALUATION_UNIFORM_COMPONENTS = 0x8E80,
    MAX_TESS_CONTROL_TEXTURE_IMAGE_UNITS = 0x8E81,
    MAX_TESS_EVALUATION_TEXTURE_IMAGE_UNITS = 0x8E82,
    MAX_TESS_CONTROL_OUTPUT_COMPONENTS = 0x8E83,
    MAX_TESS_PATCH_COMPONENTS = 0x8E84,
    MAX_TESS_CONTROL_TOTAL_OUTPUT_COMPONENTS = 0x8E85,
    MAX_TESS_EVALUATION_OUTPUT_COMPONENTS = 0x8E86,
    MAX_TESS_CONTROL_UNIFORM_BLOCKS = 0x8E89,
    MAX_TESS_EVALUATION_UNIFORM_BLOCKS = 0x8E8A,
    MAX_TESS_CONTROL_INPUT_COMPONENTS = 0x886C,
    MAX_TESS_EVALUATION_INPUT_COMPONENTS = 0x886D,
    MAX_COMBINED_TESS_CONTROL_UNIFORM_COMPONENTS = 0x8E1E,
    MAX_COMBINED_TESS_EVALUATION_UNIFORM_COMPONENTS = 0x8E1F,
    UNIFORM_BLOCK_REFERENCED_BY_TESS_CONTROL_SHADER = 0x84F0,
    UNIFORM_BLOCK_REFERENCED_BY_TESS_EVALUATION_SHADER = 0x84F1,
    TESS_EVALUATION_SHADER = 0x8E87,
    TESS_CONTROL_SHADER = 0x8E88,
    TRANSFORM_FEEDBACK = 0x8E22,
    TRANSFORM_FEEDBACK_BUFFER_PAUSED = 0x8E23,
    TRANSFORM_FEEDBACK_BUFFER_ACTIVE = 0x8E24,
    TRANSFORM_FEEDBACK_BINDING = 0x8E25,
    MAX_TRANSFORM_FEEDBACK_BUFFERS = 0x8E70,
    FIXED = 0x140C,
    IMPLEMENTATION_COLOR_READ_TYPE = 0x8B9A,
    IMPLEMENTATION_COLOR_READ_FORMAT = 0x8B9B,
    LOW_FLOAT = 0x8DF0,
    MEDIUM_FLOAT = 0x8DF1,
    HIGH_FLOAT = 0x8DF2,
    LOW_INT = 0x8DF3,
    MEDIUM_INT = 0x8DF4,
    HIGH_INT = 0x8DF5,
    SHADER_COMPILER = 0x8DFA,
    SHADER_BINARY_FORMATS = 0x8DF8,
    NUM_SHADER_BINARY_FORMATS = 0x8DF9,
    MAX_VERTEX_UNIFORM_VECTORS = 0x8DFB,
    MAX_VARYING_VECTORS = 0x8DFC,
    MAX_FRAGMENT_UNIFORM_VECTORS = 0x8DFD,
    RGB565 = 0x8D62,
    PROGRAM_BINARY_RETRIEVABLE_HINT = 0x8257,
    PROGRAM_BINARY_LENGTH = 0x8741,
    NUM_PROGRAM_BINARY_FORMATS = 0x87FE,
    PROGRAM_BINARY_FORMATS = 0x87FF,
    VERTEX_SHADER_BIT = 0x00000001,
    FRAGMENT_SHADER_BIT = 0x00000002,
    GEOMETRY_SHADER_BIT = 0x00000004,
    TESS_CONTROL_SHADER_BIT = 0x00000008,
    TESS_EVALUATION_SHADER_BIT = 0x00000010,
    ALL_SHADER_BITS = 0xFFFFFFFF,
    PROGRAM_SEPARABLE = 0x8258,
    ACTIVE_PROGRAM = 0x8259,
    PROGRAM_PIPELINE_BINDING = 0x825A,
    MAX_VIEWPORTS = 0x825B,
    VIEWPORT_SUBPIXEL_BITS = 0x825C,
    VIEWPORT_BOUNDS_RANGE = 0x825D,
    LAYER_PROVOKING_VERTEX = 0x825E,
    VIEWPORT_INDEX_PROVOKING_VERTEX = 0x825F,
    UNDEFINED_VERTEX = 0x8260,
    COPY_READ_BUFFER_BINDING = 0x8F36,
    COPY_WRITE_BUFFER_BINDING = 0x8F37,
    TRANSFORM_FEEDBACK_ACTIVE = 0x8E24,
    TRANSFORM_FEEDBACK_PAUSED = 0x8E23,
    UNPACK_COMPRESSED_BLOCK_WIDTH = 0x9127,
    UNPACK_COMPRESSED_BLOCK_HEIGHT = 0x9128,
    UNPACK_COMPRESSED_BLOCK_DEPTH = 0x9129,
    UNPACK_COMPRESSED_BLOCK_SIZE = 0x912A,
    PACK_COMPRESSED_BLOCK_WIDTH = 0x912B,
    PACK_COMPRESSED_BLOCK_HEIGHT = 0x912C,
    PACK_COMPRESSED_BLOCK_DEPTH = 0x912D,
    PACK_COMPRESSED_BLOCK_SIZE = 0x912E,
    NUM_SAMPLE_COUNTS = 0x9380,
    MIN_MAP_BUFFER_ALIGNMENT = 0x90BC,
    ATOMIC_COUNTER_BUFFER = 0x92C0,
    ATOMIC_COUNTER_BUFFER_BINDING = 0x92C1,
    ATOMIC_COUNTER_BUFFER_START = 0x92C2,
    ATOMIC_COUNTER_BUFFER_SIZE = 0x92C3,
    ATOMIC_COUNTER_BUFFER_DATA_SIZE = 0x92C4,
    ATOMIC_COUNTER_BUFFER_ACTIVE_ATOMIC_COUNTERS = 0x92C5,
    ATOMIC_COUNTER_BUFFER_ACTIVE_ATOMIC_COUNTER_INDICES = 0x92C6,
    ATOMIC_COUNTER_BUFFER_REFERENCED_BY_VERTEX_SHADER = 0x92C7,
    ATOMIC_COUNTER_BUFFER_REFERENCED_BY_TESS_CONTROL_SHADER = 0x92C8,
    ATOMIC_COUNTER_BUFFER_REFERENCED_BY_TESS_EVALUATION_SHADER = 0x92C9,
    ATOMIC_COUNTER_BUFFER_REFERENCED_BY_GEOMETRY_SHADER = 0x92CA,
    ATOMIC_COUNTER_BUFFER_REFERENCED_BY_FRAGMENT_SHADER = 0x92CB,
    MAX_VERTEX_ATOMIC_COUNTER_BUFFERS = 0x92CC,
    MAX_TESS_CONTROL_ATOMIC_COUNTER_BUFFERS = 0x92CD,
    MAX_TESS_EVALUATION_ATOMIC_COUNTER_BUFFERS = 0x92CE,
    MAX_GEOMETRY_ATOMIC_COUNTER_BUFFERS = 0x92CF,
    MAX_FRAGMENT_ATOMIC_COUNTER_BUFFERS = 0x92D0,
    MAX_COMBINED_ATOMIC_COUNTER_BUFFERS = 0x92D1,
    MAX_VERTEX_ATOMIC_COUNTERS = 0x92D2,
    MAX_TESS_CONTROL_ATOMIC_COUNTERS = 0x92D3,
    MAX_TESS_EVALUATION_ATOMIC_COUNTERS = 0x92D4,
    MAX_GEOMETRY_ATOMIC_COUNTERS = 0x92D5,
    MAX_FRAGMENT_ATOMIC_COUNTERS = 0x92D6,
    MAX_COMBINED_ATOMIC_COUNTERS = 0x92D7,
    MAX_ATOMIC_COUNTER_BUFFER_SIZE = 0x92D8,
    MAX_ATOMIC_COUNTER_BUFFER_BINDINGS = 0x92DC,
    ACTIVE_ATOMIC_COUNTER_BUFFERS = 0x92D9,
    UNIFORM_ATOMIC_COUNTER_BUFFER_INDEX = 0x92DA,
    UNSIGNED_INT_ATOMIC_COUNTER = 0x92DB,
    VERTEX_ATTRIB_ARRAY_BARRIER_BIT = 0x00000001,
    ELEMENT_ARRAY_BARRIER_BIT = 0x00000002,
    UNIFORM_BARRIER_BIT = 0x00000004,
    TEXTURE_FETCH_BARRIER_BIT = 0x00000008,
    SHADER_IMAGE_ACCESS_BARRIER_BIT = 0x00000020,
    COMMAND_BARRIER_BIT = 0x00000040,
    PIXEL_BUFFER_BARRIER_BIT = 0x00000080,
    TEXTURE_UPDATE_BARRIER_BIT = 0x00000100,
    BUFFER_UPDATE_BARRIER_BIT = 0x00000200,
    FRAMEBUFFER_BARRIER_BIT = 0x00000400,
    TRANSFORM_FEEDBACK_BARRIER_BIT = 0x00000800,
    ATOMIC_COUNTER_BARRIER_BIT = 0x00001000,
    ALL_BARRIER_BITS = 0xFFFFFFFF,
    MAX_IMAGE_UNITS = 0x8F38,
    MAX_COMBINED_IMAGE_UNITS_AND_FRAGMENT_OUTPUTS = 0x8F39,
    IMAGE_BINDING_NAME = 0x8F3A,
    IMAGE_BINDING_LEVEL = 0x8F3B,
    IMAGE_BINDING_LAYERED = 0x8F3C,
    IMAGE_BINDING_LAYER = 0x8F3D,
    IMAGE_BINDING_ACCESS = 0x8F3E,
    IMAGE_1D = 0x904C,
    IMAGE_2D = 0x904D,
    IMAGE_3D = 0x904E,
    IMAGE_2D_RECT = 0x904F,
    IMAGE_CUBE = 0x9050,
    IMAGE_BUFFER = 0x9051,
    IMAGE_1D_ARRAY = 0x9052,
    IMAGE_2D_ARRAY = 0x9053,
    IMAGE_CUBE_MAP_ARRAY = 0x9054,
    IMAGE_2D_MULTISAMPLE = 0x9055,
    IMAGE_2D_MULTISAMPLE_ARRAY = 0x9056,
    INT_IMAGE_1D = 0x9057,
    INT_IMAGE_2D = 0x9058,
    INT_IMAGE_3D = 0x9059,
    INT_IMAGE_2D_RECT = 0x905A,
    INT_IMAGE_CUBE = 0x905B,
    INT_IMAGE_BUFFER = 0x905C,
    INT_IMAGE_1D_ARRAY = 0x905D,
    INT_IMAGE_2D_ARRAY = 0x905E,
    INT_IMAGE_CUBE_MAP_ARRAY = 0x905F,
    INT_IMAGE_2D_MULTISAMPLE = 0x9060,
    INT_IMAGE_2D_MULTISAMPLE_ARRAY = 0x9061,
    UNSIGNED_INT_IMAGE_1D = 0x9062,
    UNSIGNED_INT_IMAGE_2D = 0x9063,
    UNSIGNED_INT_IMAGE_3D = 0x9064,
    UNSIGNED_INT_IMAGE_2D_RECT = 0x9065,
    UNSIGNED_INT_IMAGE_CUBE = 0x9066,
    UNSIGNED_INT_IMAGE_BUFFER = 0x9067,
    UNSIGNED_INT_IMAGE_1D_ARRAY = 0x9068,
    UNSIGNED_INT_IMAGE_2D_ARRAY = 0x9069,
    UNSIGNED_INT_IMAGE_CUBE_MAP_ARRAY = 0x906A,
    UNSIGNED_INT_IMAGE_2D_MULTISAMPLE = 0x906B,
    UNSIGNED_INT_IMAGE_2D_MULTISAMPLE_ARRAY = 0x906C,
    MAX_IMAGE_SAMPLES = 0x906D,
    IMAGE_BINDING_FORMAT = 0x906E,
    IMAGE_FORMAT_COMPATIBILITY_TYPE = 0x90C7,
    IMAGE_FORMAT_COMPATIBILITY_BY_SIZE = 0x90C8,
    IMAGE_FORMAT_COMPATIBILITY_BY_CLASS = 0x90C9,
    MAX_VERTEX_IMAGE_UNIFORMS = 0x90CA,
    MAX_TESS_CONTROL_IMAGE_UNIFORMS = 0x90CB,
    MAX_TESS_EVALUATION_IMAGE_UNIFORMS = 0x90CC,
    MAX_GEOMETRY_IMAGE_UNIFORMS = 0x90CD,
    MAX_FRAGMENT_IMAGE_UNIFORMS = 0x90CE,
    MAX_COMBINED_IMAGE_UNIFORMS = 0x90CF,
    COMPRESSED_RGBA_BPTC_UNORM = 0x8E8C,
    COMPRESSED_SRGB_ALPHA_BPTC_UNORM = 0x8E8D,
    COMPRESSED_RGB_BPTC_SIGNED_FLOAT = 0x8E8E,
    COMPRESSED_RGB_BPTC_UNSIGNED_FLOAT = 0x8E8F,
    TEXTURE_IMMUTABLE_FORMAT = 0x912F,
    NUM_SHADING_LANGUAGE_VERSIONS = 0x82E9,
    VERTEX_ATTRIB_ARRAY_LONG = 0x874E,
    COMPRESSED_RGB8_ETC2 = 0x9274,
    COMPRESSED_SRGB8_ETC2 = 0x9275,
    COMPRESSED_RGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9276,
    COMPRESSED_SRGB8_PUNCHTHROUGH_ALPHA1_ETC2 = 0x9277,
    COMPRESSED_RGBA8_ETC2_EAC = 0x9278,
    COMPRESSED_SRGB8_ALPHA8_ETC2_EAC = 0x9279,
    COMPRESSED_R11_EAC = 0x9270,
    COMPRESSED_SIGNED_R11_EAC = 0x9271,
    COMPRESSED_RG11_EAC = 0x9272,
    COMPRESSED_SIGNED_RG11_EAC = 0x9273,
    PRIMITIVE_RESTART_FIXED_INDEX = 0x8D69,
    ANY_SAMPLES_PASSED_CONSERVATIVE = 0x8D6A,
    MAX_ELEMENT_INDEX = 0x8D6B,
    COMPUTE_SHADER = 0x91B9,
    MAX_COMPUTE_UNIFORM_BLOCKS = 0x91BB,
    MAX_COMPUTE_TEXTURE_IMAGE_UNITS = 0x91BC,
    MAX_COMPUTE_IMAGE_UNIFORMS = 0x91BD,
    MAX_COMPUTE_SHARED_MEMORY_SIZE = 0x8262,
    MAX_COMPUTE_UNIFORM_COMPONENTS = 0x8263,
    MAX_COMPUTE_ATOMIC_COUNTER_BUFFERS = 0x8264,
    MAX_COMPUTE_ATOMIC_COUNTERS = 0x8265,
    MAX_COMBINED_COMPUTE_UNIFORM_COMPONENTS = 0x8266,
    MAX_COMPUTE_WORK_GROUP_INVOCATIONS = 0x90EB,
    MAX_COMPUTE_WORK_GROUP_COUNT = 0x91BE,
    MAX_COMPUTE_WORK_GROUP_SIZE = 0x91BF,
    COMPUTE_WORK_GROUP_SIZE = 0x8267,
    UNIFORM_BLOCK_REFERENCED_BY_COMPUTE_SHADER = 0x90EC,
    ATOMIC_COUNTER_BUFFER_REFERENCED_BY_COMPUTE_SHADER = 0x90ED,
    DISPATCH_INDIRECT_BUFFER = 0x90EE,
    DISPATCH_INDIRECT_BUFFER_BINDING = 0x90EF,
    COMPUTE_SHADER_BIT = 0x00000020,
    DEBUG_OUTPUT_SYNCHRONOUS = 0x8242,
    DEBUG_NEXT_LOGGED_MESSAGE_LENGTH = 0x8243,
    DEBUG_CALLBACK_FUNCTION = 0x8244,
    DEBUG_CALLBACK_USER_PARAM = 0x8245,
    DEBUG_SOURCE_API = 0x8246,
    DEBUG_SOURCE_WINDOW_SYSTEM = 0x8247,
    DEBUG_SOURCE_SHADER_COMPILER = 0x8248,
    DEBUG_SOURCE_THIRD_PARTY = 0x8249,
    DEBUG_SOURCE_APPLICATION = 0x824A,
    DEBUG_SOURCE_OTHER = 0x824B,
    DEBUG_TYPE_ERROR = 0x824C,
    DEBUG_TYPE_DEPRECATED_BEHAVIOR = 0x824D,
    DEBUG_TYPE_UNDEFINED_BEHAVIOR = 0x824E,
    DEBUG_TYPE_PORTABILITY = 0x824F,
    DEBUG_TYPE_PERFORMANCE = 0x8250,
    DEBUG_TYPE_OTHER = 0x8251,
    MAX_DEBUG_MESSAGE_LENGTH = 0x9143,
    MAX_DEBUG_LOGGED_MESSAGES = 0x9144,
    DEBUG_LOGGED_MESSAGES = 0x9145,
    DEBUG_SEVERITY_HIGH = 0x9146,
    DEBUG_SEVERITY_MEDIUM = 0x9147,
    DEBUG_SEVERITY_LOW = 0x9148,
    DEBUG_TYPE_MARKER = 0x8268,
    DEBUG_TYPE_PUSH_GROUP = 0x8269,
    DEBUG_TYPE_POP_GROUP = 0x826A,
    DEBUG_SEVERITY_NOTIFICATION = 0x826B,
    MAX_DEBUG_GROUP_STACK_DEPTH = 0x826C,
    DEBUG_GROUP_STACK_DEPTH = 0x826D,
    BUFFER = 0x82E0,
    SHADER = 0x82E1,
    PROGRAM = 0x82E2,
    QUERY = 0x82E3,
    PROGRAM_PIPELINE = 0x82E4,
    SAMPLER = 0x82E6,
    MAX_LABEL_LENGTH = 0x82E8,
    DEBUG_OUTPUT = 0x92E0,
    CONTEXT_FLAG_DEBUG_BIT = 0x00000002,
    MAX_UNIFORM_LOCATIONS = 0x826E,
    FRAMEBUFFER_DEFAULT_WIDTH = 0x9310,
    FRAMEBUFFER_DEFAULT_HEIGHT = 0x9311,
    FRAMEBUFFER_DEFAULT_LAYERS = 0x9312,
    FRAMEBUFFER_DEFAULT_SAMPLES = 0x9313,
    FRAMEBUFFER_DEFAULT_FIXED_SAMPLE_LOCATIONS = 0x9314,
    MAX_FRAMEBUFFER_WIDTH = 0x9315,
    MAX_FRAMEBUFFER_HEIGHT = 0x9316,
    MAX_FRAMEBUFFER_LAYERS = 0x9317,
    MAX_FRAMEBUFFER_SAMPLES = 0x9318,
    INTERNALFORMAT_SUPPORTED = 0x826F,
    INTERNALFORMAT_PREFERRED = 0x8270,
    INTERNALFORMAT_RED_SIZE = 0x8271,
    INTERNALFORMAT_GREEN_SIZE = 0x8272,
    INTERNALFORMAT_BLUE_SIZE = 0x8273,
    INTERNALFORMAT_ALPHA_SIZE = 0x8274,
    INTERNALFORMAT_DEPTH_SIZE = 0x8275,
    INTERNALFORMAT_STENCIL_SIZE = 0x8276,
    INTERNALFORMAT_SHARED_SIZE = 0x8277,
    INTERNALFORMAT_RED_TYPE = 0x8278,
    INTERNALFORMAT_GREEN_TYPE = 0x8279,
    INTERNALFORMAT_BLUE_TYPE = 0x827A,
    INTERNALFORMAT_ALPHA_TYPE = 0x827B,
    INTERNALFORMAT_DEPTH_TYPE = 0x827C,
    INTERNALFORMAT_STENCIL_TYPE = 0x827D,
    MAX_WIDTH = 0x827E,
    MAX_HEIGHT = 0x827F,
    MAX_DEPTH = 0x8280,
    MAX_LAYERS = 0x8281,
    MAX_COMBINED_DIMENSIONS = 0x8282,
    COLOR_COMPONENTS = 0x8283,
    DEPTH_COMPONENTS = 0x8284,
    STENCIL_COMPONENTS = 0x8285,
    COLOR_RENDERABLE = 0x8286,
    DEPTH_RENDERABLE = 0x8287,
    STENCIL_RENDERABLE = 0x8288,
    FRAMEBUFFER_RENDERABLE = 0x8289,
    FRAMEBUFFER_RENDERABLE_LAYERED = 0x828A,
    FRAMEBUFFER_BLEND = 0x828B,
    READ_PIXELS = 0x828C,
    READ_PIXELS_FORMAT = 0x828D,
    READ_PIXELS_TYPE = 0x828E,
    TEXTURE_IMAGE_FORMAT = 0x828F,
    TEXTURE_IMAGE_TYPE = 0x8290,
    GET_TEXTURE_IMAGE_FORMAT = 0x8291,
    GET_TEXTURE_IMAGE_TYPE = 0x8292,
    MIPMAP = 0x8293,
    MANUAL_GENERATE_MIPMAP = 0x8294,
    AUTO_GENERATE_MIPMAP = 0x8295,
    COLOR_ENCODING = 0x8296,
    SRGB_READ = 0x8297,
    SRGB_WRITE = 0x8298,
    FILTER = 0x829A,
    VERTEX_TEXTURE = 0x829B,
    TESS_CONTROL_TEXTURE = 0x829C,
    TESS_EVALUATION_TEXTURE = 0x829D,
    GEOMETRY_TEXTURE = 0x829E,
    FRAGMENT_TEXTURE = 0x829F,
    COMPUTE_TEXTURE = 0x82A0,
    TEXTURE_SHADOW = 0x82A1,
    TEXTURE_GATHER = 0x82A2,
    TEXTURE_GATHER_SHADOW = 0x82A3,
    SHADER_IMAGE_LOAD = 0x82A4,
    SHADER_IMAGE_STORE = 0x82A5,
    SHADER_IMAGE_ATOMIC = 0x82A6,
    IMAGE_TEXEL_SIZE = 0x82A7,
    IMAGE_COMPATIBILITY_CLASS = 0x82A8,
    IMAGE_PIXEL_FORMAT = 0x82A9,
    IMAGE_PIXEL_TYPE = 0x82AA,
    SIMULTANEOUS_TEXTURE_AND_DEPTH_TEST = 0x82AC,
    SIMULTANEOUS_TEXTURE_AND_STENCIL_TEST = 0x82AD,
    SIMULTANEOUS_TEXTURE_AND_DEPTH_WRITE = 0x82AE,
    SIMULTANEOUS_TEXTURE_AND_STENCIL_WRITE = 0x82AF,
    TEXTURE_COMPRESSED_BLOCK_WIDTH = 0x82B1,
    TEXTURE_COMPRESSED_BLOCK_HEIGHT = 0x82B2,
    TEXTURE_COMPRESSED_BLOCK_SIZE = 0x82B3,
    CLEAR_BUFFER = 0x82B4,
    TEXTURE_VIEW = 0x82B5,
    VIEW_COMPATIBILITY_CLASS = 0x82B6,
    FULL_SUPPORT = 0x82B7,
    CAVEAT_SUPPORT = 0x82B8,
    IMAGE_CLASS_4_X_32 = 0x82B9,
    IMAGE_CLASS_2_X_32 = 0x82BA,
    IMAGE_CLASS_1_X_32 = 0x82BB,
    IMAGE_CLASS_4_X_16 = 0x82BC,
    IMAGE_CLASS_2_X_16 = 0x82BD,
    IMAGE_CLASS_1_X_16 = 0x82BE,
    IMAGE_CLASS_4_X_8 = 0x82BF,
    IMAGE_CLASS_2_X_8 = 0x82C0,
    IMAGE_CLASS_1_X_8 = 0x82C1,
    IMAGE_CLASS_11_11_10 = 0x82C2,
    IMAGE_CLASS_10_10_10_2 = 0x82C3,
    VIEW_CLASS_128_BITS = 0x82C4,
    VIEW_CLASS_96_BITS = 0x82C5,
    VIEW_CLASS_64_BITS = 0x82C6,
    VIEW_CLASS_48_BITS = 0x82C7,
    VIEW_CLASS_32_BITS = 0x82C8,
    VIEW_CLASS_24_BITS = 0x82C9,
    VIEW_CLASS_16_BITS = 0x82CA,
    VIEW_CLASS_8_BITS = 0x82CB,
    VIEW_CLASS_S3TC_DXT1_RGB = 0x82CC,
    VIEW_CLASS_S3TC_DXT1_RGBA = 0x82CD,
    VIEW_CLASS_S3TC_DXT3_RGBA = 0x82CE,
    VIEW_CLASS_S3TC_DXT5_RGBA = 0x82CF,
    VIEW_CLASS_RGTC1_RED = 0x82D0,
    VIEW_CLASS_RGTC2_RG = 0x82D1,
    VIEW_CLASS_BPTC_UNORM = 0x82D2,
    VIEW_CLASS_BPTC_FLOAT = 0x82D3,
    UNIFORM = 0x92E1,
    UNIFORM_BLOCK = 0x92E2,
    PROGRAM_INPUT = 0x92E3,
    PROGRAM_OUTPUT = 0x92E4,
    BUFFER_VARIABLE = 0x92E5,
    SHADER_STORAGE_BLOCK = 0x92E6,
    VERTEX_SUBROUTINE = 0x92E8,
    TESS_CONTROL_SUBROUTINE = 0x92E9,
    TESS_EVALUATION_SUBROUTINE = 0x92EA,
    GEOMETRY_SUBROUTINE = 0x92EB,
    FRAGMENT_SUBROUTINE = 0x92EC,
    COMPUTE_SUBROUTINE = 0x92ED,
    VERTEX_SUBROUTINE_UNIFORM = 0x92EE,
    TESS_CONTROL_SUBROUTINE_UNIFORM = 0x92EF,
    TESS_EVALUATION_SUBROUTINE_UNIFORM = 0x92F0,
    GEOMETRY_SUBROUTINE_UNIFORM = 0x92F1,
    FRAGMENT_SUBROUTINE_UNIFORM = 0x92F2,
    COMPUTE_SUBROUTINE_UNIFORM = 0x92F3,
    TRANSFORM_FEEDBACK_VARYING = 0x92F4,
    ACTIVE_RESOURCES = 0x92F5,
    MAX_NAME_LENGTH = 0x92F6,
    MAX_NUM_ACTIVE_VARIABLES = 0x92F7,
    MAX_NUM_COMPATIBLE_SUBROUTINES = 0x92F8,
    NAME_LENGTH = 0x92F9,
    TYPE = 0x92FA,
    ARRAY_SIZE = 0x92FB,
    OFFSET = 0x92FC,
    BLOCK_INDEX = 0x92FD,
    ARRAY_STRIDE = 0x92FE,
    MATRIX_STRIDE = 0x92FF,
    IS_ROW_MAJOR = 0x9300,
    ATOMIC_COUNTER_BUFFER_INDEX = 0x9301,
    BUFFER_BINDING = 0x9302,
    BUFFER_DATA_SIZE = 0x9303,
    NUM_ACTIVE_VARIABLES = 0x9304,
    ACTIVE_VARIABLES = 0x9305,
    REFERENCED_BY_VERTEX_SHADER = 0x9306,
    REFERENCED_BY_TESS_CONTROL_SHADER = 0x9307,
    REFERENCED_BY_TESS_EVALUATION_SHADER = 0x9308,
    REFERENCED_BY_GEOMETRY_SHADER = 0x9309,
    REFERENCED_BY_FRAGMENT_SHADER = 0x930A,
    REFERENCED_BY_COMPUTE_SHADER = 0x930B,
    TOP_LEVEL_ARRAY_SIZE = 0x930C,
    TOP_LEVEL_ARRAY_STRIDE = 0x930D,
    LOCATION = 0x930E,
    LOCATION_INDEX = 0x930F,
    IS_PER_PATCH = 0x92E7,
    SHADER_STORAGE_BUFFER = 0x90D2,
    SHADER_STORAGE_BUFFER_BINDING = 0x90D3,
    SHADER_STORAGE_BUFFER_START = 0x90D4,
    SHADER_STORAGE_BUFFER_SIZE = 0x90D5,
    MAX_VERTEX_SHADER_STORAGE_BLOCKS = 0x90D6,
    MAX_GEOMETRY_SHADER_STORAGE_BLOCKS = 0x90D7,
    MAX_TESS_CONTROL_SHADER_STORAGE_BLOCKS = 0x90D8,
    MAX_TESS_EVALUATION_SHADER_STORAGE_BLOCKS = 0x90D9,
    MAX_FRAGMENT_SHADER_STORAGE_BLOCKS = 0x90DA,
    MAX_COMPUTE_SHADER_STORAGE_BLOCKS = 0x90DB,
    MAX_COMBINED_SHADER_STORAGE_BLOCKS = 0x90DC,
    MAX_SHADER_STORAGE_BUFFER_BINDINGS = 0x90DD,
    MAX_SHADER_STORAGE_BLOCK_SIZE = 0x90DE,
    SHADER_STORAGE_BUFFER_OFFSET_ALIGNMENT = 0x90DF,
    SHADER_STORAGE_BARRIER_BIT = 0x00002000,
    MAX_COMBINED_SHADER_OUTPUT_RESOURCES = 0x8F39,
    DEPTH_STENCIL_TEXTURE_MODE = 0x90EA,
    TEXTURE_BUFFER_OFFSET = 0x919D,
    TEXTURE_BUFFER_SIZE = 0x919E,
    TEXTURE_BUFFER_OFFSET_ALIGNMENT = 0x919F,
    TEXTURE_VIEW_MIN_LEVEL = 0x82DB,
    TEXTURE_VIEW_NUM_LEVELS = 0x82DC,
    TEXTURE_VIEW_MIN_LAYER = 0x82DD,
    TEXTURE_VIEW_NUM_LAYERS = 0x82DE,
    TEXTURE_IMMUTABLE_LEVELS = 0x82DF,
    VERTEX_ATTRIB_BINDING = 0x82D4,
    VERTEX_ATTRIB_RELATIVE_OFFSET = 0x82D5,
    VERTEX_BINDING_DIVISOR = 0x82D6,
    VERTEX_BINDING_OFFSET = 0x82D7,
    VERTEX_BINDING_STRIDE = 0x82D8,
    MAX_VERTEX_ATTRIB_RELATIVE_OFFSET = 0x82D9,
    MAX_VERTEX_ATTRIB_BINDINGS = 0x82DA,
    VERTEX_BINDING_BUFFER = 0x8F4F,
    MAX_VERTEX_ATTRIB_STRIDE = 0x82E5,
    PRIMITIVE_RESTART_FOR_PATCHES_SUPPORTED = 0x8221,
    TEXTURE_BUFFER_BINDING = 0x8C2A,
    MAP_PERSISTENT_BIT = 0x0040,
    MAP_COHERENT_BIT = 0x0080,
    DYNAMIC_STORAGE_BIT = 0x0100,
    CLIENT_STORAGE_BIT = 0x0200,
    CLIENT_MAPPED_BUFFER_BARRIER_BIT = 0x00004000,
    BUFFER_IMMUTABLE_STORAGE = 0x821F,
    BUFFER_STORAGE_FLAGS = 0x8220,
    CLEAR_TEXTURE = 0x9365,
    LOCATION_COMPONENT = 0x934A,
    TRANSFORM_FEEDBACK_BUFFER_INDEX = 0x934B,
    TRANSFORM_FEEDBACK_BUFFER_STRIDE = 0x934C,
    QUERY_BUFFER = 0x9192,
    QUERY_BUFFER_BARRIER_BIT = 0x00008000,
    QUERY_BUFFER_BINDING = 0x9193,
    QUERY_RESULT_NO_WAIT = 0x9194,
    MIRROR_CLAMP_TO_EDGE = 0x8743,
    CONTEXT_LOST = 0x0507,
    NEGATIVE_ONE_TO_ONE = 0x935E,
    ZERO_TO_ONE = 0x935F,
    CLIP_ORIGIN = 0x935C,
    CLIP_DEPTH_MODE = 0x935D,
    QUERY_WAIT_INVERTED = 0x8E17,
    QUERY_NO_WAIT_INVERTED = 0x8E18,
    QUERY_BY_REGION_WAIT_INVERTED = 0x8E19,
    QUERY_BY_REGION_NO_WAIT_INVERTED = 0x8E1A,
    MAX_CULL_DISTANCES = 0x82F9,
    MAX_COMBINED_CLIP_AND_CULL_DISTANCES = 0x82FA,
    TEXTURE_TARGET = 0x1006,
    QUERY_TARGET = 0x82EA,
    GUILTY_CONTEXT_RESET = 0x8253,
    INNOCENT_CONTEXT_RESET = 0x8254,
    UNKNOWN_CONTEXT_RESET = 0x8255,
    RESET_NOTIFICATION_STRATEGY = 0x8256,
    LOSE_CONTEXT_ON_RESET = 0x8252,
    NO_RESET_NOTIFICATION = 0x8261,
    CONTEXT_FLAG_ROBUST_ACCESS_BIT = 0x00000004,
    CONTEXT_RELEASE_BEHAVIOR = 0x82FB,
    CONTEXT_RELEASE_BEHAVIOR_FLUSH = 0x82FC,
    SHADER_BINARY_FORMAT_SPIR_V = 0x9551,
    SPIR_V_BINARY = 0x9552,
    PARAMETER_BUFFER = 0x80EE,
    PARAMETER_BUFFER_BINDING = 0x80EF,
    CONTEXT_FLAG_NO_ERROR_BIT = 0x00000008,
    VERTICES_SUBMITTED = 0x82EE,
    PRIMITIVES_SUBMITTED = 0x82EF,
    VERTEX_SHADER_INVOCATIONS = 0x82F0,
    TESS_CONTROL_SHADER_PATCHES = 0x82F1,
    TESS_EVALUATION_SHADER_INVOCATIONS = 0x82F2,
    GEOMETRY_SHADER_PRIMITIVES_EMITTED = 0x82F3,
    FRAGMENT_SHADER_INVOCATIONS = 0x82F4,
    COMPUTE_SHADER_INVOCATIONS = 0x82F5,
    CLIPPING_INPUT_PRIMITIVES = 0x82F6,
    CLIPPING_OUTPUT_PRIMITIVES = 0x82F7,
    POLYGON_OFFSET_CLAMP = 0x8E1B,
    SPIR_V_EXTENSIONS = 0x9553,
    NUM_SPIR_V_EXTENSIONS = 0x9554,
    TEXTURE_MAX_ANISOTROPY = 0x84FE,
    MAX_TEXTURE_MAX_ANISOTROPY = 0x84FF,
    TRANSFORM_FEEDBACK_OVERFLOW = 0x82EC,
    TRANSFORM_FEEDBACK_STREAM_OVERFLOW = 0x82ED,
}