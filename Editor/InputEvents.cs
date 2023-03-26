using System.Numerics;

namespace SBEngine.Editor;

public class InputEvent<T>
{
    public T InputType;
    public InputState State;

    public bool Used { get; private set; }
    
    public InputEvent(T inputType, InputState state)
    {
        InputType = inputType;
        State = state;
        Used = false;
    }
    
    public bool Equals<T>(T inputType, InputState state)
    {
        //@TODO we probably got boxing here, do something about it this is retarded.
        return inputType.Equals(InputType) && State == state;
    }
    
    public void Use()
    {
        Used = true;
    }
}

public class InputEvents
{
    public TextFieldInputs TextFieldInputs { get; private set; }
    
    public IReadOnlyList<InputEvent<Key>> KeyEvents => _keyEvents;

    private List<InputEvent<Key>> _keyEvents;
    private List<InputEvent<MouseButton>> _mouseButtonEvents;

    private Key[] _keys;
    private MouseButton[] _mouseButtons;
    
    public InputEvents(Engine engine)
    {
        _keyEvents = new List<InputEvent<Key>>();
        _mouseButtonEvents = new List<InputEvent<MouseButton>>();
        _keys = Enum.GetValues<Key>();
        _mouseButtons = Enum.GetValues<MouseButton>();

        TextFieldInputs = new TextFieldInputs(engine.PlatformEvents, engine.Time);
    }

    public void Clear()
    {
        _keyEvents.Clear();
        _mouseButtonEvents.Clear();
        TextFieldInputs.Clear();
    }
    
    public void Update(Input input)
    {
        HandleEvents(_keyEvents, _keys, input.IsKey);
        HandleEvents(_mouseButtonEvents, _mouseButtons, input.IsMouseButton);
        
        TextFieldInputs.Update(this);
    }

    public bool TryGetKeyEvent(Key input, InputState state, out InputEvent<Key>? keyEvent)
    {
        keyEvent = null;
        
        for (int i = 0; i < _keyEvents.Count; i++)
        {
            var ev = _keyEvents[i];

            if (ev.Equals(input, state) && ev.Used == false)
            {
                keyEvent = ev as InputEvent<Key>;
                return true;
            }
        }

        return false;
    }
    
    public bool TryGetMouseButtonEvent(MouseButton input, InputState state, out InputEvent<MouseButton>? mouseButtonEvent)
    {
        mouseButtonEvent = null;
        
        for (int i = 0; i < _mouseButtonEvents.Count; i++)
        {
            var ev = _mouseButtonEvents[i];

            if (ev.Equals(input, state) && ev.Used == false)
            {
                mouseButtonEvent = ev as InputEvent<MouseButton>;
                return true;
            }
        }

        return false;
    }

    void HandleEvents<T>(List<InputEvent<T>> eventList, T[] values, Func<T, InputState, bool> checkEventFunc)
    {
        for (int i = 0; i < values.Length; i++)
        {
            if (checkEventFunc(values[i], InputState.DOWN))
            {
                eventList.Add(new InputEvent<T>(values[i], InputState.DOWN));
            }
            
            if (checkEventFunc(values[i], InputState.UP))
            {
                eventList.Add(new InputEvent<T>(values[i], InputState.UP));
            }
            
            if (checkEventFunc(values[i], InputState.ACTIVE))
            {
                eventList.Add(new InputEvent<T>(values[i], InputState.ACTIVE));
            }
        }
    }

}