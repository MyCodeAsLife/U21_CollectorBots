using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBase : MonoBehaviour
{
    [SerializeField] private Transform _map;
    [SerializeField] private Transform _gatheringPoint;

    private CollectorBot _prefabCollectorBot;
    private ObjectPool<CollectorBot> _poolCollectorBots;            // �������� � ��������� �������, ������� ����� �������� �� ����� ������
    //private Explosion _prefabExplosion;                           // ����� �� ������� U20, ���������� ������ �� ���� ������ � �����
    private float _scanningRadius;
    private int _maxCountCollectorBots;
    private List<BaseResource> _resources;
    private IList<CollectorBot> _poolOfWorkingCollectorBots;
    private IList<CollectorBot> _poolOfIdleCollectorBots;

    private void Start()    //++
    {
        _maxCountCollectorBots = 3;
        _prefabCollectorBot = Resources.Load<CollectorBot>("Prefabs/CollectorBot");
        //_prefabExplosion = Resources.Load<Explosion>("Prefabs/Explosion");      // ����� �� ������� U20
        _resources = new List<BaseResource>();
        _scanningRadius = _map.localScale.x > _map.localScale.z ? _map.localScale.x : _map.localScale.z;
        _scanningRadius *= 5;   // ���������� ����� (���-�� unit/2 � ����� "����" �����.
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

    private CollectorBot Create(CollectorBot prefab)        // ���������� ����� �������� (����� ������� �����?)
    {
        var obj = Instantiate<CollectorBot>(prefab);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Enable(CollectorBot bot)
    {
        bot.gameObject.SetActive(true);
        bot.TaskCompleted += OnTaskCompleted;
    }

    private void OnTaskCompleted(CollectorBot bot)
    {
        _poolOfWorkingCollectorBots.Remove(bot);
        _poolOfIdleCollectorBots.Add(bot);
    }

    private void Disable(CollectorBot bot)
    {
        bot.TaskCompleted -= OnTaskCompleted;
        bot.gameObject.SetActive(false);
    }

    private void GetResources()         // �������������(������ ��� ������ �� ����������) ��� ����������� � �������� ResourceScanning
    {
        Collider[] hits = Physics.OverlapSphere(Vector3.zero, _scanningRadius);
        List<BaseResource> resources = new List<BaseResource>();

        foreach (Collider hit in hits)
            if (hit.TryGetComponent(out BaseResource resource))
                resources.Add(resource);

        _resources = resources;
        //var effect = Instantiate(_prefabExplosion);           // ����� �� ������� U20, �������������� ��� ������� �������
        //effect.transform.position = position;
    }

    private IEnumerator ResourceScanning()          // �������� ��������� ��� ����� ���������� �������?
    {
        float Delay = 2.0f;     // const?
        bool isWork = true;

        while (isWork)
        {
            yield return new WaitForSeconds(Delay);

            GetResources();
            ShowScanningResultInDebug();
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
            collectorBot.GoTo(_gatheringPoint.position);

            _poolOfWorkingCollectorBots.Add(collectorBot);
        }
    }

    private void ShowScanningResultInDebug()        // ++++ ��� ������������
    {
        int foodCount = 0;
        int timberCount = 0;
        int marbleCount = 0;

        for (int i = 0; i < _resources.Count; i++)
        {
            switch (_resources[i].ResourceType)
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
