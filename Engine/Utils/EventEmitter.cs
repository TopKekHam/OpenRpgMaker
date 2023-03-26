namespace SBEngine;

public struct Emitter<T>
{
    private Action<T> _emitterFunction;

    public Emitter(Action<T> emitterFunction)
    {
        _emitterFunction = emitterFunction;
    }

    public void Emit(T arg)
    {
        _emitterFunction(arg);
    }
        
}

public class EventEmitter<T>
{
    private List<Action<T>> _subscribers;
    
    public EventEmitter(out Emitter<T> emmiter)
    {
        _subscribers = new List<Action<T>>();
        emmiter = new Emitter<T>(EmitEvent);
    }

    public void Subscribe(Action<T> callback)
    {
        _subscribers.Add(callback);
    }

    public void Unsubscribe(Action<T> callback)
    {
        _subscribers.Remove(callback);
    }

    void EmitEvent(T arg)
    {
        for (int i = 0; i < _subscribers.Count; i++)
        {
            _subscribers[i](arg);
        }
    }
    
}