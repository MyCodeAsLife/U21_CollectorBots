using System;
using UnityEngine;

public class ResourceSpawnerBase
{
    private readonly Transform _parent;
    private readonly Resource _prefabResource;
    private readonly Func<Resource, Resource> _createResource;

    private ObjectPool<Resource> _pool;

    public Action Collected;

    public int NumberOfActiveResources => _pool.ActiveResourcesCount;

    public ResourceSpawnerBase(Resource prefabResource, Func<Resource, Resource> createFunc, Transform parent)
    {
        _prefabResource = prefabResource;
        _createResource = createFunc;
        _parent = parent;
        _pool = new ObjectPool<Resource>(_prefabResource, Create, Enable, Disable);
    }

    ~ResourceSpawnerBase()
    {
        RemoveResourcesFromMap();
    }

    public void RemoveResourcesFromMap()
    {
        _pool.ReturnAll();
    }

    public void Spawn(Vector3 spawnPosition)
    {
        var resource = _pool.Get();
        resource.transform.position = spawnPosition;
        resource.gameObject.SetActive(true);
    }

    private Resource Create(Resource prefab)
    {
        var obj = _createResource(prefab);
        obj.transform.SetParent(_parent);

        return obj;
    }

    private void Enable(Resource obj)
    {
        obj.gameObject.SetActive(true);
        obj.Harvest += OnResourceHarvest;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.position = Vector3.zero;
    }

    private void Disable(Resource obj)
    {
        obj.Harvest -= OnResourceHarvest;
        obj.gameObject.SetActive(false);
    }

    private void OnResourceHarvest(Resource resource)
    {
        resource.transform.SetParent(_parent);
        _pool.Return(resource);
        Collected?.Invoke();
    }
}