using System;
using UnityEngine;

public class ResourceSpawner
{
    //[SerializeField] private Transform _map;                // �������� � ������� ��������, � ������ ���������� ���������� ��� ������ �������, ���� � ����� ������

    private readonly Transform Parent;
    private readonly Resource PrefabResource;
    private readonly Func<Resource, Resource> CreateResource;         // ������������ �������� ����� readoly?

    private ObjectPool<Resource> _pool;
    //private int _maxRecourcesOnMap;                         // ������������ ���-�� ����� ������� �������

    //private float _mapX;                                    // �������� ���������� ������ ����� ������� �������
    //private float _mapZ;                                    // �������� ���������� ������ ����� ������� �������
    //private float _spawnDelay;                              // ������������ �������� ����� ������� �������
    public Action Collected;                          // ����� �� ���������� ������???

    public int NumberOfActiveResources => _pool.ActiveResourcesCount;

    public ResourceSpawner(Resource prefabResource, Func<Resource, Resource> createFunc, Transform parent)             // �������� �� start?
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

    private void OnResourceHarvest(Resource resource)                                               // �������� ������
    {
        resource.transform.SetParent(Parent);
        _pool.Return(resource);
        Collected?.Invoke(resource);
        //int numberOfFood = _maxFoodOnMap - _poolFood.ActiveResourcesCount;                          // ����� ������� ����� ���������� ������ � �� ������� ����� �������� ������� �������
        //StartCoroutine(SpawnResource(_poolFood, numberOfFood));                                     // ����� ������� ������� ����� �������� ������� ������� ������� �� ����� �������� � ������� �����������
    }
}
