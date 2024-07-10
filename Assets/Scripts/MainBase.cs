using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBase : MonoBehaviour
{
    [SerializeField] private Transform _map;

    private CollectorBot _prefabCollectorBot;
    private ObjectPool<CollectorBot> _poolCollectorBots;
    //private Explosion _prefabExplosion;         // ����� �� ������� U20, ���������� ������ �� ���� ������ � �����
    private float _scanningRadius;
    private List<BaseResource> _resources;

    private void Start()    //++
    {
        _prefabCollectorBot = Resources.Load<CollectorBot>("Prefabs/CollectorBot");
        //_prefabExplosion = Resources.Load<Explosion>("Prefabs/Explosion");      // ����� �� ������� U20
        _resources = new List<BaseResource>();
        _scanningRadius = _map.localScale.x > _map.localScale.z ? _map.localScale.x : _map.localScale.z;
        _scanningRadius *= 5;   // ���������� ����� (���-�� unit/2 � ����� "����" �����.
        _poolCollectorBots = new ObjectPool<CollectorBot>(_prefabCollectorBot, Create, Enable, Disable);

        StartCoroutine(ResourceScanning());
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

    private void Enable(CollectorBot obj)
    {
        obj.gameObject.SetActive(true);
        //obj.Harvest += OnResourceHarvest;
    }

    private void Disable(CollectorBot obj)
    {
        //obj.Harvest -= OnResourceHarvest;
        obj.gameObject.SetActive(false);
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

    private void StartInitialization()      // 2 ���� ����� (������� � ���������).
    {

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
