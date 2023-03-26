using System.Numerics;
using System.Runtime.InteropServices;
using ThirdParty.OpenGL;

namespace SBEngine.Editor;

public interface ILayoutHandler
{
    Box CalcBoxForSize(Vector2 preferredSize);
    Vector2 GetMaxSize();
}

public enum ContentAlignment
{
    START,
    MIDDLE,
    END
}

public class IMGUIStyle
{
    public float UnitPerPixel = 80;
    public float FontSize = 0.25f;

    public Vector4 TextColor = VColors.White;
    public Vector4 TextColorNegative = VColors.Black;

    public Vector4 PrimaryColor = new Vector4(0.3f, 0.3f, 0.3f, 1f);
    public Vector4 HoverPrimaryColor = new Vector4(0.5f, 0.5f, 0.5f, 1f);
    public Vector4 ActivePrimaryColor = new Vector4(0.8f, 0.8f, 0.8f, 1f);

    public float BorderSize => FontSize / 16f;
    public float ElementHeight => FontSize * 1.5f;
}

[Flags]
public enum IMGUIElementState
{
    NONE = 0,
    HOVERED = 1,
    ACTIVE = 2,
    SELECTED = 4
}

[Flags]
public enum IMGUIElementStateChange
{
    NONE = 0,
    HOVER_START = 1 << 0,
    HOVER_STOP = 1 << 1,
    ACTIVE_START = 1 << 2,
    ACTIVE_STOP = 1 << 3,
    SELECT_START = 1 << 4,
    SELECT_STOP = 1 << 5
}

public delegate bool ValueParser<T>(string str, ref T arg);

public class IMGUI
{
    public Vector2 MousePosition { get; private set; }
    public Vector2 DelataMousePosition { get; private set; }
    public ILayoutHandler Layout => _layoutStack.Peek();

    public int DrawLayer = 0;

    public Camera2D Camera;
    public IMGUIStyle Style;

    public Dictionary<string, object> _elements;

    private Engine _engine;
    private QuadMeshBuffer _buffer;
    private Material _generalMaterial;
    private Box _workingArea;
    private TrueTypeFont _font;
    private Stack<ILayoutHandler> _layoutStack;
    private DrawCallMultiple _drawCallCommands, _textDrawCallCommands;

    private string? _activeElement = null;
    private string? _selectedElement = null;
    private InputEvents _inputEvents;

    public IMGUI(Engine engine, InputEvents inputEvents)
    {
        _engine = engine;
        _inputEvents = inputEvents;
        _elements = new Dictionary<string, object>();
        Style = new IMGUIStyle();
        _layoutStack = new Stack<ILayoutHandler>();
        _drawCallCommands = new DrawCallMultiple();
        _textDrawCallCommands = new DrawCallMultiple();

        _font = new TrueTypeFont(engine.Renderer.Gl, new ResourcePath("OfficeCodePro.ttf").PathFromRoot);
        char[] chars = new char[128];

        for (int i = 0; i < 128; i++)
        {
            chars[i] = (char)i;
        }

        _font.BakeGlyphs(chars, 32);

        _buffer = new QuadMeshBuffer(engine.Renderer.Gl);

        _generalMaterial = new Material(engine.AssetsDatabase.GetAsset<GLShader>(".\\Resources\\Shaders\\uber.glsl"));
        _generalMaterial.Variables["uTexture"] = VariableValueUnion.Texture(0, _font.Texture);
    }

    public bool GetData<T>(string id, out T state) where T : class, new()
    {
        if (_elements.TryGetValue(id, out object obj) == false)
        {
            state = new T();
            _elements.Add(id, state);
            return true;
        }

        state = obj as T;
        return false;
    }

    public void BeginFrame()
    {
        Camera = new Camera2D();
        var windowSize = _engine.Window.GetWindowSize().ToVector2();
        Camera.Size = windowSize / Style.UnitPerPixel;
        Camera.Offset = Camera.Size / -2f;

        _workingArea = new Box(Camera.Position - Camera.Offset, Camera.Size);
        UpdateMousePosition(Camera.Size);
        _inputEvents.Update(_engine.Input);

        _layoutStack.Clear();
        _layoutStack.Push(new ConstantLayout(_workingArea));
    }

