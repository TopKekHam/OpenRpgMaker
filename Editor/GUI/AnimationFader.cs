namespace SBEngine.Editor;

public delegate T ValueMixer<T>(T val1, T val2, float normal);

public struct AnimationFader<T>
{
    public T Value { get => _mixer(_previewsValue, _currentValue, _timer / _duration); }

    private ValueMixer<T> _mixer;
    private T _previewsValue;
    private T _currentValue;
    private float _duration;
    public float _timer;
    
    public AnimationFader(ValueMixer<T> mixer)
    {
        _mixer = mixer;
        _previewsValue = default;
        _currentValue = default;
        _duration = default;
        _timer = default;
    }

    public void Reset(T value, float duration)
    {
        _previewsValue = value;
        _currentValue = value;
        _duration = duration;
        _timer = duration;
    }
    
    public void ChangedState(T newValue)
    {
        _previewsValue = Value;
        _currentValue = newValue;
        _timer = 0;
    }

    public void Update(float deltaTime)
    {
        _timer = MathF.Min(_duration, deltaTime + _timer);
    }
}