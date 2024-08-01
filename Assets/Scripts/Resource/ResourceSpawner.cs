using System;
using UnityEngine;

public class ResourceSpawner
{
    //[SerializeField] private Transform _map;                // ќставить в главном спавнере, и оттуда передавать координаты дл€ спавна ресурса, сюда в метод спавна

    private readonly Transform Parent;
    private readonly Resource PrefabResource;
    private readonly Func<Resource, Resource> CreateResource;         // ¬озвращаемое значение будет readoly?

    private ObjectPool<Resource> _pool;
    //private int _maxRecourcesOnMap;                         // –егулировать кол-во будет главный спавнер

    //private float _mapX;                                    // ¬ыбирать координаты спавна будет главный спавнер
    //private float _mapZ;                                    // ¬ыбирать координаты спавна будет главный спавнер
    //private float _spawnDelay;                              // –егулировать интервал будет главный спавнер
    public Action Collected;                          // Ќужно ли возвращать ресурс???

    public int NumberOfActiveResources => _pool.ActiveResourcesCount;

    public ResourceSpawner(Resource prefabResource, Func<Resource, Resource> createFunc, Transform parent)             // «аменить на start?
    {
        PrefabResource = prefabResource;
        CreateResource = createFunc;
        Parent = parent;
        _pool = new ObjectPool<Resource>(PrefabResource, Create, Enable, Disable);

        //_maxRecourcesOnMap = 3;
        //_spawnDelay = 3f;
    }

    ~ResourceSpawner()
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
        //var obj = Instantiate<Resource>(prefab);
        var obj = CreateResource(prefab);
        obj.transform.SetParent(Parent);

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

    private void OnResourceHarvest(Resource resource)                                               // ƒописать логику
    {
        resource.transform.SetParent(Parent);
        _pool.Return(resource);
        Collected?.Invoke(resource);
        //int numberOfFood = _maxFoodOnMap - _poolFood.ActiveResourcesCount;                          // јкшин который будет запускатс€ отсюда и на который будет подписан главный спавнер
        //StartCoroutine(SpawnResource(_poolFood, numberOfFood));                                     // «атем главный спавнер будет смотреть сколько данного ресурса на карте осталось и спанить недостающие
    }
}