    public void EndFrame()
    {
        var drawCallGeneral = new DrawCall(_buffer, _generalMaterial, new Transform2D());
        drawCallGeneral.Multiple = _drawCallCommands;

        var drawCallText = new DrawCall(_buffer, _generalMaterial, new Transform2D());
        drawCallText.Multiple = _textDrawCallCommands;
        
        _engine.Renderer.SetDepthTesting(true);
        _engine.Renderer.ClearDepth();
        
        _engine.Renderer.QueueDrawCall(drawCallText, 0);
        _engine.Renderer.QueueDrawCall(drawCallGeneral, 0);
        _engine.Renderer.Render(Camera);
        
        _buffer.Clear();
        _drawCallCommands.Clear();
        _textDrawCallCommands.Clear();
    }

    void UpdateMousePosition(Vector2 activeAreaSize)
    {
        var windowSize = _engine.Window.GetWindowSize().ToVector2();

        var mousePosition = _engine.Input.MousePosition;
        mousePosition.Y = windowSize.Y - _engine.Input.MousePosition.Y;

        MousePosition = mousePosition / windowSize * activeAreaSize;

        var deltaMousePosition = _engine.Input.MousePositionDelta;
        deltaMousePosition.Y *= -1;

        DelataMousePosition = deltaMousePosition / windowSize * activeAreaSize;
    }

    public IMGUIElementState UpdateState(string id, Box box)
    {
        bool hovering = box.IsPointInside(MousePosition);
        bool active = id == _activeElement;
        bool selected = id == _selectedElement;

        if (_inputEvents.TryGetMouseButtonEvent(MouseButton.LEFT, InputState.DOWN, out var mouseEventDown))
        {
            if (hovering)
            {
                mouseEventDown.Use();
                active = true;
                _activeElement = id;
            }
            else
            {
                if (selected)
                {
                    selected = false;
                    _selectedElement = null;
                }
            }
        }

        if (_inputEvents.TryGetMouseButtonEvent(MouseButton.LEFT, InputState.UP, out var mouseEventUp))
        {
            if (hovering)
            {
                mouseEventUp.Use();

                selected = true;
                _selectedElement = id;
            }

            active = false;
            _activeElement = null;
        }

        return (hovering ? IMGUIElementState.HOVERED : IMGUIElementState.NONE) |
               (active ? IMGUIElementState.ACTIVE : IMGUIElementState.NONE) |
               (selected ? IMGUIElementState.SELECTED : IMGUIElementState.NONE);
    }

    public IMGUIElementStateChange GetStateChanges(IMGUIElementState prevState, IMGUIElementState state)
    {
        IMGUIElementStateChange change = IMGUIElementStateChange.NONE;

        //HOVER_START 
        if (prevState.Flag(IMGUIElementState.HOVERED) == false && state.Flag(IMGUIElementState.HOVERED))
        {
            change |= IMGUIElementStateChange.HOVER_START;
        }

        //HOVER_STOP
        if (prevState.Flag(IMGUIElementState.HOVERED) && state.Flag(IMGUIElementState.HOVERED) == false)
        {
            change |= IMGUIElementStateChange.HOVER_STOP;
        }

        //ACTIVE_START
        if (prevState.Flag(IMGUIElementState.ACTIVE) == false && state.Flag(IMGUIElementState.ACTIVE))
        {
            change |= IMGUIElementStateChange.ACTIVE_START;
        }

        //ACTIVE_STOP 
        if (prevState.Flag(IMGUIElementState.ACTIVE) && state.Flag(IMGUIElementState.ACTIVE) == false)
        {
            change |= IMGUIElementStateChange.ACTIVE_STOP;
        }

        //SELECT_START
        if (prevState.Flag(IMGUIElementState.SELECTED) == false && state.Flag(IMGUIElementState.SELECTED))
        {
            change |= IMGUIElementStateChange.SELECT_START;
        }

        //SELECT_STOP 
        if (prevState.Flag(IMGUIElementState.SELECTED) && state.Flag(IMGUIElementState.SELECTED) == false)
        {
            change |= IMGUIElementStateChange.SELECT_STOP;
        }

        return change;
    }

