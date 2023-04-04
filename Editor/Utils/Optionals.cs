
public struct Optional<TResult, TError>
{
    public bool Valid;
    public TResult? Result;
    public TError? Error;

    public Optional(TResult result)
    {
        Valid = true;
        Result = result;
    }

    public Optional(TError error)
    {
        Valid = false;
        Error = error;
    }

    public static implicit operator bool(Optional<TResult, TError> optional)
    {
        return optional.Valid;
    }
}

public struct Optional<TResult>
{
    public bool Valid;
    public TResult? Result;

    public Optional(TResult result)
    {
        Valid = true;
        Result = result;
    }

    public Optional()
    {
        Valid = false;
    }

    public static implicit operator bool(Optional<TResult> optional)
    {
        return optional.Valid;
    }
}