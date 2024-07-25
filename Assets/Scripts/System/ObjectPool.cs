using System.Collections.Generic;
using System;

public class ObjectPool<T>
{
    private readonly T _environments;

    private readonly Func<T, T> CreateObject;
    private readonly Action<T> DisableObject;
    private readonly Action<T> EnableObject;

    private Queue<T> _pool = new();
    private List<T> _active = new();

    public ObjectPool(T environments, Func<T, T> createObject, Action<T> enableObject, Action<T> disableObject)
    {
        _environments = environments;
        CreateObject = createObject;
        EnableObject = enableObject;
        DisableObject = disableObject;

        Return(CreateObject(_environments));
    }

    public int ActiveResourcesCount => _active.Count;

    public T Get()
    {
        T obj = _pool.Count > 0 ? _pool.Dequeue() : CreateObject(_environments);
        EnableObject(obj);
        _active.Add(obj);
        return obj;
    }

    public void Return(T obj)
    {
        DisableObject(obj);
        _active.Remove(obj);
        _pool.Enqueue(obj);
    }

    public void ReturnAll()
    {
        foreach (T obj in _active.ToArray())
            Return(obj);
    }
}
