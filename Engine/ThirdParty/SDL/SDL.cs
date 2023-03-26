using System.Runtime.InteropServices;

namespace ThirdParty.SDL;

public unsafe delegate void SDL_AudioCallback(void* userdata, byte* stream, int length);

public unsafe delegate void* SDL_Malloc(ulong size);
public unsafe delegate void* SDL_Realloc(void* mem, ulong size);
public unsafe delegate void SDL_Free(void* mem);

public unsafe delegate void* Malloc(ulong size);
public unsafe delegate void Free(void* ptr);
public unsafe delegate void* Realloc(void* ptr, ulong new_size);

public unsafe class SDLMemoryOps
{

    private Malloc _malloc;
    private Free _free;
    private Realloc _realloc;

    public SDLMemoryOps()
    {
        void* malloc, free, realloc, calloc;
        SDL.SDL_GetMemoryFunctions(&malloc, &calloc, &realloc, &free);

        _malloc = Marshal.GetDelegateForFunctionPointer<Malloc>(new IntPtr(malloc));
        _free = Marshal.GetDelegateForFunctionPointer<Free>(new IntPtr(free));
        _realloc = Marshal.GetDelegateForFunctionPointer<Realloc>(new IntPtr(realloc));
    }

    public void Free(void* ptr)
    {
        _free(ptr);
    }

    public void* Malloc(ulong size)
    {
        return _malloc(size);
    }

    public void* Realloc(void* ptr, ulong new_size)
    {
        return _realloc(ptr, new_size);
    }
}

public unsafe static class SDL
{

    const string dll_name = "Resources/SDL2.dll";

    public static uint SDL_WINDOWPOS_CENTERED = 0x2FFF0000u;

    static SDL()
    {
        int res = SDL_Init(SDL_InitFlags.SDL_INIT_EVERYTHING);

        if (res != 0)
        {
            SDL_PrintError();
        }
    }
    
    public static byte[] ToCStr(this string str)
    {
        return System.Text.Encoding.ASCII.GetBytes(str + "\0");
    }

    public static string CStrToString(byte* bytes, int length)
    {
        return System.Text.Encoding.ASCII.GetString(bytes, length);
    }

    public static int CStrLen(byte* bytes)
    {
        int idx = 0;
        while (*(bytes + idx) != '\0') idx++;
        return idx;
    }

