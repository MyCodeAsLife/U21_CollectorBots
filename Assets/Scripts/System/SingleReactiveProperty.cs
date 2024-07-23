using System;

public class SingleReactiveProperty<T>
{
    private T _value;

    public event Action<T> Change;

    public T Value
    {
        get
        {
            return _value;
        }
        set
        {
            _value = value;
            Change?.Invoke(Value);
        }
    }
}
