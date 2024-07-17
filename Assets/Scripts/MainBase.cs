using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBase : MonoBehaviour
{
    [SerializeField] private Transform _map;
    [SerializeField] private Transform _gatheringPoint;             // Точка сбора спавнящихся юнитов
    [SerializeField] private CollectorBotSpawner _botSpawner;

    //private Explosion _prefabExplosion;                           // Взять из проекта U20, передовать ссылку на этот эффект в мобов
    private float _scanningRadius;
    private int _maxCountCollectorBots;
    private int _food;
    private int _timber;
    private int _marble;

    private IList<BaseResource> _freeResources;
    private IList<BaseResource> _resourcesPlannedForCollection;
    private IList<CollectorBot> _poolOfWorkingCollectorBots;
    private IList<CollectorBot> _poolOfIdleCollectorBots;

    private void Start()    //++
    {
        _maxCountCollectorBots = 3;
        //_prefabExplosion = Resources.Load<Explosion>("Prefabs/Explosion");      // Взять из проекта U20
        _freeResources = new List<BaseResource>();
        _resourcesPlannedForCollection = new List<BaseResource>();
        _scanningRadius = _map.localScale.x > _map.localScale.z ? _map.localScale.x : _map.localScale.z;
        _scanningRadius *= 5;   // Магическое число (кол-во unit/2 в одном "кубе" карты. Сделать константой.
        _poolOfWorkingCollectorBots = new List<CollectorBot>();
        _poolOfIdleCollectorBots = new List<CollectorBot>();

        StartCoroutine(ResourceScanning());
        StartCoroutine(StartInitialization());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetResource(ResourceType resourceType)
    {
        for (int i = 0; i < _resourcesPlannedForCollection.Count; ++i)
            if (_resourcesPlannedForCollection[i].ResourceType == resourceType)
            {
                _resourcesPlannedForCollection.RemoveAt(i);
                break;
            }

        switch (resourceType)
        {
            case ResourceType.Food:
                _food++;
                break;

            case ResourceType.Timber:
                _timber++;
                break;

            case ResourceType.Marble:
                _marble++;
                break;
        }
    }

    public void OnCollectorBotTaskCompleted(CollectorBot bot)
    {
        _poolOfWorkingCollectorBots.Remove(bot);
        _poolOfIdleCollectorBots.Add(bot);
    }

    private void FindFreeResources()         // Переименовать(потому как ничего не возвращает) или переместить в корутину ResourceScanning
    {
        Collider[] hits = Physics.OverlapSphere(Vector3.zero, _scanningRadius);

        foreach (Collider hit in hits)
            if (hit.TryGetComponent(out BaseResource resource))
                if (_freeResources.Contains(resource) == false)                             // Проблема при сравнении ресурса           ++++++++
                    if (_resourcesPlannedForCollection.Contains(resource) == false)         // Проблема при сравнении ресурса           ++++++++
                        _freeResources.Add(resource);
    }

    private IEnumerator ResourceScanning()          // Работать постоянно или когда закончатся ресурсы?
    {
        float Delay = 2.0f;     // const?
        bool isWork = true;

        while (isWork)
        {
            yield return new WaitForSeconds(Delay);
            FindFreeResources();
            //ShowScanningResultInDebug();                        // ----

            if (_freeResources.Count > 0 && _poolOfIdleCollectorBots.Count > 0)
                DistributeCollectionTasks();
        }
    }

    private void DistributeCollectionTasks()                // Распределение заданий на сбор ресурсов
    {
        bool isWork = true;

        while (isWork)
        {
            if (_freeResources.Count < 1 || _poolOfIdleCollectorBots.Count < 1)
                break;

            _poolOfIdleCollectorBots[0].SetCollectionTask(_freeResources[0]);
            _resourcesPlannedForCollection.Add(_freeResources[0]);
            _freeResources.RemoveAt(0);
            _poolOfWorkingCollectorBots.Add(_poolOfIdleCollectorBots[0]);
            _poolOfIdleCollectorBots.RemoveAt(0);
        }
    }

    private void CreateCollectorBot()
    {
        var collectorBot = _botSpawner.GetCollectorBot();
        collectorBot.TaskCompleted += OnCollectorBotTaskCompleted;
        collectorBot.transform.position = transform.position;
        collectorBot.SetBaseAffiliation(this);
        collectorBot.GoTo(_gatheringPoint.position);
        _poolOfWorkingCollectorBots.Add(collectorBot);
    }

    private IEnumerator StartInitialization()      // 2 пула ботов (занятые и свободные).
    {
        var delay = new WaitForSeconds(1f);

        for (int i = 0; i < _maxCountCollectorBots; i++)
        {
            yield return delay;
            CreateCollectorBot();
        }
    }

    private void ShowScanningResultInDebug()        // ---- Для тестирования
    {
        int foodCount = 0;
        int timberCount = 0;
        int marbleCount = 0;

        for (int i = 0; i < _freeResources.Count; i++)
        {
            switch (_freeResources[i].ResourceType)
            {
                case ResourceType.Food:
                    foodCount++;
                    break;

                case ResourceType.Timber:
                    timberCount++;
                    break;

                case ResourceType.Marble:
                    marbleCount++;
                    break;

                default:
                    throw new Exception("Incorrect resource type");
            }
        }

        Debug.Log("Food = " + foodCount + ". Timber = " + timberCount + ". Marble = " + marbleCount);
    }
}