    public static string CStrToString(byte* bytes)
    {
        return CStrToString(bytes, CStrLen(bytes));
    }

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GetMemoryFunctions(void** malloc_func, void** calloc_func, void** realloc_func, void** free_func);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_RWops* SDL_memset(void* ptr, int value, ulong size);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_RWops* SDL_RWFromMem(void* mem, int size);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_AudioSpec* SDL_LoadWAV_RW(SDL_RWops* src, int freesrc, SDL_AudioSpec* spec, byte** audio_buf, uint* audio_len);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_AudioSpec* SDL_LoadWAV([MarshalAs(UnmanagedType.LPStr)] string file, SDL_AudioSpec* spec, byte** audio_buf, uint* audio_len);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_Init(SDL_InitFlags flags);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_CreateWindow([MarshalAs(UnmanagedType.LPStr)] string title, uint x, uint y, int w, int h, SDL_WindowFlags flags);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool SDL_PollEvent(ref SDL_Event @event);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SDL_GL_CreateContext(IntPtr window_handle);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GL_SwapWindow(IntPtr window_handle);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MaximizeWindow(IntPtr window_handle);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_WindowFlags SDL_GetWindowFlags(IntPtr window_handle);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowMinimumSize(IntPtr window_handle, int min_w, int min_h);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_Surface* SDL_GetWindowSurface(IntPtr window_handle);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_UpdateWindowSurface(IntPtr window_handle);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void* SDL_GL_GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string proc_name);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_SetAttribute(SDL_GL_Attr attr, int value);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_GetAttribute(SDL_GL_Attr attr, ref int value);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_SetAttribute(SDL_GL_Attr attr, SDL_GL_Profile value);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_GetAttribute(SDL_GL_Attr attr, ref SDL_GL_Profile value);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_SetAttribute(SDL_GL_Attr attr, SDL_GL_ContextFlag value);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_GetAttribute(SDL_GL_Attr attr, ref SDL_GL_ContextFlag value);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern SDL_MouseInput SDL_GetMouseState(ref int x, ref int y);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong SDL_GetPerformanceCounter();

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern ulong SDL_GetPerformanceFrequency();

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowTitle(IntPtr window, [MarshalAs(UnmanagedType.LPStr)] string title);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* SDL_GetWindowTitle(IntPtr window);

    public static string SDL_GetWindowTitleManaged(IntPtr window)
    {
        var ptr = SDL_GetWindowTitle(window);
        string str = CStrToString(ptr);
        SDL_free(ptr);
        return str;
    }

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_GetWindowSize(IntPtr window, ref int width, ref int height);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_SetWindowSize(IntPtr window, int width, int height);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool SDL_ShowCursor(bool toggle);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_StartTextInput();

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_StopTextInput();

    public const string SDL_HINT_RENDER_SCALE_QUALITY = "SDL_RENDER_SCALE_QUALITY";

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool SDL_SetHint([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string value);

    static uint SDLK_SCANCODE_MASK = 1 << 30;

    public static uint SDL_SCANCODE_TO_KEYCODE(SDL_SCANCODE scan_code)
    {
        return (uint)scan_code | SDLK_SCANCODE_MASK;
    }


    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GL_SetSwapInterval(SwapInterval interval);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_OpenAudio(ref SDL_AudioSpec desired, SDL_AudioSpec* obtained);

    public static delegate* unmanaged[Cdecl]<void*, byte*, int> SDL_CreateAudioCallbackFucntionPoiter(SDL_AudioCallback callback)
    {
        var ptr = Marshal.GetFunctionPointerForDelegate(callback);
        return (delegate* unmanaged[Cdecl]<void*, byte*, int>)ptr.ToPointer();
    }

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_PauseAudio(bool pause_on);


    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* SDL_GetKeyboardState(int* numkeys);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern byte* SDL_GetError();

    // only SDL_WINDOW_FULLSCREEN, SDL_WINDOW_FULLSCREEN_DESKTOP or 0 works
    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool SDL_SetWindowFullscreen(IntPtr window, SDL_WindowFlags flags);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetDisplayDPI(int displayIndex, float* ddpi, float* hdpi, float* vdpi);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetWindowDisplayIndex(IntPtr window);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetNumDisplayModes(int displayIndex);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetDisplayMode(int displayIndex, int modeIndex, SDL_DisplayMode* mode);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_GetCurrentDisplayMode(int displayIndex, SDL_DisplayMode* mode);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern bool SDL_HasClipboardText();

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern char* SDL_GetClipboardText();

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern int SDL_SetClipboardText([MarshalAs(UnmanagedType.LPStr)] string text);

    [DllImport(dll_name, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_free(void* ptr);

    public static string SDL_GetErrorString()
    {
        return CStrToString(SDL_GetError());
    }

    public static void SDL_PrintError()
    {
        Console.WriteLine(SDL_GetErrorString());
    }

    public static string SDL_GetClipboardString()
    {
        char* text = SDL_GetClipboardText();
        var str = Marshal.PtrToStringAnsi(new IntPtr(text));
        SDL_free(text);
        return str;
    }

}


[StructLayout(LayoutKind.Sequential)]
public unsafe struct SDL_RWops
{
    public delegate* unmanaged[Cdecl]<SDL_RWops*, long> size;
    public delegate* unmanaged[Cdecl]<SDL_RWops*, long, int, long> seek;
    public delegate* unmanaged[Cdecl]<SDL_RWops*, void*, long, long, long> read;
    public delegate* unmanaged[Cdecl]<SDL_RWops*, void*, long, long, long> write;
    public delegate* unmanaged[Cdecl]<SDL_RWops*, int> close;
    public uint type;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SDL_DisplayMode
{
    public uint format;
    public int width;
    public int height;
    public int refresh_rate;
    public void* driver_data;
}

[Flags]
public enum SDL_MouseInput
{
    NONE = 0,
    LEFT = 1 << 0,
    MIDDLE = 1 << 1,
    RIGHT = 1 << 2,
}

public enum SDL_AudioFormat : ushort
{
    AUDIO_U8 = 0x0008, /**< Unsigned 8-bit samples */
    AUDIO_S8 = 0x8008, /**< Signed 8-bit samples */
    AUDIO_U16LSB = 0x0010,  /**< Unsigned 16-bit samples */
    AUDIO_S16LSB = 0x8010, /**< Signed 16-bit samples */
    AUDIO_U16MSB = 0x1010,  /**< As above, but big-endian byte order */
    AUDIO_S16MSB = 0x9010, /**< As above, but big-endian byte order */
    AUDIO_U16 = AUDIO_U16LSB,
    AUDIO_S16 = AUDIO_S16LSB,

    AUDIO_S32LSB = 0x8020, /**< 32-bit integer samples */
    AUDIO_S32MSB = 0x9020,  /**< As above, but big-endian byte order */
    AUDIO_S32 = AUDIO_S32LSB,

    AUDIO_F32LSB = 0x8120,  /**< 32-bit floating point samples */
    AUDIO_F32MSB = 0x9120,  /**< As above, but big-endian byte order */
    AUDIO_F32 = AUDIO_F32LSB,
}


[StructLayout(LayoutKind.Sequential)]
public unsafe struct SDL_AudioSpec
{
    public int freq;                   /**< DSP frequency -- samples per second */
    public SDL_AudioFormat format;     /**< Audio data format */
    public byte channels;             /**< Number of channels: 1 mono, 2 stereo */
    public byte silence;              /**< Audio buffer silence value (calculated) */
    public ushort samples;             /**< Audio buffer size in sample FRAMES (total samples divided by channel count) */
    public uint size;                /**< Audio buffer size in bytes (calculated) */
    public delegate* unmanaged[Cdecl]<void*, byte*, int> callback; /**< Callback that feeds the audio device (NULL to use SDL_QueueAudio()). */
    public void* userdata;             /**< Userdata passed to callback (ignored for NULL callbacks). */
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SDL_Surface
{
    public uint flags;               /**< Read-only */
    public SDL_PixelFormat* format;    /**< Read-only */
    public int w, h;                   /**< Read-only */
    public int pitch;                  /**< Read-only */
    public void* pixels;               /**< Read-write */

    /** Application data associated with the surface */
    public void* userdata;             /**< Read-write */

    /** information needed for surfaces requiring locks */
    public int locked;                 /**< Read-only */

    /** list of BlitMap that hold a reference to this surface */
    void* list_blitmap;         /**< Private */

    /** clipping information */
    public SDL_Rect clip_rect;         /**< Read-only */

    /** info for fast blit mapping to other surfaces */
    void* map;    /**< Private*/

    /** Reference count -- used when freeing surface */
    public int refcount;               /**< Read-mostly */
}

public struct SDL_Rect
{
    public int x, y;
    public int w, h;
}

public enum SwapInterval
{
    NONE = 0,
    VSYNC = 1,
    ADAPTIVE_VSYNC = -1
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SDL_PixelFormat
{
    public uint format;
    public SDL_Palette* palette;
    public byte BitsPerPixel;
    public byte BytesPerPixel;
    public fixed byte padding[2];
    public uint Rmask;
    public uint Gmask;
    public uint Bmask;
    public uint Amask;
    public byte Rloss;
    public byte Gloss;
    public byte Bloss;
    public byte Aloss;
    public byte Rshift;
    public byte Gshift;
    public byte Bshift;
    public byte Ashift;
    public int refcount;
    public SDL_PixelFormat* next;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SDL_Palette
{
    public int ncolors;
    public SDL_Color* colors;
    public uint version;
    public int refcount;
}

[StructLayout(LayoutKind.Sequential)]
public struct SDL_Color
{
    public byte r;
    public byte g;
    public byte b;
    public byte a;
}
public enum SDL_InitFlags : uint
{
    SDL_INIT_TIMER = 0x00000001u,
    SDL_INIT_AUDIO = 0x00000010u,
    SDL_INIT_VIDEO = 0x00000020u,  /**< SDL_INIT_VIDEO implies SDL_INIT_EVENTS */
    SDL_INIT_JOYSTICK = 0x00000200u,  /**< SDL_INIT_JOYSTICK implies SDL_INIT_EVENTS */
    SDL_INIT_HAPTIC = 0x00001000u,
    SDL_INIT_GAMECONTROLLER = 0x00002000u,  /**< SDL_INIT_GAMECONTROLLER implies SDL_INIT_JOYSTICK */
    SDL_INIT_EVENTS = 0x00004000u,
    SDL_INIT_SENSOR = 0x00008000u,
    SDL_INIT_NOPARACHUTE = 0x00100000u,  /**< compatibility; this flag is ignored. */
    SDL_INIT_EVERYTHING = SDL_INIT_TIMER | SDL_INIT_AUDIO | SDL_INIT_VIDEO | SDL_INIT_EVENTS |
                          SDL_INIT_JOYSTICK | SDL_INIT_HAPTIC | SDL_INIT_GAMECONTROLLER | SDL_INIT_SENSOR
}

public enum SDL_WindowFlags : uint
{
    WINDOW_NONE = 0x00000000, /** for SDL_SetWindowFullscreen */
    WINDOW_FULLSCREEN = 0x00000001,         /**< fullscreen window */
    WINDOW_OPENGL = 0x00000002,             /**< window usable with OpenGL context */
    WINDOW_SHOWN = 0x00000004,              /**< window is visible */
    WINDOW_HIDDEN = 0x00000008,             /**< window is not visible */
    WINDOW_BORDERLESS = 0x00000010,         /**< no window decoration */
    WINDOW_RESIZABLE = 0x00000020,          /**< window can be resized */
    WINDOW_MINIMIZED = 0x00000040,          /**< window is minimized */
    WINDOW_MAXIMIZED = 0x00000080,          /**< window is maximized */
    WINDOW_MOUSE_GRABBED = 0x00000100,      /**< window has grabbed mouse input */
    WINDOW_INPUT_FOCUS = 0x00000200,        /**< window has input focus */
    WINDOW_MOUSE_FOCUS = 0x00000400,        /**< window has mouse focus */
    WINDOW_FULLSCREEN_DESKTOP = (WINDOW_FULLSCREEN | 0x00001000),
    WINDOW_FOREIGN = 0x00000800,            /**< window not created by SDL */
    WINDOW_ALLOW_HIGHDPI = 0x00002000,      /**< window should be created in high-DPI mode if supported.
                                                     On macOS NSHighResolutionCapable must be set true in the
                                                     application's Info.plist for this to have any effect. */
    WINDOW_MOUSE_CAPTURE = 0x00004000,   /**< window has mouse captured (unrelated to MOUSE_GRABBED) */
    WINDOW_ALWAYS_ON_TOP = 0x00008000,   /**< window should always be above others */
    WINDOW_SKIP_TASKBAR = 0x00010000,   /**< window should not be added to the taskbar */
    WINDOW_UTILITY = 0x00020000,   /**< window should be treated as a utility window */
    WINDOW_TOOLTIP = 0x00040000,   /**< window should be treated as a tooltip */
    WINDOW_POPUP_MENU = 0x00080000,   /**< window should be treated as a popup menu */
    WINDOW_KEYBOARD_GRABBED = 0x00100000,   /**< window has grabbed keyboard input */
    WINDOW_VULKAN = 0x10000000,   /**< window usable for Vulkan surface */
    WINDOW_METAL = 0x20000000,   /**< window usable for Metal view */

    WINDOW_INPUT_GRABBED = WINDOW_MOUSE_GRABBED /**< equivalent to WINDOW_MOUSE_GRABBED for compatibility */
}

[StructLayout(LayoutKind.Explicit)]
public unsafe struct SDL_Event
{
    [FieldOffset(0)] public SDL_EventType type;                     /**< Event type, shared with all events */
    [FieldOffset(0)] public SDL_CommonEvent common;                 /**< Common event data */
    [FieldOffset(0)] public SDL_DisplayEvent display;               /**< Display event data */
    [FieldOffset(0)] public SDL_WindowEvent window;                 /**< Window event data */
    [FieldOffset(0)] public SDL_KeyboardEvent key;                  /**< Keyboard event data */
    //public SDL_TextEditingEvent edit;              /**< Text editing event data */
    [FieldOffset(0)] public SDL_TextInputEvent text;                /**< Text input event data */
    [FieldOffset(0)] public SDL_MouseMotionEvent motion;            /**< Mouse motion event data */
    //public SDL_MouseButtonEvent button;            /**< Mouse button event data */
    [FieldOffset(0)] public SDL_MouseWheelEvent wheel;              /**< Mouse wheel event data */
    //public SDL_JoyAxisEvent jaxis;                 /**< Joystick axis event data */
    //public SDL_JoyBallEvent jball;                 /**< Joystick ball event data */
    //public SDL_JoyHatEvent jhat;                   /**< Joystick hat event data */
    //public SDL_JoyButtonEvent jbutton;             /**< Joystick button event data */
    //public SDL_JoyDeviceEvent jdevice;             /**< Joystick device change event data */
    //public SDL_ControllerAxisEvent caxis;          /**< Game Controller axis event data */
    //public SDL_ControllerButtonEvent cbutton;      /**< Game Controller button event data */
    //public SDL_ControllerDeviceEvent cdevice;      /**< Game Controller device event data */
    //public SDL_ControllerTouchpadEvent ctouchpad;  /**< Game Controller touchpad event data */
    //public SDL_ControllerSensorEvent csensor;      /**< Game Controller sensor event data */
    //public SDL_AudioDeviceEvent adevice;           /**< Audio device event data */
    //public SDL_SensorEvent sensor;                 /**< Sensor event data */
    //public SDL_QuitEvent quit;                     /**< Quit request event data */
    //public SDL_UserEvent user;                     /**< Custom event data */
    //public SDL_SysWMEvent syswm;                   /**< System dependent window event data */
    //public SDL_TouchFingerEvent tfinger;           /**< Touch finger event data */
    //public SDL_MultiGestureEvent mgesture;         /**< Gesture event data */
    //public SDL_DollarGestureEvent dgesture;        /**< Gesture event data */
    //public SDL_DropEvent drop;                     /**< Drag and drop event data */

    /* This is necessary for ABI compatibility between Visual C++ and GCC.
       Visual C++ will respect the push pack pragma and use 52 bytes (size of
       SDL_TextEditingEvent, the largest structure for 32-bit and 64-bit
       architectures) for this union, and GCC will use the alignment of the
       largest datatype within the union, which is 8 bytes on 64-bit
       architectures.

       So... we'll add padding to force the size to be 56 bytes for both.

       On architectures where pointers are 16 bytes, this needs rounding up to
       the next multiple of 16, 64, and on architectures where pointers are
       even larger the size of SDL_UserEvent will dominate as being 3 pointers.
    */
    [FieldOffset(0)] fixed byte padding[56];
}

[StructLayout(LayoutKind.Sequential)]
public struct SDL_MouseMotionEvent
{
    public SDL_EventType type;        /**< ::SDL_MOUSEMOTION */
    public uint timestamp;   /**< In milliseconds, populated using SDL_GetTicks() */
    public uint windowID;    /**< The window with mouse focus, if any */
    public uint which;       /**< The mouse instance id, or SDL_TOUCH_MOUSEID */
    public uint state;       /**< The current button state */
    public int x;           /**< X coordinate, relative to window */
    public int y;           /**< Y coordinate, relative to window */
    public int xrel;        /**< The relative motion in the X direction */
    public int yrel;        /**< The relative motion in the Y direction */
}

[StructLayout(LayoutKind.Sequential)]
public struct SDL_MouseWheelEvent
{
    public uint type;        /**< ::SDL_MOUSEWHEEL */
    public uint timestamp;   /**< In milliseconds, populated using SDL_GetTicks() */
    public uint windowID;    /**< The window with mouse focus, if any */
    public uint which;       /**< The mouse instance id, or SDL_TOUCH_MOUSEID */
    public int x;            /**< The amount scrolled horizontally, positive to the right and negative to the left */
    public int y;            /**< The amount scrolled vertically, positive away from the user and negative toward the user */
    public uint direction;   /**< Set to one of the SDL_MOUSEWHEEL_* defines. When FLIPPED the values in X and Y will be opposite. Multiply by -1 to change them back */
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct SDL_TextInputEvent
{
    public uint type;
    public uint timestamp;
    public uint windowID;
    public fixed byte text[32];
}

public struct SDL_WindowEvent
{
    public uint type;        /**< ::SDL_WINDOWEVENT */
    public uint timestamp;   /**< In milliseconds, populated using SDL_GetTicks() */
    public uint windowID;    /**< The associated window */
    public SDL_WindowEventID event_id;        /**< ::SDL_WindowEventID */
    public byte padding1;
    public byte padding2;
    public byte padding3;
    public int data1;       /**< event dependent data */
    public int data2;       /**< event dependent data */
}

public enum SDL_WindowEventID : byte
{
    SDL_WINDOWEVENT_NONE,           /**< Never used */
    SDL_WINDOWEVENT_SHOWN,          /**< Window has been shown */
    SDL_WINDOWEVENT_HIDDEN,         /**< Window has been hidden */
    SDL_WINDOWEVENT_EXPOSED,        /**< Window has been exposed and should be
                                         redrawn */
    SDL_WINDOWEVENT_MOVED,          /**< Window has been moved to data1, data2
                                     */
    SDL_WINDOWEVENT_RESIZED,        /**< Window has been resized to data1xdata2 */
    SDL_WINDOWEVENT_SIZE_CHANGED,   /**< The window size has changed, either as
                                         a result of an API call or through the
                                         system or user changing the window size. */
    SDL_WINDOWEVENT_MINIMIZED,      /**< Window has been minimized */
    SDL_WINDOWEVENT_MAXIMIZED,      /**< Window has been maximized */
    SDL_WINDOWEVENT_RESTORED,       /**< Window has been restored to normal size
                                         and position */
    SDL_WINDOWEVENT_ENTER,          /**< Window has gained mouse focus */
    SDL_WINDOWEVENT_LEAVE,          /**< Window has lost mouse focus */
    SDL_WINDOWEVENT_FOCUS_GAINED,   /**< Window has gained keyboard focus */
    SDL_WINDOWEVENT_FOCUS_LOST,     /**< Window has lost keyboard focus */
    SDL_WINDOWEVENT_CLOSE,          /**< The window manager requests that the window be closed */
    SDL_WINDOWEVENT_TAKE_FOCUS,     /**< Window is being offered a focus (should SetWindowInputFocus() on itself or a subwindow, or ignore) */
    SDL_WINDOWEVENT_HIT_TEST        /**< Window had a hit test that wasn't SDL_HITTEST_NORMAL. */
}

public struct SDL_DisplayEvent
{
    public uint type;        /**< ::SDL_DISPLAYEVENT */
    public uint timestamp;   /**< In milliseconds, populated using SDL_GetTicks() */
    public uint display;     /**< The associated display index */
    public byte @event;        /**< ::SDL_DisplayEventID */
    public byte padding1;
    public byte padding2;
    public byte padding3;
    public int data1;       /**< event dependent data */
}

public enum SDL_EventType : uint
{
    FIRSTEVENT = 0,     /**< Unused (do not remove) */

    /* Application events */
    QUIT = 0x100, /**< User-requested quit */

    /* These application events have special meaning on iOS, see README-ios.md for details */
    APP_TERMINATING,        /**< The application is being terminated by the OS
                                     Called on iOS in applicationWillTerminate()
                                     Called on Android in onDestroy()
                                */
    APP_LOWMEMORY,          /**< The application is low on memory, free memory if possible.
                                     Called on iOS in applicationDidReceiveMemoryWarning()
                                     Called on Android in onLowMemory()
                                */
    APP_WILLENTERBACKGROUND, /**< The application is about to enter the background
                                     Called on iOS in applicationWillResignActive()
                                     Called on Android in onPause()
                                */
    APP_DIDENTERBACKGROUND, /**< The application did enter the background and may not get CPU for some time
                                     Called on iOS in applicationDidEnterBackground()
                                     Called on Android in onPause()
                                */
    APP_WILLENTERFOREGROUND, /**< The application is about to enter the foreground
                                     Called on iOS in applicationWillEnterForeground()
                                     Called on Android in onResume()
                                */
    APP_DIDENTERFOREGROUND, /**< The application is now interactive
                                     Called on iOS in applicationDidBecomeActive()
                                     Called on Android in onResume()
                                */

    LOCALECHANGED,  /**< The user's locale preferences have changed. */

    /* Display events */
    DISPLAYEVENT = 0x150,  /**< Display state change */

    /* Window events */
    WINDOWEVENT = 0x200, /**< Window state change */
    SYSWMEVENT,             /**< System specific event */

    /* Keyboard events */
    KEYDOWN = 0x300, /**< Key pressed */
    KEYUP,                  /**< Key released */
    TEXTEDITING,            /**< Keyboard text editing (composition) */
    TEXTINPUT,              /**< Keyboard text input */
    KEYMAPCHANGED,          /**< Keymap changed due to a system event such as an
                                     input language or keyboard layout change.
                                */

    /* Mouse events */
    MOUSEMOTION = 0x400, /**< Mouse moved */
    MOUSEBUTTONDOWN,        /**< Mouse button pressed */
    MOUSEBUTTONUP,          /**< Mouse button released */
    MOUSEWHEEL,             /**< Mouse wheel motion */

    /* Joystick events */
    JOYAXISMOTION = 0x600, /**< Joystick axis motion */
    JOYBALLMOTION,          /**< Joystick trackball motion */
    JOYHATMOTION,           /**< Joystick hat position change */
    JOYBUTTONDOWN,          /**< Joystick button pressed */
    JOYBUTTONUP,            /**< Joystick button released */
    JOYDEVICEADDED,         /**< A new joystick has been inserted into the system */
    JOYDEVICEREMOVED,       /**< An opened joystick has been removed */

    /* Game controller events */
    CONTROLLERAXISMOTION = 0x650, /**< Game controller axis motion */
    CONTROLLERBUTTONDOWN,          /**< Game controller button pressed */
    CONTROLLERBUTTONUP,            /**< Game controller button released */
    CONTROLLERDEVICEADDED,         /**< A new Game controller has been inserted into the system */
    CONTROLLERDEVICEREMOVED,       /**< An opened Game controller has been removed */
    CONTROLLERDEVICEREMAPPED,      /**< The controller mapping was updated */
    CONTROLLERTOUCHPADDOWN,        /**< Game controller touchpad was touched */
    CONTROLLERTOUCHPADMOTION,      /**< Game controller touchpad finger was moved */
    CONTROLLERTOUCHPADUP,          /**< Game controller touchpad finger was lifted */
    CONTROLLERSENSORUPDATE,        /**< Game controller sensor was updated */

    /* Touch events */
    FINGERDOWN = 0x700,
    FINGERUP,
    FINGERMOTION,

    /* Gesture events */
    DOLLARGESTURE = 0x800,
    DOLLARRECORD,
    MULTIGESTURE,

    /* Clipboard events */
    CLIPBOARDUPDATE = 0x900, /**< The clipboard changed */

    /* Drag and drop events */
    DROPFILE = 0x1000, /**< The system requests a file open */
    DROPTEXT,                 /**< text/plain drag-and-drop event */
    DROPBEGIN,                /**< A new set of drops is beginning (NULL filename) */
    DROPCOMPLETE,             /**< Current set of drops is now complete (NULL filename) */

    /* Audio hotplug events */
    AUDIODEVICEADDED = 0x1100, /**< A new audio device is available */
    AUDIODEVICEREMOVED,        /**< An audio device has been removed. */

    /* Sensor events */
    SENSORUPDATE = 0x1200,     /**< A sensor was updated */

    /* Render events */
    RENDER_TARGETS_RESET = 0x2000, /**< The render targets have been reset and their contents need to be updated */
    RENDER_DEVICE_RESET, /**< The device has been reset and all textures need to be recreated */

    /** Events ::USEREVENT through ::LASTEVENT are for your use,
     *  and should be allocated with RegisterEvents()
     */
    USEREVENT = 0x8000,

    /**
     *  This last event is only for bounding internal arrays
     */
    LASTEVENT = 0xFFFF
}

public enum SDL_GL_Attr
{
    RED_SIZE,
    GREEN_SIZE,
    BLUE_SIZE,
    ALPHA_SIZE,
    BUFFER_SIZE,
    DOUBLEBUFFER,
    DEPTH_SIZE,
    STENCIL_SIZE,
    ACCUM_RED_SIZE,
    ACCUM_GREEN_SIZE,
    ACCUM_BLUE_SIZE,
    ACCUM_ALPHA_SIZE,
    STEREO,
    MULTISAMPLEBUFFERS,
    MULTISAMPLESAMPLES,
    ACCELERATED_VISUAL,
    RETAINED_BACKING,
    CONTEXT_MAJOR_VERSION,
    CONTEXT_MINOR_VERSION,
    CONTEXT_EGL,
    CONTEXT_FLAGS,
    CONTEXT_PROFILE_MASK,
    SHARE_WITH_CURRENT_CONTEXT,
    FRAMEBUFFER_SRGB_CAPABLE,
    CONTEXT_RELEASE_BEHAVIOR,
    CONTEXT_RESET_NOTIFICATION,
    CONTEXT_NO_ERROR
}

public enum SDL_GL_Profile
{
    CONTEXT_PROFILE_CORE = 0x0001,
    CONTEXT_PROFILE_COMPATIBILITY = 0x0002,
    CONTEXT_PROFILE_ES = 0x0004 /**< GLX_CONTEXT_ES2_PROFILE_BIT_EXT */
}

public enum SDL_GL_ContextFlag
{
    CONTEXT_DEBUG_FLAG = 0x0001,
    CONTEXT_FORWARD_COMPATIBLE_FLAG = 0x0002,
    CONTEXT_ROBUST_ACCESS_FLAG = 0x0004,
    CONTEXT_RESET_ISOLATION_FLAG = 0x0008
}


public struct SDL_KeyboardEvent
{
    public uint type;        /**< ::SDL_KEYDOWN or ::SDL_KEYUP */
    public uint timestamp;   /**< In milliseconds, populated using SDL_GetTicks() */
    public uint windowID;    /**< The window with keyboard focus, if any */
    public byte state;        /**< ::SDL_PRESSED or ::SDL_RELEASED */
    public byte repeat;       /**< Non-zero if this is a key repeat */
    public byte padding2;
    public byte padding3;
    public SDL_Keysym keysym;  /**< The key that was pressed or released */
}

public struct SDL_Keysym
{
    public SDL_SCANCODE scancode;
    public int sym;
    public ushort mod;
    uint unused;
}

public struct SDL_CommonEvent
{
    public uint type;
    public uint timestamp;   /**< In milliseconds, populated using SDL_GetTicks() */
}

public enum SDL_SCANCODE : uint
{
    UNKNOWN = 0,

    /**
     *  \name Usage page 0x07
     *
     *  These values are from usage page 0x07 (USB keyboard page).
     */
    /* @{ */

    A = 4,
    B = 5,
    C = 6,
    D = 7,
    E = 8,
    F = 9,
    G = 10,
    H = 11,
    I = 12,
    J = 13,
    K = 14,
    L = 15,
    M = 16,
    N = 17,
    O = 18,
    P = 19,
    Q = 20,
    R = 21,
    S = 22,
    T = 23,
    U = 24,
    V = 25,
    W = 26,
    X = 27,
    Y = 28,
    Z = 29,

    Number_1 = 30,
    Number_2 = 31,
    Number_3 = 32,
    Number_4 = 33,
    Number_5 = 34,
    Number_6 = 35,
    Number_7 = 36,
    Number_8 = 37,
    Number_9 = 38,
    Number_0 = 39,

    RETURN = 40,
    ESCAPE = 41,
    BACKSPACE = 42,
    TAB = 43,
    SPACE = 44,

    MINUS = 45,
    EQUALS = 46,
    LEFTBRACKET = 47,
    RIGHTBRACKET = 48,
    BACKSLASH = 49, /**< Located at the lower left of the return
                                  *   key on ISO keyboards and at the right end
                                  *   of the QWERTY row on ANSI keyboards.
                                  *   Produces REVERSE SOLIDUS (backslash) and
                                  *   VERTICAL LINE in a US layout, REVERSE
                                  *   SOLIDUS and VERTICAL LINE in a UK Mac
                                  *   layout, NUMBER SIGN and TILDE in a UK
                                  *   Windows layout, DOLLAR SIGN and POUND SIGN
                                  *   in a Swiss German layout, NUMBER SIGN and
                                  *   APOSTROPHE in a German layout, GRAVE
                                  *   ACCENT and POUND SIGN in a French Mac
                                  *   layout, and ASTERISK and MICRO SIGN in a
                                  *   French Windows layout.
                                  */
    NONUSHASH = 50, /**< ISO USB keyboards actually use this code
                                  *   instead of 49 for the same key, but all
                                  *   OSes I've seen treat the two codes
                                  *   identically. So, as an implementor, unless
                                  *   your keyboard generates both of those
                                  *   codes and your OS treats them differently,
                                  *   you should generate SDL_SCANCODE_BACKSLASH
                                  *   instead of this code. As a user, you
                                  *   should not rely on this code because SDL
                                  *   will never generate it with most (all?)
                                  *   keyboards.
                                  */
    SEMICOLON = 51,
    APOSTROPHE = 52,
    GRAVE = 53, /**< Located in the top left corner (on both ANSI
                              *   and ISO keyboards). Produces GRAVE ACCENT and
                              *   TILDE in a US Windows layout and in US and UK
                              *   Mac layouts on ANSI keyboards, GRAVE ACCENT
                              *   and NOT SIGN in a UK Windows layout, SECTION
                              *   SIGN and PLUS-MINUS SIGN in US and UK Mac
                              *   layouts on ISO keyboards, SECTION SIGN and
                              *   DEGREE SIGN in a Swiss German layout (Mac:
                              *   only on ISO keyboards), CIRCUMFLEX ACCENT and
                              *   DEGREE SIGN in a German layout (Mac: only on
                              *   ISO keyboards), SUPERSCRIPT TWO and TILDE in a
                              *   French Windows layout, COMMERCIAL AT and
                              *   NUMBER SIGN in a French Mac layout on ISO
                              *   keyboards, and LESS-THAN SIGN and GREATER-THAN
                              *   SIGN in a Swiss German, German, or French Mac
                              *   layout on ANSI keyboards.
                              */
    COMMA = 54,
    PERIOD = 55,
    SLASH = 56,

    CAPSLOCK = 57,

    F1 = 58,
    F2 = 59,
    F3 = 60,
    F4 = 61,
    F5 = 62,
    F6 = 63,
    F7 = 64,
    F8 = 65,
    F9 = 66,
    F10 = 67,
    F11 = 68,
    F12 = 69,

    PRINTSCREEN = 70,
    SCROLLLOCK = 71,
    PAUSE = 72,
    INSERT = 73, /**< insert on PC, help on some Mac keyboards (but
                      does send code 73, not 117) */
    HOME = 74,
    PAGEUP = 75,
    DELETE = 76,
    END = 77,
    PAGEDOWN = 78,
    RIGHT = 79,
    LEFT = 80,
    DOWN = 81,
    UP = 82,

    NUMLOCKCLEAR = 83, /**< num lock on PC, clear on Mac keyboards
                        */
    KP_DIVIDE = 84,
    KP_MULTIPLY = 85,
    KP_MINUS = 86,
    KP_PLUS = 87,
    KP_ENTER = 88,
    KP_1 = 89,
    KP_2 = 90,
    KP_3 = 91,
    KP_4 = 92,
    KP_5 = 93,
    KP_6 = 94,
    KP_7 = 95,
    KP_8 = 96,
    KP_9 = 97,
    KP_0 = 98,
    KP_PERIOD = 99,

    NONUSBACKSLASH = 100, /**< This is the additional key that ISO
                           *   keyboards have over ANSI ones,
                           *   located between left shift and Y.
                           *   Produces GRAVE ACCENT and TILDE in a
                           *   US or UK Mac layout, REVERSE SOLIDUS
                           *   (backslash) and VERTICAL LINE in a
                           *   US or UK Windows layout, and
                           *   LESS-THAN SIGN and GREATER-THAN SIGN
                           *   in a Swiss German, German, or French
                           *   layout. */
    APPLICATION = 101, /**< windows contextual menu, compose */
    POWER = 102, /**< The USB document says this is a status flag,
                  *   not a physical key - but some Mac keyboards
                  *   do have a power key. */
    KP_EQUALS = 103,
    F13 = 104,
    F14 = 105,
    F15 = 106,
    F16 = 107,
    F17 = 108,
    F18 = 109,
    F19 = 110,
    F20 = 111,
    F21 = 112,
    F22 = 113,
    F23 = 114,
    F24 = 115,
    EXECUTE = 116,
    HELP = 117,
    MENU = 118,
    SELECT = 119,
    STOP = 120,
    AGAIN = 121,   /**< redo */
    UNDO = 122,
    CUT = 123,
    COPY = 124,
    PASTE = 125,
    FIND = 126,
    MUTE = 127,
    VOLUMEUP = 128,
    VOLUMEDOWN = 129,
    /* not sure whether there's a reason to enable these */
    /*     SDL_SCANCODE_LOCKINGCAPSLOCK = 130,  */
    /*     SDL_SCANCODE_LOCKINGNUMLOCK = 131, */
    /*     SDL_SCANCODE_LOCKINGSCROLLLOCK = 132, */
    KP_COMMA = 133,
    KP_EQUALSAS400 = 134,

    INTERNATIONAL1 = 135, /**< used on Asian keyboards, see
                               footnotes in USB doc */
    INTERNATIONAL2 = 136,
    INTERNATIONAL3 = 137, /**< Yen */
    INTERNATIONAL4 = 138,
    INTERNATIONAL5 = 139,
    INTERNATIONAL6 = 140,
    INTERNATIONAL7 = 141,
    INTERNATIONAL8 = 142,
    INTERNATIONAL9 = 143,
    LANG1 = 144, /**< Hangul/English toggle */
    LANG2 = 145, /**< Hanja conversion */
    LANG3 = 146, /**< Katakana */
    LANG4 = 147, /**< Hiragana */
    LANG5 = 148, /**< Zenkaku/Hankaku */
    LANG6 = 149, /**< reserved */
    LANG7 = 150, /**< reserved */
    LANG8 = 151, /**< reserved */
    LANG9 = 152, /**< reserved */

    ALTERASE = 153, /**< Erase-Eaze */
    SYSREQ = 154,
    CANCEL = 155,
    CLEAR = 156,
    PRIOR = 157,
    RETURN2 = 158,
    SEPARATOR = 159,
    OUT = 160,
    OPER = 161,
    CLEARAGAIN = 162,
    CRSEL = 163,
    EXSEL = 164,

    KP_00 = 176,
    KP_000 = 177,
    THOUSANDSSEPARATOR = 178,
    DECIMALSEPARATOR = 179,
    CURRENCYUNIT = 180,
    CURRENCYSUBUNIT = 181,
    KP_LEFTPAREN = 182,
    KP_RIGHTPAREN = 183,
    KP_LEFTBRACE = 184,
    KP_RIGHTBRACE = 185,
    KP_TAB = 186,
    KP_BACKSPACE = 187,
    KP_A = 188,
    KP_B = 189,
    KP_C = 190,
    KP_D = 191,
    KP_E = 192,
    KP_F = 193,
    KP_XOR = 194,
    KP_POWER = 195,
    KP_PERCENT = 196,
    KP_LESS = 197,
    KP_GREATER = 198,
    KP_AMPERSAND = 199,
    KP_DBLAMPERSAND = 200,
    KP_VERTICALBAR = 201,
    KP_DBLVERTICALBAR = 202,
    KP_COLON = 203,
    KP_HASH = 204,
    KP_SPACE = 205,
    KP_AT = 206,
    KP_EXCLAM = 207,
    KP_MEMSTORE = 208,
    KP_MEMRECALL = 209,
    KP_MEMCLEAR = 210,
    KP_MEMADD = 211,
    KP_MEMSUBTRACT = 212,
    KP_MEMMULTIPLY = 213,
    KP_MEMDIVIDE = 214,
    KP_PLUSMINUS = 215,
    KP_CLEAR = 216,
    KP_CLEARENTRY = 217,
    KP_BINARY = 218,
    KP_OCTAL = 219,
    KP_DECIMAL = 220,
    KP_HEXADECIMAL = 221,

    LCTRL = 224,
    LSHIFT = 225,
    LALT = 226, /**< alt, option */
    LGUI = 227, /**< windows, command (apple), meta */
    RCTRL = 228,
    RSHIFT = 229,
    RALT = 230, /**< alt gr, option */
    RGUI = 231, /**< windows, command (apple), meta */

    MODE = 257,    /**< I'm not sure if this is really not covered
                                 *   by any of the above, but since there's a
                                 *   special KMOD_MODE for it I'm adding it here
                                 */

    /* @} *//* Usage page 0x07 */

    /**
     *  \name Usage page 0x0C
     *
     *  These values are mapped from usage page 0x0C (USB consumer page).
     */
    /* @{ */

    AUDIONEXT = 258,
    AUDIOPREV = 259,
    AUDIOSTOP = 260,
    AUDIOPLAY = 261,
    AUDIOMUTE = 262,
    MEDIASELECT = 263,
    WWW = 264,
    MAIL = 265,
    CALCULATOR = 266,
    COMPUTER = 267,
    AC_SEARCH = 268,
    AC_HOME = 269,
    AC_BACK = 270,
    AC_FORWARD = 271,
    AC_STOP = 272,
    AC_REFRESH = 273,
    AC_BOOKMARKS = 274,

    /* @} *//* Usage page 0x0C */

    /**
     *  \name Walther keys
     *
     *  These are values that Christian Walther added (for mac keyboard?).
     */
    /* @{ */

    BRIGHTNESSDOWN = 275,
    BRIGHTNESSUP = 276,
    DISPLAYSWITCH = 277, /**< display mirroring/dual display
                              switch, video mode switch */
    KBDILLUMTOGGLE = 278,
    KBDILLUMDOWN = 279,
    KBDILLUMUP = 280,
    EJECT = 281,
    SLEEP = 282,

    APP1 = 283,
    APP2 = 284,

    /* @} *//* Walther keys */

    /**
     *  \name Usage page 0x0C (additional media keys)
     *
     *  These values are mapped from usage page 0x0C (USB consumer page).
     */
    /* @{ */

    AUDIOREWIND = 285,
    AUDIOFASTFORWARD = 286,

    /* @} *//* Usage page 0x0C (additional media keys) */

    /* Add any other keys here. */

    SDL_NUM_SCANCODES = 512 /**< not a key, just marks the number of scancodes
                                 for array bounds */
}