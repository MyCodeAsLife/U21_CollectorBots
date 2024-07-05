using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBase : MonoBehaviour
{
    [SerializeField] private Transform _map;
    private ObjectPool<CollectorBot> _poolCollectorBots;
    //private Explosion _prefabExplosion;         // Взять из проекта U20, передовать ссылку на этот эффект в мобов
    private float _scanningRadius;
    private List<BaseResource> _resources;

    private void Start()    //++
    {
        //_prefabExplosion = Resources.Load<Explosion>("Prefabs/Explosion");      // Взять из проекта U20
        _resources = new List<BaseResource>();
        _scanningRadius = _map.localScale.x > _map.localScale.z ? _map.localScale.x : _map.localScale.z;
        _scanningRadius *= 5;   // Магическое число

        StartCoroutine(ResourceScanning());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void GetResources()         // Переименовать(потому как ничего не возвращает) или переместить в корутину ResourceScanning
    {
        Collider[] hits = Physics.OverlapSphere(Vector3.zero, _scanningRadius);
        List<BaseResource> resources = new List<BaseResource>();

        foreach (Collider hit in hits)
            if (hit.TryGetComponent(out BaseResource resource))
                resources.Add(resource);

        _resources = resources;
        //var effect = Instantiate(_prefabExplosion);           // Взять из проекта U20, воспроизводить при подборе ресурса
        //effect.transform.position = position;
    }

    private IEnumerator ResourceScanning()          // Работать постоянно или когда закончатся ресурсы?
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

    private void ShowScanningResultInDebug()        // Для тестирования
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
