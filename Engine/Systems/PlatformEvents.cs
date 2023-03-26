using System.Runtime.InteropServices;
using ThirdParty.SDL;
using static ThirdParty.SDL.SDL;

namespace SBEngine;

public enum PlatformEventType
{
    UNKNOWN,
    QUIT,
    WINDOW_RESIZED
}

public unsafe interface SDLEventHandler
{
    void Handle(SDL_Event* sdlEvent);
}

[StructLayout(LayoutKind.Explicit)]
public struct PlatformEvent
{
    [FieldOffset(0)] public PlatformEventType Type;
    [FieldOffset(4)] public WindowResized WindowResized;
}

public struct WindowResized
{
    public Vector2I NewSize;
}


public unsafe class PlatformEvents
{
    public IReadOnlyList<PlatformEvent> Events => _events;
    public EventEmitter<SDL_Event> SDLEventEvent { get; private set; }

    private List<PlatformEvent> _events;
    private Input _input;
    private Emitter<SDL_Event> _SDLEventEventEmitter;
    private Window _window;

    public PlatformEvents(Input input, Window window)
    {
        _events = new List<PlatformEvent>();
        _input = input;
        _window = window;
        SDLEventEvent = new EventEmitter<SDL_Event>(out _SDLEventEventEmitter);
    }

    public void Update()
    {
        _events.Clear();

        SDL_Event sdlEvent = new SDL_Event();

        while (SDL_PollEvent(ref sdlEvent))
        {
            PlatformEvent platformEvent = new PlatformEvent();

            if (sdlEvent.type == SDL_EventType.QUIT)
            {
                platformEvent.Type = PlatformEventType.QUIT;
            }
            else if (sdlEvent.type == SDL_EventType.WINDOWEVENT)
            {

                if (sdlEvent.window.event_id == SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED)
                {
                    platformEvent.Type = PlatformEventType.WINDOW_RESIZED;

                    _window.Size = new Vector2I(sdlEvent.window.data1, sdlEvent.window.data2);

                    platformEvent.WindowResized = new WindowResized()
                    {
                        NewSize = new Vector2I(sdlEvent.window.data1, sdlEvent.window.data2)
                    };
                }
                
            }

            _input.HandleSDLEvent(&sdlEvent);

            // this shit is coping like 64 bytes each call back...
            _SDLEventEventEmitter.Emit(sdlEvent);

            if (platformEvent.Type != PlatformEventType.UNKNOWN)
            {
                _events.Add(platformEvent);
            }
        }
    }
}