    public void AnimateFader<T>(ref AnimationFader<T> fader, IMGUIElementState prevState, IMGUIElementState state,
        T normal, T hovered, T active)
    {
        var stateChange = GetStateChanges(prevState, state);

        bool startedHovering = stateChange.Flag(IMGUIElementStateChange.HOVER_START);
        bool endedHovering = stateChange.Flag(IMGUIElementStateChange.HOVER_STOP);
        bool becomeActive = stateChange.Flag(IMGUIElementStateChange.ACTIVE_START);
        bool becomeInactive = stateChange.Flag(IMGUIElementStateChange.ACTIVE_STOP);

        bool isActive = state.Flag(IMGUIElementState.ACTIVE);

        // hover / normal -> active
        if (becomeActive)
        {
            fader.ChangedState(active);
        }

        if (isActive == false)
        {
            // normal -> hover 
            if (startedHovering)
            {
                fader.ChangedState(hovered);
            }

            // hover -> normal
            if (endedHovering)
            {
                fader.ChangedState(normal);
            }
        }

        // active -> hover / normal 
        if (becomeInactive)
        {
            // hovering
            if ((state & IMGUIElementState.HOVERED) != 0)
            {
                fader.ChangedState(hovered);
            }
            else
            {
                fader.ChangedState(normal);
            }
        }
    }

    public void Gap(Vector2 size)
    {
        var max = Layout.GetMaxSize();

        if (size.X == 0)
        {
            size.X = max.X;
        }

        if (size.Y == 0)
        {
            size.Y = max.Y;
        }

        Layout.CalcBoxForSize(size);
    }

    public void Box(Vector4 color, float z = 0)
    {
        var box = Layout.CalcBoxForSize(Layout.GetMaxSize());
        _buffer.AddColorQuad(new Vector3(box.Position, z), box.Size, color);
        _drawCallCommands.PushCall(6);
    }

    public void Box(Vector2 size, Vector4 color, float z = 0)
    {
        var box = Layout.CalcBoxForSize(size);
        _buffer.AddColorQuad(new Vector3(box.Position, z), box.Size, color);
        _drawCallCommands.PushCall(6);
    }

    public void Box(Box box, Vector4 color, float z = 0)
    {
        _buffer.AddColorQuad(new Vector3(box.Position, z), box.Size, color);
        _drawCallCommands.PushCall(6);
    }

    public void Box(Vector3 position, Vector2 size, Vector4 color)
    {
        _buffer.AddColorQuad(position, size, color);
        _drawCallCommands.PushCall(6);
    }

    public void Text(string text, Vector4 color, float size = 0, float preferredMinWidth = 0, float z = 0)
    {
        if (size == 0)
        {
            size = Style.FontSize;
        }

        var box = QuadMeshBufferUtils.GetTextQuadsBoundingBox(_font, text, size);

        if (preferredMinWidth != 0)
        {
            box.Size.X = MathF.Max(preferredMinWidth, box.Size.X);
        }

        var givenBox = Layout.CalcBoxForSize(box.Size);

        givenBox.AlignInside(ref box, Alignment.LEFT | Alignment.TOP);

        var quads = _buffer.AddTextQuads(_font, text, new Vector3(box.Left, box.Top, z), size, color);
        _drawCallCommands.PushCall(6 * quads);
    }

    public void Text(string text, float preferredMinWidth = 0)
    {
        Text(text, Style.TextColor, Style.FontSize, preferredMinWidth);
    }

    public bool Button(string id, string text, float z = 0)
    {
        if (GetData<IMGUIButton>(id, out var button))
        {
            button.BackgroundColorFader.Reset(Style.PrimaryColor, 0.1f);
        }

        button.BackgroundColorFader.Update(_engine.Time.DeltaTime);

        // layouting

        var textWidth = QuadMeshBufferUtils.GetTextQuadsBoundingBox(_font, text, Style.FontSize).Size.X;

        var preferredBoxSize =
            new Vector2(textWidth + (Style.BorderSize * 4),
                Style.FontSize * 1.5f); //textBoundingBox.Size + new Vector2(0.25f);
        var box = Layout.CalcBoxForSize(preferredBoxSize);

        var textPosition = AlignTextInBox(Style.FontSize, box);

        var prevState = button.State;
        button.State = UpdateState(id, box);
        var stateChange = GetStateChanges(prevState, button.State);

        // render

        AnimateFader(ref button.BackgroundColorFader, prevState, button.State,
            Style.PrimaryColor, Style.HoverPrimaryColor, Style.ActivePrimaryColor);

        Vector4 textColor = button.State.Flag(IMGUIElementState.ACTIVE)
            ? Style.TextColorNegative
            : Style.TextColor;

        var quads = _buffer.AddTextQuads(_font, text, new Vector3(textPosition, z), Style.FontSize, textColor);
        _textDrawCallCommands.PushCall(6 * quads);
        
        _buffer.AddColorQuad(new Vector3(box.Position, z), box.Size, button.BackgroundColorFader.Value);
        _drawCallCommands.PushCall(6);

        bool clicked = stateChange.Flag(IMGUIElementStateChange.ACTIVE_STOP) &&
                       button.State.Flag(IMGUIElementState.HOVERED);

        return clicked;
    }

