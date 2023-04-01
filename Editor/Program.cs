using static SDL;

public unsafe class Program
{

    public static bool Running;

    static void Main(string[] args)
    {

        SDL_Init(SDL_InitFlags.SDL_INIT_EVERYTHING);

        Time.Load();

        Window.Load(1280, 720, WindowFlags.RESIZABLE);
        Window.QuitEvent.Subscribe(() => { Running = false; });

        OpenGL.Load(Window.WindowHandle, new OpenGLConfig()
        {
            majorVersion = 4,
            minorVersion = 6,
            frameBuffering = OpenGLBuffering.DOUBLE,
            depthBufferSize = 16
        });

        Running = true;
        while (Running) {

            Time.Update();
            Window.Update();

            OpenGL.glClearColor(1, 0, 1, 1);
            OpenGL.glClear(GL.COLOR_BUFFER_BIT);

            Window.SwapGLBuffer();
        }

    }

}