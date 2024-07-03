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

    public int ActiveResourcesCount => _active.Count;
    //public Action<int> ChangedActiveObjectsCount;

    //public int TotalSpawnedObjects { get; private set; }

    public ObjectPool(T environments, Func<T, T> createObject, Action<T> enableObject, Action<T> disableObject)
    {
        _environments = environments;
        CreateObject = createObject;
        EnableObject = enableObject;
        DisableObject = disableObject;

        Return(CreateObject(_environments));
    }

    public T Get()
    {
        T obj = _pool.Count < 1 ? CreateObject(_environments) : _pool.Dequeue();
        EnableObject(obj);
        _active.Add(obj);
        //ChangedActiveObjectsCount?.Invoke(_active.Count);
        //TotalSpawnedObjects++;

        return obj;
    }

    public void Return(T obj)
    {
        DisableObject(obj);
        _active.Remove(obj);
        _pool.Enqueue(obj);
        //ChangedActiveObjectsCount?.Invoke(_active.Count);
    }

    public void ReturnAll()
    {
        foreach (T obj in _active.ToArray())
            Return(obj);
    }
}
