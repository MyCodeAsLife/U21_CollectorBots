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

    private ResourceFood _prefabFood;               // Упростить ресурсы до одного базового, а их тип определять по полю внутри класса?
    private ResourceTimber _prefabTimber;
    private ResourceMarble _prefabMarble;

    //private CollectedResource _prefabCollectedFood;
    //private CollectedResource _prefabCollectedTimber;
    //private CollectedResource _prefabCollectedMarble;

    private ObjectPool<BaseResource> _poolFood;
    private ObjectPool<BaseResource> _poolTimber;
    private ObjectPool<BaseResource> _poolMarble;

    private void Start()
    {
        _prefabFood = Resources.Load<ResourceFood>("Prefabs/Food");
        _prefabTimber = Resources.Load<ResourceTimber>("Prefabs/Timber");
        _prefabMarble = Resources.Load<ResourceMarble>("Prefabs/Marble");

        //_prefabCollectedFood = Resources.Load<CollectedResource>("Prefabs/CollectedFood");
        //_prefabCollectedTimber = Resources.Load<CollectedResource>("Prefabs/CollectedTimber");
        //_prefabCollectedMarble = Resources.Load<CollectedResource>("Prefabs/CollectedMarble");

        _poolFood = new ObjectPool<BaseResource>(_prefabFood, Create, Enable, Disable);
        _poolTimber = new ObjectPool<BaseResource>(_prefabTimber, Create, Enable, Disable);
        _poolMarble = new ObjectPool<BaseResource>(_prefabMarble, Create, Enable, Disable);


        _maxFoodOnMap = 3;
        _maxMarbleOnMap = 3;
        _maxTimberOnMap = 5;
        _spawnDelay = 0f;

        _mapX = _map.localScale.x * 4;      // Магическое число
        _mapZ = _map.localScale.z * 4;      // Магическое число
        //Debug.Log(_map.localScale);

        StartCoroutine(InitialResourceSpawn());
    }

    private void OnDisable()
    {
        _poolFood.ReturnAll();
        _poolTimber.ReturnAll();
        _poolMarble.ReturnAll();

        StopAllCoroutines();
    }

    private BaseResource Create(BaseResource prefab)        // Доработать спавн ресурсов (Общий базовый класс?)
    {
        var obj = Instantiate<BaseResource>(prefab);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Enable(BaseResource obj)
    {
        obj.gameObject.SetActive(true);
        obj.Harvest += OnResourceHarvest;
    }

    private void Disable(BaseResource obj)
    {
        obj.Harvest -= OnResourceHarvest;
        obj.gameObject.SetActive(false);
    }

    private void OnResourceHarvest(BaseResource resource)
    {
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

    private IEnumerator SpawnResource(ObjectPool<BaseResource> pool, int numberOfResource)     // Подписатся на событие ресурса, когда его подбирают чтобы запустить данную корутину
    {
        for (int i = 0; i < numberOfResource; i++)
        {
            yield return new WaitForSeconds(_spawnDelay);

            float posX = Random.Range(-_mapX, _mapX);
            float posZ = Random.Range(-_mapZ, _mapZ);
            Vector3 spawnPos = new Vector3(posX, 0f, posZ);

            // Заспавнить ресурс
            var resource = pool.Get();
            resource.transform.position = spawnPos;
            resource.gameObject.SetActive(true);
        }
    }

    private IEnumerator InitialResourceSpawn()
    {
        // Повторять пока не будет достигнут максимум ресурсов
        yield return StartCoroutine(SpawnResource(_poolFood, _maxFoodOnMap));
        yield return StartCoroutine(SpawnResource(_poolTimber, _maxTimberOnMap));
        yield return StartCoroutine(SpawnResource(_poolMarble, _maxMarbleOnMap));
        _spawnDelay = 3f;
    }
}
