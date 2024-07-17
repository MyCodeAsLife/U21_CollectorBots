using System;
using System.Collections;
using UnityEngine;

public class CollectorBot : MonoBehaviour
{
    [SerializeField] private Transform _resourceAttachmentPoint;    // Место "крепления" на сборщике, собранного ресурса
    [SerializeField] private BaseResource _resource;                // Задается базой, при отправке на сбор
    [SerializeField] private float _speed;

    private CollectedResource _collectedResource;                   // Собранный ресурс, его визуально отображение
    private Vector3 _targetPoint;
    private bool _isWork;
    private Coroutine _moving;
    private MainBase _base;

    public event Action<CollectorBot> TaskCompleted;

    private void Start()
    {
        _speed = 7f;
        _resource = null;
    }

    private float GetDistanceToTarget() => Vector3.Distance(transform.position, _targetPoint);

    public void GoTo(Vector3 point)        // SetMovementPoint
    {
        if (_isWork)
            _resource = null;

        _targetPoint = point;
        _moving = StartCoroutine(Moving());
    }

    public void SetCollectionTask(BaseResource resource)
    {
        _resource = resource;
        _targetPoint = resource.transform.position;
        _moving = StartCoroutine(Moving());
    }

    public void SetCollectedResource(CollectedResource resource)
    {
        _collectedResource = Instantiate<CollectedResource>(resource, _resourceAttachmentPoint.transform.position, Quaternion.identity, transform);
        _collectedResource.gameObject.SetActive(true);
        GoTo(_base.transform.position);
    }

    public void SetBaseAffiliation(MainBase mainBase)
    {
        _base = mainBase;
    }

    private void OnTriggerEnter(Collider other)
    {
        float distanceToTarget = GetDistanceToTarget();

        if (_resource != null && other.TryGetComponent<BaseResource>(out var resource))
        {
            if (resource == _resource)            // Сравнивать ресурс
            {
                if (_moving != null)
                {
                    StopCoroutine(_moving);
                    _moving = null;
                    resource.TryStartCollecting(this);         // Вставить словие, на проверку возможности сбора ресурса
                }
            }
        }
        else if (other.TryGetComponent<MainBase>(out var mainBase) && _collectedResource != null)
        {
            if (mainBase == _base)
            {
                StopCoroutine(_moving);
                _base.SetResource(_collectedResource.Type);
                Destroy(_collectedResource.gameObject);
                _collectedResource = null;
                TaskCompleted?.Invoke(this);
            }
        }
    }

    private IEnumerator Moving()
    {
        _isWork = true;
        _targetPoint.y = 1;

        while (_isWork)
        {
            yield return null;
            transform.LookAt(_targetPoint);
            transform.position = Vector3.MoveTowards(transform.position, _targetPoint, _speed * Time.deltaTime);        // Хороший вариант

            if (GetDistanceToTarget() < 0.1f)
                _isWork = false;
        }

        TaskCompleted?.Invoke(this);
    }
}