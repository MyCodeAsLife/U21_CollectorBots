using System.Collections;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    [SerializeField] private Transform _map;

    private int _maxFoodOnMap;
    private int _maxMarbleOnMap;
    private int _maxTimberOnMap;

    private float _mapX;
    private float _mapZ;
    private float _spawnDelay;

    private ResourceFood _prefabFood;
    private ResourceMarble _prefabMarble;
    private ResourceTimber _prefabTimber;

    private ObjectPool<BaseResource> _poolFood;
    private ObjectPool<BaseResource> _poolMarble;
    private ObjectPool<BaseResource> _poolTimber;

    private void Start()
    {
        _prefabFood = Resources.Load<ResourceFood>("Prefabs/Food");
        _prefabMarble = Resources.Load<ResourceMarble>("Prefabs/Marble");
        _prefabTimber = Resources.Load<ResourceTimber>("Prefabs/Timber");

        _poolFood = new ObjectPool<BaseResource>(_prefabFood, Create, Enable, Disable);
        _poolMarble = new ObjectPool<BaseResource>(_prefabMarble, Create, Enable, Disable);
        _poolTimber = new ObjectPool<BaseResource>(_prefabTimber, Create, Enable, Disable);


        _maxFoodOnMap = 3;
        _maxMarbleOnMap = 3;
        _maxTimberOnMap = 5;
        _spawnDelay = 3f;

        _mapX = _map.localScale.x * 4;      // Магическое число
        _mapZ = _map.localScale.z * 4;      // Магическое число
        //Debug.Log(_map.localScale);

        StartCoroutine(InitialResourceSpawn());
    }

    private void OnDisable()
    {
        _poolFood.ReturnAll();
        _poolMarble.ReturnAll();
        _poolTimber.ReturnAll();

        StopAllCoroutines();
    }

    private BaseResource Create(BaseResource prefab)        // Доработать спавн ресурсов (Общий базовый класс?)
    {
        var obj = Instantiate<BaseResource>(prefab);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Enable(BaseResource obj)     // Переработать
    {
        obj.gameObject.SetActive(true);
        //obj.TimeEnded += OnLifetimeEnded;     // При спавне ресурса, подписатся на его событие "сбора"
    }

    private void Disable(BaseResource obj)    // Переработать
    {
        //obj.TimeEnded -= OnLifetimeEnded;     // При сборе ресурса, отписатся от его события "сбора"
        obj.gameObject.SetActive(false);
    }

    private IEnumerator SpawnResource(ObjectPool<BaseResource> pool, int count)     // Подписатся на событие ресурса, когда его подбирают чтобы запустить данную корутину
    {
        for (int i = 0; i < count; i++)
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
        yield return StartCoroutine(SpawnResource(_poolMarble, _maxMarbleOnMap));
        yield return StartCoroutine(SpawnResource(_poolTimber, _maxTimberOnMap));
    }
}
