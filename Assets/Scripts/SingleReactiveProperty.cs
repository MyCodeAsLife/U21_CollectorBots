using System;

public class SingleReactiveProperty<T>
{
    private T _value;

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

    public event Action<T> Change;
}