    Vector2 AlignTextInBox(float textSize, Box box)
    {
        var textPosition = box.Position;
        textPosition.X = box.Left + (Style.BorderSize * 2);
        textPosition.Y += (_font.Ascent - _font.Descent) * Style.FontSize / 2f;

        return textPosition;
    }

    public bool StringField(string id, ref string text, float z = 0)
    {
        if (GetData<IMGUIStringField>(id, out var field))
        {
            field.BackgroundColorFader.Reset(Style.PrimaryColor, 0.1f);
            field.cursor.Position = text.Length;
        }

        field.BackgroundColorFader.Update(_engine.Time.DeltaTime);

        // layouting

        var preferredBoxSize =
            new Vector2(Layout.GetMaxSize().X, Style.FontSize * 1.5f); //textBoundingBox.Size + new Vector2(0.25f);
        var box = Layout.CalcBoxForSize(preferredBoxSize);

        var textPosition = AlignTextInBox(Style.FontSize, box);

        var innerBox = box;
        innerBox.ChangeSideLength(-Style.BorderSize, BoxSide.ALL);

        // state change

        var prevState = field.State;
        field.State = UpdateState(id, box);
        var stateChange = GetStateChanges(prevState, field.State);

        AnimateFader(ref field.BackgroundColorFader, prevState, field.State,
            Style.PrimaryColor, Style.HoverPrimaryColor, Style.ActivePrimaryColor);

        // input

        if (stateChange.Flag(IMGUIElementStateChange.SELECT_START))
        {
            _inputEvents.TextFieldInputs.StartUsingTextInput();
            field.cursor.Position = text.Length;
        }

        if (stateChange.Flag(IMGUIElementStateChange.SELECT_STOP))
        {
            _inputEvents.TextFieldInputs.StopUsingTextInput();
        }

        bool changedText = false;

        if (field.State.Flag(IMGUIElementState.SELECTED))
        {
            changedText = _inputEvents.TextFieldInputs.EditTextLine(ref text, ref field.cursor);
        }

        //rendering

        //@TODO add scissoring for string field so the text doesnt go outside the box. 
        var quads = _buffer.AddTextQuads(_font, text, new Vector3(textPosition, z), Style.FontSize, Style.TextColor);
        _textDrawCallCommands.PushCall(6 * quads);
        
        _buffer.AddColorQuad(new Vector3(box.Position, z), box.Size, Style.ActivePrimaryColor);
        _drawCallCommands.PushCall(6);
        
        _buffer.AddColorQuad(new Vector3(innerBox.Position, z), innerBox.Size, field.BackgroundColorFader.Value);
        _drawCallCommands.PushCall(6);

        // cursor

        if (field.State.Flag(IMGUIElementState.SELECTED))
        {
            string substring = text.Substring(0, field.cursor.Position);
            var cursorBox = QuadMeshBufferUtils.GetTextQuadsBoundingBox(_font, substring, Style.FontSize);
            var left = textPosition.X + cursorBox.Size.X;

            _buffer.AddColorQuad(new Vector3(left, box.Position.Y, z),
                new Vector2(Style.FontSize / 16f, Style.FontSize), Style.TextColor);
            _drawCallCommands.PushCall(6);
        }

        return changedText;
    }

