using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBase : MonoBehaviour
{
    [SerializeField] private Transform _map;
    [SerializeField] private Transform _gatheringPoint;             // ����� ����� ����������� ������

    private CollectorBot _prefabCollectorBot;                       // �������� � ��������� �������, ������� ����� �������� �� ����� ������     +++
    private ObjectPool<CollectorBot> _poolCollectorBots;            // �������� � ��������� �������, ������� ����� �������� �� ����� ������     +++
    //private Explosion _prefabExplosion;                           // ����� �� ������� U20, ���������� ������ �� ���� ������ � �����
    private float _scanningRadius;
    private int _maxCountCollectorBots;

    private IList<BaseResource> _freeResources;
    private IList<BaseResource> _resourcesPlannedForCollection;
    private IList<CollectorBot> _poolOfWorkingCollectorBots;
    private IList<CollectorBot> _poolOfIdleCollectorBots;

    //public event Action<CollectorBot> TaskCompleted;

    private void Start()    //++
    {
        _maxCountCollectorBots = 3;
        _prefabCollectorBot = Resources.Load<CollectorBot>("Prefabs/CollectorBot");
        //_prefabExplosion = Resources.Load<Explosion>("Prefabs/Explosion");      // ����� �� ������� U20
        _freeResources = new List<BaseResource>();
        _resourcesPlannedForCollection = new List<BaseResource>();
        _scanningRadius = _map.localScale.x > _map.localScale.z ? _map.localScale.x : _map.localScale.z;
        _scanningRadius *= 5;   // ���������� ����� (���-�� unit/2 � ����� "����" �����. ������� ����������.
        _poolCollectorBots = new ObjectPool<CollectorBot>(_prefabCollectorBot, Create, Enable, Disable);
        _poolOfWorkingCollectorBots = new List<CollectorBot>();
        _poolOfIdleCollectorBots = new List<CollectorBot>();

        StartCoroutine(ResourceScanning());
        StartCoroutine(StartInitialization());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetResource(ResourceType resourceType)              // +++++++++++++++++++++++++++++++++++
    {
        for (int i = 0; i < _resourcesPlannedForCollection.Count; ++i)
            if (_resourcesPlannedForCollection[i].ResourceType == resourceType)
            {
                _resourcesPlannedForCollection.RemoveAt(i);
                break;
            }
    }

    private CollectorBot Create(CollectorBot prefab)        // ���������� ����� �������� (����� ������� �����?)
    {
        var obj = Instantiate<CollectorBot>(prefab);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Enable(CollectorBot bot)
    {
        bot.gameObject.SetActive(true);
        bot.TaskCompleted += OnCollectorBotTaskCompleted;
    }

    public void OnCollectorBotTaskCompleted(CollectorBot bot)
    {
        _poolOfWorkingCollectorBots.Remove(bot);
        _poolOfIdleCollectorBots.Add(bot);
    }

    private void Disable(CollectorBot bot)
    {
        bot.TaskCompleted -= OnCollectorBotTaskCompleted;
        bot.gameObject.SetActive(false);
    }

    private void FindFreeResources()         // �������������(������ ��� ������ �� ����������) ��� ����������� � �������� ResourceScanning
    {
        Collider[] hits = Physics.OverlapSphere(Vector3.zero, _scanningRadius);
        //List<BaseResource> allResources = new List<BaseResource>();

        foreach (Collider hit in hits)
            if (hit.TryGetComponent(out BaseResource resource))
                if (_freeResources.Contains(resource) == false)                             // �������� ��� ��������� �������           ++++++++
                    if (_resourcesPlannedForCollection.Contains(resource) == false)         // �������� ��� ��������� �������           ++++++++
                        _freeResources.Add(resource);

        //Debug.Log("Free resources - " + _freeResources.Count + ". Planed recources - " + _resourcesPlannedForCollection.Count);     //--------------
    }

    private IEnumerator ResourceScanning()          // �������� ��������� ��� ����� ���������� �������?
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

    private void DistributeCollectionTasks()                // ������������� ������� �� ���� ��������
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

    private IEnumerator StartInitialization()      // 2 ���� ����� (������� � ���������).
    {
        var delay = new WaitForSeconds(1f);

        for (int i = 0; i < _maxCountCollectorBots; i++)
        {
            yield return delay;

            var collectorBot = _poolCollectorBots.Get();
            collectorBot.transform.position = transform.position;
            collectorBot.gameObject.SetActive(true);
            collectorBot.SetBaseAffiliation(this);
            collectorBot.GoTo(_gatheringPoint.position);

            _poolOfWorkingCollectorBots.Add(collectorBot);
        }
    }

    private void ShowScanningResultInDebug()        // ---- ��� ������������
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
