using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Transform _map;

    private int _maxFoodOnMap;
    private int _maxTimberOnMap;
    private int _maxMarbleOnMap;

    private float _mapX;
    private float _mapZ;
    private float _spawnDelay;

    private BaseResource _prefabFood;
    private BaseResource _prefabTimber;
    private BaseResource _prefabMarble;

    private ObjectPool<BaseResource> _poolFood;
    private ObjectPool<BaseResource> _poolTimber;
    private ObjectPool<BaseResource> _poolMarble;

    private void Start()
    {
        const float OffsetFromTheEdgeOfTheMap = 1;
        const float Half = 0.5f;
        const float PlaneScale = 10;
        const float Area = PlaneScale * Half - OffsetFromTheEdgeOfTheMap;

        _prefabFood = Resources.Load<BaseResource>("Prefabs/Food");
        _prefabTimber = Resources.Load<BaseResource>("Prefabs/Timber");
        _prefabMarble = Resources.Load<BaseResource>("Prefabs/Marble");

        _poolFood = new ObjectPool<BaseResource>(_prefabFood, Create, Enable, Disable);
        _poolTimber = new ObjectPool<BaseResource>(_prefabTimber, Create, Enable, Disable);
        _poolMarble = new ObjectPool<BaseResource>(_prefabMarble, Create, Enable, Disable);

        _maxFoodOnMap = 3;
        _maxMarbleOnMap = 3;
        _maxTimberOnMap = 5;
        _spawnDelay = 3f;

        _mapX = _map.localScale.x * Area;
        _mapZ = _map.localScale.z * Area;

        StartCoroutine(InitialResourceSpawn());
    }

    private void OnDisable()
    {
        _poolFood.ReturnAll();
        _poolTimber.ReturnAll();
        _poolMarble.ReturnAll();

        StopAllCoroutines();
    }

    private BaseResource Create(BaseResource prefab)
    {
        var obj = Instantiate<BaseResource>(prefab);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Enable(BaseResource obj)
    {
        obj.gameObject.SetActive(true);
        obj.Harvest += OnResourceHarvest;
        obj.transform.rotation = Quaternion.identity;
        obj.transform.position = Vector3.zero;
    }

    private void Disable(BaseResource obj)
    {
        obj.Harvest -= OnResourceHarvest;
        obj.gameObject.SetActive(false);
    }

    private void OnResourceHarvest(BaseResource resource)
    {
        resource.transform.SetParent(transform);

        switch (resource.ResourceType)
        {
            case ResourceType.Food:
                _poolFood.Return(resource);
                int numberOfFood = _maxFoodOnMap - _poolFood.ActiveResourcesCount;
                StartCoroutine(SpawnResource(_poolFood, numberOfFood));
                break;

            case ResourceType.Timber:
                _poolTimber.Return(resource);
                int numberOfTimber = _maxTimberOnMap - _poolTimber.ActiveResourcesCount;
                StartCoroutine(SpawnResource(_poolTimber, numberOfTimber));
                break;

            case ResourceType.Marble:
                _poolMarble.Return(resource);
                int numberOfMarble = _maxMarbleOnMap - _poolMarble.ActiveResourcesCount;
                StartCoroutine(SpawnResource(_poolMarble, numberOfMarble));
                break;

            default:
                throw new Exception("Unknown resource");
        }

    }

    private IEnumerator SpawnResource(ObjectPool<BaseResource> pool, int numberOfResource)
    {
        for (int i = 0; i < numberOfResource; i++)
        {
            yield return new WaitForSeconds(_spawnDelay);

            float posX = Random.Range(-_mapX, _mapX);
            float posZ = Random.Range(-_mapZ, _mapZ);
            Vector3 spawnPos = new Vector3(posX, 0f, posZ);
            var resource = pool.Get();
            resource.transform.position = spawnPos;
            resource.gameObject.SetActive(true);
        }
    }

    private IEnumerator InitialResourceSpawn()
    {
        float temp = _spawnDelay;
        _spawnDelay = 0;
        yield return StartCoroutine(SpawnResource(_poolFood, _maxFoodOnMap));
        yield return StartCoroutine(SpawnResource(_poolTimber, _maxTimberOnMap));
        yield return StartCoroutine(SpawnResource(_poolMarble, _maxMarbleOnMap));
        _spawnDelay = temp;
    }
}