    public bool ParsedStringField<T>(string id, ref T value, ValueParser<T> parser, float z = 0)
    {
        string valueString = value.ToString();

        if (GetData<IMGUIStringField>(id, out var field))
        {
            field.BackgroundColorFader.Reset(Style.PrimaryColor, 0.1f);
            field.cursor.Position = valueString.Length;
        }

        field.BackgroundColorFader.Update(_engine.Time.DeltaTime);

        // layouting

        var preferredBoxSize =
            new Vector2(Layout.GetMaxSize().X, Style.FontSize * 1.5f); //textBoundingBox.Size + new Vector2(0.25f);
        var box = Layout.CalcBoxForSize(preferredBoxSize);

        float borderSize = Style.FontSize / 16f;

        var textPosition = AlignTextInBox(Style.FontSize, box);

        var innerBox = box;
        innerBox.ChangeSideLength(-borderSize, BoxSide.ALL);

        // state change

        var prevState = field.State;
        field.State = UpdateState(id, box);
        var stateChange = GetStateChanges(prevState, field.State);

        AnimateFader(ref field.BackgroundColorFader, prevState, field.State,
            Style.PrimaryColor, Style.HoverPrimaryColor, Style.ActivePrimaryColor);

        // input

        if (stateChange.Flag(IMGUIElementStateChange.SELECT_START))
        {
            _inputEvents.TextFieldInputs.StartUsingTextInput();
            field.cursor.Position = valueString.Length;
        }

        if (stateChange.Flag(IMGUIElementStateChange.SELECT_STOP))
        {
            _inputEvents.TextFieldInputs.StopUsingTextInput();
        }

        bool changedText = false;

        if (field.State.Flag(IMGUIElementState.SELECTED))
        {
            var cursorCopy = field.cursor;
            changedText = _inputEvents.TextFieldInputs.EditTextLine(ref valueString, ref field.cursor);

            if (changedText)
            {
                if (parser(valueString, ref value) == false)
                {
                    field.cursor = cursorCopy;
                    valueString = value.ToString();
                }
            }
        }

        //rendering

        //@TODO add scissoring for string field so the text doesnt go outside the box. 
        var quads = _buffer.AddTextQuads(_font, valueString, new Vector3(textPosition, z), Style.FontSize, Style.TextColor);
        _textDrawCallCommands.PushCall(6 * quads);
        
        _buffer.AddColorQuad(new Vector3(box.Position, z), box.Size, Style.ActivePrimaryColor);
        _drawCallCommands.PushCall(6);
        
        _buffer.AddColorQuad(new Vector3(innerBox.Position, z), innerBox.Size, field.BackgroundColorFader.Value);
        _drawCallCommands.PushCall(6);

        // cursor

        if (field.State.Flag(IMGUIElementState.SELECTED))
        {
            string substring = valueString.Substring(0, field.cursor.Position);
            var cursorBox = QuadMeshBufferUtils.GetTextQuadsBoundingBox(_font, substring, Style.FontSize);
            var left = textPosition.X + cursorBox.Size.X;

            _buffer.AddColorQuad(new Vector3(left, box.Position.Y, z),
                new Vector2(Style.FontSize / 16f, Style.FontSize), Style.TextColor);
            _drawCallCommands.PushCall(6);
        }

        return changedText;
    }

    public bool IntField(string id, ref int value)
    {
        return ParsedStringField(id, ref value, Parser.ParseInt);
    }

    public bool FloatField(string id, ref float value)
    {
        return ParsedStringField(id, ref value, Parser.ParseFloat);
    }

    public bool CheckBox(string id, ref bool value, float z = 0)
    {
        if (GetData<IMGUIStringField>(id, out var checkBox))
        {
            checkBox.BackgroundColorFader.Reset(Style.PrimaryColor, 0.1f);
        }

        checkBox.BackgroundColorFader.Update(_engine.Time.DeltaTime);

        //layouting
        float borderSize = Style.FontSize / 16f;

        Vector2 size = new Vector2(Style.FontSize * 1.5f);
        var box = Layout.CalcBoxForSize(size);

        var innerBox = box;
        innerBox.ChangeSideLength(-borderSize, BoxSide.ALL);

        var symbolBox = innerBox;
        symbolBox.ChangeSideLength(-borderSize * 3, BoxSide.ALL);

        // state

        var prevState = checkBox.State;
        checkBox.State = UpdateState(id, box);

        var changedState = GetStateChanges(prevState, checkBox.State);

        var changed = false;

        if (changedState.Flag(IMGUIElementStateChange.ACTIVE_STOP) &&
            checkBox.State.Flag(IMGUIElementState.HOVERED))
        {
            value = !value;
            changed = true;
        }

        AnimateFader(ref checkBox.BackgroundColorFader, prevState, checkBox.State,
            Style.PrimaryColor, Style.HoverPrimaryColor, Style.ActivePrimaryColor);

        // render

        _buffer.AddColorQuad(new Vector3(box.Position, z), box.Size, Style.ActivePrimaryColor);
        _drawCallCommands.PushCall(6);
        
        _buffer.AddColorQuad(new Vector3(innerBox.Position, z), innerBox.Size, checkBox.BackgroundColorFader.Value);
        _drawCallCommands.PushCall(6);

        if (value)
        {
            _buffer.AddColorQuad(new Vector3(symbolBox.Position, z), symbolBox.Size, VColors.Green);
            _drawCallCommands.PushCall(6);
        }

        return changed;
    }

