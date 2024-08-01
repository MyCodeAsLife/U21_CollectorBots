using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MainSpawner : MonoBehaviour
{
    [SerializeField] private Transform _map;
    //private ResourceSpawner[] resourceSpawners;               // Сделать универсальный массив? тогда необходимо делать методы на свичах?
    private ResourceSpawner _foodSpawner;
    private ResourceSpawner _timberSpawner;
    private ResourceSpawner _marbleSpawner;


    private int _maxFoodOnMap;
    private int _maxTimberOnMap;
    private int _maxMarbleOnMap;

    private float _mapX;
    private float _mapZ;
    private float _spawnDelay;

    //private Resource _prefabFood;
    //private Resource _prefabTimber;
    //private Resource _prefabMarble;

    //private ObjectPool<Resource> _poolFood;
    //private ObjectPool<Resource> _poolTimber;
    //private ObjectPool<Resource> _poolMarble;

    private void Start()
    {
        const float OffsetFromTheEdgeOfTheMap = 1;
        const float Half = 0.5f;
        const float PlaneScale = 10;
        const float Area = PlaneScale * Half - OffsetFromTheEdgeOfTheMap;

        //_prefabFood = Resources.Load<Resource>("Prefabs/Food");
        //_prefabTimber = Resources.Load<Resource>("Prefabs/Timber");
        //_prefabMarble = Resources.Load<Resource>("Prefabs/Marble");

        var foodPrefab = Resources.Load<Resource>("Prefabs/Food");                                      // Переместить в Awake
        _foodSpawner = new ResourceSpawner(foodPrefab, CreateResource, transform);                      // Переместить в Awake
        _foodSpawner.Collected += OnFoodCollecting;                                                     // Переместить в Awake или Enable

        var timberPrefab = Resources.Load<Resource>("Prefabs/Timber");                                  // Переместить в Awake
        _timberSpawner = new ResourceSpawner(timberPrefab, CreateResource, transform);                  // Переместить в Awake
        _timberSpawner.Collected += OnTimberCollecting;                                                 // Переместить в Awake или Enable

        var marblePrefab = Resources.Load<Resource>("Prefabs/Marble");                                  // Переместить в Awake
        _marbleSpawner = new ResourceSpawner(marblePrefab, CreateResource, transform);                  // Переместить в Awake
        _marbleSpawner.Collected += OnMarbleCollecting;                                                 // Переместить в Awake или Enable

        //_poolFood = new ObjectPool<Resource>(_prefabFood, Create, Enable, Disable);
        //_poolTimber = new ObjectPool<Resource>(_prefabTimber, Create, Enable, Disable);
        //_poolMarble = new ObjectPool<Resource>(_prefabMarble, Create, Enable, Disable);

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
        //_poolFood.ReturnAll();
        //_poolTimber.ReturnAll();
        //_poolMarble.ReturnAll();
        _foodSpawner.Collected -= OnFoodCollecting;
        _timberSpawner.Collected -= OnTimberCollecting;
        _marbleSpawner.Collected -= OnMarbleCollecting;

        _foodSpawner.RemoveResourcesFromMap();
        _timberSpawner.RemoveResourcesFromMap();
        _marbleSpawner.RemoveResourcesFromMap();

        StopAllCoroutines();                                            // Выпилить
    }

    private Resource CreateResource(Resource prefab) => Instantiate<Resource>(prefab);

    //private Resource Create(Resource prefab)
    //{
    //    var obj = Instantiate<Resource>(prefab);
    //    obj.transform.SetParent(transform);

    //    return obj;
    //}

    //private void Enable(Resource obj)
    //{
    //    obj.gameObject.SetActive(true);
    //    obj.Harvest += OnResourceHarvest;
    //    obj.transform.rotation = Quaternion.identity;
    //    obj.transform.position = Vector3.zero;
    //}

    //private void Disable(Resource obj)
    //{
    //    obj.Harvest -= OnResourceHarvest;
    //    obj.gameObject.SetActive(false);
    //}

    //private void OnResourceHarvest(Resource resource)           // Разбить на спавнеры для каждого ресурса и через мастер спавнер циклом выбирать какой спавнер вызвать.
    //{
    //    resource.transform.SetParent(transform);

    //    switch (resource.ResourceType)
    //    {
    //        case ResourceType.Food:
    //            _poolFood.Return(resource);
    //            int numberOfFood = _maxFoodOnMap - _poolFood.ActiveResourcesCount;
    //            StartCoroutine(SpawnResource(_poolFood, numberOfFood));
    //            break;

    //        case ResourceType.Timber:
    //            _poolTimber.Return(resource);
    //            int numberOfTimber = _maxTimberOnMap - _poolTimber.ActiveResourcesCount;
    //            StartCoroutine(SpawnResource(_poolTimber, numberOfTimber));
    //            break;

    //        case ResourceType.Marble:
    //            _poolMarble.Return(resource);
    //            int numberOfMarble = _maxMarbleOnMap - _poolMarble.ActiveResourcesCount;
    //            StartCoroutine(SpawnResource(_poolMarble, numberOfMarble));
    //            break;

    //        default:
    //            throw new Exception("Unknown resource");
    //    }
    //}

    private void OnFoodCollecting()
    {
        int numberOfFood = _maxFoodOnMap - _foodSpawner.NumberOfActiveResources;
        StartCoroutine(SpawnRes(_foodSpawner, numberOfFood));
    }

    private void OnTimberCollecting()
    {
        int numberOfTimber = _maxTimberOnMap - _poolFood.ActiveResourcesCount;
        StartCoroutine(SpawnRes(_timberSpawner, numberOfTimber));
    }

    private void OnMarbleCollecting()
    {
        int numberOfMarble = _maxMarbleOnMap - _poolFood.ActiveResourcesCount;
        StartCoroutine(SpawnRes(_marbleSpawner, numberOfMarble));
    }

    private IEnumerator SpawnRes(ResourceSpawner resourceSpawner, int numberOfResource)
    {
        var delay = new WaitForSeconds(_spawnDelay);

        for (int i = 0; i < numberOfResource; i++)
        {
            yield return delay;

            float posX = Random.Range(-_mapX, _mapX);
            float posZ = Random.Range(-_mapZ, _mapZ);
            Vector3 spawnPos = new Vector3(posX, 0f, posZ);

            // Вызывать метод спавна у спавнера и передовать в него координаты
            resourceSpawner.Spawn(spawnPos);
            //var resource = pool.Get();
            //resource.transform.position = spawnPos;
            //resource.gameObject.SetActive(true);
        }
    }

    private IEnumerator SpawnResource(ObjectPool<Resource> pool, int numberOfResource)
    {
        var delay = new WaitForSeconds(_spawnDelay);

        for (int i = 0; i < numberOfResource; i++)
        {
            yield return delay;

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
