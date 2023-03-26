using System.Runtime.InteropServices;
using ThirdParty.SDL;

namespace SBEngine.Editor;

public struct TextFieldCursorLine
{
    public int Position;
}

public class HeldKey
{
    public Key Key;
    public float Timer;
    public bool HeldThisUpdate;
}

public unsafe class TextFieldInputs
{
    public Vector2I CursorPositionDelta { get; private set; }
    public int DeleteCharacters { get; private set; }
    public string StringInput { get; private set; }

    public float RepeatDuration = 0.075f;
    public float FirstRepeatDuration = 0.3f;

    private int _activeCount;
    private bool _editingText;
    private string _thisFrameString;
    private List<HeldKey> _keysHeld;
    private Time _time;

    public TextFieldInputs(PlatformEvents platformEvents, Time time)
    {
        _time = time;
        platformEvents.SDLEventEvent.Subscribe(HandleSDLEvent);

        CursorPositionDelta = Vector2I.Zero;
        DeleteCharacters = 0;
        _editingText = false;
        StringInput = "";
        _thisFrameString = "";
        _activeCount = 0;
        _keysHeld = new List<HeldKey>();
    }

    public void StartUsingTextInput()
    {
        if (_activeCount == 0)
        {
            SDL.SDL_StartTextInput();
            _editingText = true;
        }

        _activeCount += 1;
    }

    public void StopUsingTextInput()
    {
        _activeCount -= 1;

        if (_activeCount == 0)
        {
            SDL.SDL_StopTextInput();
            _editingText = false;
        }
    }

    public void Clear()
    {
        StringInput = "";
    }

    /// <summary>
    /// Will use any unicode character, arrow key and backspace key events.
    /// </summary>
    /// <param name="inputEvents"></param>
    public void Update(InputEvents inputEvents)
    {
        if (_editingText == false) return;

        CursorPositionDelta = Vector2I.Zero;
        DeleteCharacters = 0;

        for (int i = _keysHeld.Count - 1; i >= 0; i--)
        {
            _keysHeld[i].HeldThisUpdate = false;
        }
        
        for (int i = 0; i < inputEvents.KeyEvents.Count; i++)
        {
            var keyEvent = inputEvents.KeyEvents[i];

            if (keyEvent.Used) continue;
            
            HandleEventKey(keyEvent);
        }

        for (int i = _keysHeld.Count - 1; i >= 0; i--)
        {
            if (_keysHeld[i].HeldThisUpdate == false)
            {
                _keysHeld.RemoveAt(i);
            }
        }
    }
    
    public bool EditTextLine(ref string text, ref TextFieldCursorLine cursor)
    {
        cursor.Position += CursorPositionDelta.X;
        cursor.Position = MathEx.Clamp(cursor.Position, 0, text.Length);

        bool textChanged = false;
        
        while (DeleteCharacters > 0 && cursor.Position != 0)
        {
            text = text.Remove(cursor.Position - 1, 1);
            DeleteCharacters -= 1;
            cursor.Position -= 1;
            textChanged = true;
        }

        if (string.IsNullOrEmpty(StringInput) == false)
        {
            text = text.Insert(cursor.Position, StringInput);
            cursor.Position += StringInput.Length;
            textChanged = true;
        }

        return textChanged;
    }

    int HandleRepeat(InputEvent<Key> keyEvent)
    {
        if (keyEvent.State == InputState.DOWN) return 1;
        if (keyEvent.State == InputState.UP) return 0;

        HeldKey heldKey = null;

        for (int i = 0; i < _keysHeld.Count; i++)
        {
            if (_keysHeld[i].Key == keyEvent.InputType)
            {
                heldKey = _keysHeld[i];
            }
        }

        if (heldKey == null)
        {
            heldKey = new HeldKey()
            {
                Key = keyEvent.InputType,
                Timer = FirstRepeatDuration
            };

            _keysHeld.Add(heldKey);
        }
        else
        {
            heldKey.Timer -= _time.DeltaTime;
        }

        heldKey.HeldThisUpdate = true;

        int repeats = 0;

        while (heldKey.Timer < 0)
        {
            heldKey.Timer += RepeatDuration;
            repeats++;
        }

        return repeats;
    }

    void HandleSDLEvent(SDL_Event sdlEvent)
    {
        if (sdlEvent.type == SDL_EventType.TEXTINPUT)
        {
            byte* ptr = sdlEvent.text.text;
            StringInput += Marshal.PtrToStringUni(new IntPtr(ptr));
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="keyEvent">KeyEvent can be any state</param>
    void HandleEventKey(InputEvent<Key> keyEvent)
    {
        if (keyEvent.State == InputState.UP) return;

        var temp = CursorPositionDelta;
        int times = HandleRepeat(keyEvent);
        
        if (keyEvent.InputType == Key.ARROW_UP)
        {
            temp.Y += times;
            keyEvent.Use();
        }
        else if (keyEvent.InputType == Key.ARROW_DOWN)
        {
            temp.Y -= times;
            keyEvent.Use();
        }
        else if (keyEvent.InputType == Key.ARROW_RIGHT)
        {
            temp.X += times;
            keyEvent.Use();
        }
        else if (keyEvent.InputType == Key.ARROW_LEFT)
        {
            temp.X -= times;
            keyEvent.Use();
        }
        else if (keyEvent.InputType == Key.BACKSPACE)
        {
            DeleteCharacters += times;
            keyEvent.Use();
        }
        else
        {
            // what do with the unicode characters ?
        }

        
        CursorPositionDelta = temp;
    }
}