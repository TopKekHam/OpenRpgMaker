using System.Numerics;
using ThirdParty.SDL;
using static ThirdParty.SDL.SDL;
using ThirdParty.OpenGL;

namespace SBEngine;

public class Window
{
    private IntPtr _sdlWindowHandle;
    private OpenGL _opengl;

    public Vector2I Size;

    public Window(int width, int height, string title)
    {
        _sdlWindowHandle = SDL_CreateWindow(title, SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED,
            width, height, SDL_WindowFlags.WINDOW_OPENGL | SDL_WindowFlags.WINDOW_RESIZABLE);

        _opengl = new OpenGL(_sdlWindowHandle, new OpenGLConfig()
        {
            majorVersion = 4,
            minorVersion = 6,
            frameBuffering = OpenGLBuffering.DOUBLE,
            depthBufferSize = 16
        });
    }

    public void SwapGLBuffers()
    {
        SDL_GL_SwapWindow(_sdlWindowHandle);
    }

    public OpenGL GetOpenGLReference()
    {
        return _opengl;
    }

    public Vector2I GetWindowSize()
    {
        Vector2I size = new Vector2I();
        SDL_GetWindowSize(_sdlWindowHandle, ref size.X, ref size.Y);
        return size;
    }
    
    public Space GetSpace()
    {
        var size = Size.ToVector2();
        
        return new Space()
        {
            From = Vector2.Zero,
            To = size,
            AxisDirection = new Vector2(1, -1),
        };
    }
}