    // Layouts
    // @TODO think about the most simple and effective layouting system.

    public void EndLayout()
    {
        _layoutStack.Pop();
    }

    public void BeginColumns(float width = 0, ContentAlignment contentAlignment = ContentAlignment.MIDDLE)
    {
        var size = Layout.GetMaxSize();

        if (width != 0)
        {
            size.Y = MathF.Min(width, size.Y);
        }

        Box box = Layout.CalcBoxForSize(size);
        var newLayout = new RowLayout(box, contentAlignment);

        _layoutStack.Push(newLayout);
    }

    public void BeginRows(float height = 0, ContentAlignment contentAlignment = ContentAlignment.MIDDLE)
    {
        var size = Layout.GetMaxSize();

        if (height != 0)
        {
            size.X = MathF.Min(size.X, height);
        }

        Box box = Layout.CalcBoxForSize(size);
        var newLayout = new ColumnLayout(box, contentAlignment);

        _layoutStack.Push(newLayout);
    }

    public void CapWidth(float width, int times = 1)
    {
        var maxSize = Layout.GetMaxSize();
        var newLayout = new CapLayout(this, new Vector2(width, maxSize.Y), Layout, times);
        _layoutStack.Push(newLayout);
    }

    public void CapHeight(float height, int times = 1)
    {
        var maxSize = Layout.GetMaxSize();
        var newLayout = new CapLayout(this, new Vector2(maxSize.X, height), Layout, times);
        _layoutStack.Push(newLayout);
    }

    public void LayoutBoxOnce(Box box)
    {
    }

    public void BeginLayers(int numberOfLayers = 1)
    {
        var maxSize = Layout.GetMaxSize();
        var box = Layout.CalcBoxForSize(maxSize);
        var newLayout = new LayersLayout(this, box, numberOfLayers);
        _layoutStack.Push(newLayout);
    }

    public void BeginConstant(Box box)
    {
        _layoutStack.Push(new ConstantLayout(box));
    }

    public void SplitHorizontalPanels(string id, Box[] boxes)
    {
        GetData<IMGUISplitPanel>(id, out var data);

        float dragAreaWidth = 0.125f;

        for (int i = 0; i < boxes.Length; i++)
        {
            float width = boxes[i].Size.X - dragAreaWidth;
            
            if(i != 0) // left side
            { 
                var box = boxes[i];
                box.ChangeSideLength(-width, BoxSide.RIGHT);

                if (box.IsPointInside(MousePosition))
                {
                    if (_inputEvents.TryGetMouseButtonEvent(MouseButton.LEFT, InputState.DOWN, out var lmev))
                    {
                        lmev.Use();
                        data.Dragging = true;
                        data.SelectedEdge = i - 1;
                    }

                    Box(box, new Vector4(0, 0, 0, 0.75f));
                }
            }

            { // right side
                var box = boxes[i];

                box.ChangeSideLength(-width, BoxSide.LEFT);

                if (box.IsPointInside(MousePosition))
                {
                    if (_inputEvents.TryGetMouseButtonEvent(MouseButton.LEFT, InputState.DOWN, out var lmev))
                    {
                        lmev.Use();
                        data.Dragging = true;
                        data.SelectedEdge = i;
                    }

                    Box(box, new Vector4(0, 0, 0, 0.75f));
                }
            }
        }

        if (_inputEvents.TryGetMouseButtonEvent(MouseButton.LEFT, InputState.UP, out var lmru))
        {
            data.Dragging = false;
        }

        if (data.Dragging)
        {
            boxes[data.SelectedEdge].ChangeSideLength(DelataMousePosition.X, BoxSide.LEFT);
            boxes[data.SelectedEdge + 1].ChangeSideLength(-DelataMousePosition.X, BoxSide.RIGHT);
        }
    }
}