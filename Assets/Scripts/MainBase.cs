using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainBase : MonoBehaviour
{
    [SerializeField] private Transform _map;
    [SerializeField] private Transform _gatheringPoint;
    [SerializeField] private TMP_Text _displayNumberOfFood;
    [SerializeField] private TMP_Text _displayNumberOfTimber;
    [SerializeField] private TMP_Text _displayNumberOfMarble;
    [SerializeField] private CollectorBotSpawner _botSpawner;

    private Vector3 _scanningArea;
    private int _maxCountCollectorBots;
    private SingleReactiveProperty<int> _numberOfFood;
    private SingleReactiveProperty<int> _numberOfTimber;
    private SingleReactiveProperty<int> _numberOfMarble;

    private IList<BaseResource> _freeResources;
    private IList<BaseResource> _resourcesPlannedForCollection;
    private IList<CollectorBot> _poolOfWorkingCollectorBots;
    private IList<CollectorBot> _poolOfIdleCollectorBots;

    private void Awake()
    {
        _numberOfFood = new SingleReactiveProperty<int>();
        _numberOfTimber = new SingleReactiveProperty<int>();
        _numberOfMarble = new SingleReactiveProperty<int>();
    }

    private void OnEnable()
    {
        _numberOfFood.Change += DisplayNumberOfFood;
        _numberOfTimber.Change += DisplayNumberOfTimber;
        _numberOfMarble.Change += DisplayNumberOfMarble;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        _numberOfFood.Change -= DisplayNumberOfFood;
        _numberOfTimber.Change -= DisplayNumberOfTimber;
        _numberOfMarble.Change -= DisplayNumberOfMarble;
    }

    private void Start()
    {
        const float PlaneScale = 10;
        _maxCountCollectorBots = 3;
        _freeResources = new List<BaseResource>();
        _resourcesPlannedForCollection = new List<BaseResource>();
        _scanningArea = new Vector3(_map.localScale.x * PlaneScale, _map.localScale.y * PlaneScale, _map.localScale.z * PlaneScale);
        _poolOfWorkingCollectorBots = new List<CollectorBot>();
        _poolOfIdleCollectorBots = new List<CollectorBot>();

        StartCoroutine(ResourceScanning());
        StartCoroutine(StartInitialization());
        DisplayNumberOfFood(0);
        DisplayNumberOfTimber(0);
        DisplayNumberOfMarble(0);
    }

    public void StoreResource(ResourceType resourceType)
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
                _numberOfFood.Value++;
                break;

            case ResourceType.Timber:
                _numberOfTimber.Value++;
                break;

            case ResourceType.Marble:
                _numberOfMarble.Value++;
                break;
        }
    }

    private void DisplayNumberOfFood(int number) => _displayNumberOfFood.text = "Food: " + number.ToString();
    private void DisplayNumberOfTimber(int number) => _displayNumberOfTimber.text = "Timber: " + number.ToString();
    private void DisplayNumberOfMarble(int number) => _displayNumberOfMarble.text = "Marble: " + number.ToString();

    private void OnCollectorBotTaskCompleted(CollectorBot bot)
    {
        _poolOfWorkingCollectorBots.Remove(bot);
        _poolOfIdleCollectorBots.Add(bot);
    }

    private void FindFreeResources()
    {
        Collider[] hits = Physics.OverlapBox(Vector3.zero, _scanningArea);

        foreach (Collider hit in hits)
            if (hit.TryGetComponent(out BaseResource resource))
                if (_freeResources.Contains(resource) == false)
                    if (_resourcesPlannedForCollection.Contains(resource) == false)
                        _freeResources.Add(resource);
    }

    private IEnumerator ResourceScanning()
    {
        const float Delay = 2.0f;
        bool isWork = true;

        while (isWork)
        {
            yield return new WaitForSeconds(Delay);
            FindFreeResources();

            if (_freeResources.Count > 0 && _poolOfIdleCollectorBots.Count > 0)
                DistributeCollectionTasks();
        }
    }

    private void DistributeCollectionTasks()
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

    private IEnumerator StartInitialization()
    {
        var delay = new WaitForSeconds(1f);

        for (int i = 0; i < _maxCountCollectorBots; i++)
        {
            yield return delay;
            CreateCollectorBot();
        }
    }
}
