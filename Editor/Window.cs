using System;
using System.Runtime.InteropServices;
using static SDL;

public class WindowEvent<T>
{
    List<Action<T>> subs = new List<Action<T>>();

    public void EmmitEvent(T value)
    {
        for (int i = 0; i < subs.Count; i++)
        {
            subs[i](value);
        }
    }


    public void Subscribe(Action<T> sub)
    {
        subs.Add(sub);
    }

    public void Unsubscribe(Action<T> sub)
    {
        for (int i = 0; i < subs.Count; i++)
        {
            if (subs[i] == sub)
            {
                subs.RemoveAt(i);
                i--;
            }
        }
    }


    public static WindowEvent<T> operator +(WindowEvent<T> action, Action<T> sub)
    {
        action.subs.Add(sub);
        return action;
    }

    public static WindowEvent<T> operator -(WindowEvent<T> action, Action<T> sub)
    {
        for (int i = 0; i < action.subs.Count; i++)
        {
            if (action.subs[i] == sub)
            {
                action.subs.RemoveAt(i);
                i--;
            }
        }
        return action;
    }
}

public class WindowEvent
{
    List<Action> subs = new List<Action>();

    public void EmmitEvent()
    {
        for (int i = 0; i < subs.Count; i++)
        {
            subs[i]();
        }
    }

    public void Subscribe(Action sub)
    {
        subs.Add(sub);
    }

    public void Unsubscribe(Action sub)
    {
        for (int i = 0; i < subs.Count; i++)
        {
            if (subs[i] == sub)
            {
                subs.RemoveAt(i);
                i--;
            }
        }
    }

    public static WindowEvent operator +(WindowEvent action, Action sub)
    {
        action.subs.Add(sub);
        return action;
    }

    public static WindowEvent operator -(WindowEvent action, Action sub)
    {
        for (int i = 0; i < action.subs.Count; i++)
        {
            if (action.subs[i] == sub)
            {
                action.subs.RemoveAt(i);
                i--;
            }
        }
        return action;
    }
}

public enum WindowFlags : uint
{
    NONE = 0,
    RESIZABLE = SDL_WindowFlags.WINDOW_RESIZABLE,
    FULLSCREEN = SDL_WindowFlags.WINDOW_FULLSCREEN,
    FULLSCREEN_BORDERLESS = SDL_WindowFlags.WINDOW_FULLSCREEN_DESKTOP,
}

public class Window
{

    public static WindowEvent QuitEvent = new WindowEvent();
    public static WindowEvent<Vector2I> ResizeEvent = new WindowEvent<Vector2I>();
    public static WindowEvent<string?> TextEvent = new WindowEvent<string>();

    public static IntPtr WindowHandle;

    public static void Load(int width, int height, WindowFlags flags = WindowFlags.NONE)
    {
        WindowHandle = SDL_CreateWindow("window", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, width, height, SDL_WindowFlags.WINDOW_OPENGL | (SDL_WindowFlags)flags);
    }

    public static void Update()
    {
        SDL_Event ev = new SDL_Event();

        while (SDL_PollEvent(ref ev))
        {
            if (ev.type == SDL_EventType.QUIT)
            {
                QuitEvent.EmmitEvent();
            }
            else if (ev.type == SDL_EventType.WINDOWEVENT)
            {
                if (ev.window.event_id == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                {
                    ResizeEvent.EmmitEvent(new Vector2I(ev.window.data1, ev.window.data2));
                }
            }
            else if (ev.type == SDL_EventType.TEXTINPUT)
            {
                unsafe
                {
                    IntPtr ptr = new IntPtr(ev.text.text);
                    string? text = Marshal.PtrToStringAnsi(ptr);
                    TextEvent.EmmitEvent(text);
                }
            }
        }
    }

    public static Vector2I GetSize()
    {
        Vector2I Vector2 = new Vector2I();
        SDL_GetWindowSize(WindowHandle, ref Vector2.X, ref Vector2.Y);
        return Vector2;
    }

    public static void SetSize(Vector2I vec)
    {
        SDL_SetWindowSize(WindowHandle, vec.X, vec.Y);
    }

    public static string GetTitle()
    {
        return SDL_GetWindowTitleManaged(WindowHandle);
    }

    public static void SetTitle(string title)
    {
        SDL_SetWindowTitle(WindowHandle, title);
    }

    public static void SwapGLBuffer()
    {
        SDL_GL_SwapWindow(WindowHandle);
    }

    //public static Box GetSizeBox()
    //{
    //    var size = GetSize();
    //    return new Box(size.X, size.Y);
    //}

}