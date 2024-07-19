using System;
using System.Collections;
using UnityEngine;

public class CollectorBot : MonoBehaviour
{
    [SerializeField] private Transform _resourceAttachmentPoint;
    [SerializeField] private BaseResource _resource;
    [SerializeField] private float _speed;

    private CollectedResource _collectedResource;
    private Vector3 _targetPoint;
    private Coroutine _moving;
    private MainBase _base;
    private bool _isWork;

    public event Action<CollectorBot> TaskCompleted;

    public float DurationOfCollecting { get; private set; }

    private void Start()
    {
        _speed = 7f;
        _resource = null;
        DurationOfCollecting = 5f;
    }

    public void GoTo(Vector3 point)
    {
        if (_isWork)
            _resource = null;

        _targetPoint = point;
        _moving = StartCoroutine(Moving());
    }

    public void SetCollectionTask(BaseResource resource)
    {
        if (_collectedResource != null)
            StoreResource();

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

    private float GetDistanceToTarget() => Vector3.Distance(transform.position, _targetPoint);

    private void OnTriggerEnter(Collider other)
    {
        float distanceToTarget = GetDistanceToTarget();

        if (_resource != null && other.TryGetComponent<BaseResource>(out var resource))
        {
            if (resource == _resource)
            {
                if (_moving != null)
                {
                    StopCoroutine(_moving);
                    _moving = null;
                    resource.TryStartCollecting(this);
                }
            }
        }
        else if (other.TryGetComponent<MainBase>(out var mainBase) && _collectedResource != null)
        {
            if (mainBase == _base)
            {
                StopCoroutine(_moving);
                _moving = null;
                StoreResource();
                TaskCompleted?.Invoke(this);
            }
        }
    }

    private void StoreResource()
    {
        _base.StoreResource(_collectedResource.Type);
        Destroy(_collectedResource.gameObject);
        _collectedResource = null;
    }

    private IEnumerator Moving()
    {
        _isWork = true;
        _targetPoint.y = 1;

        while (_isWork)
        {
            yield return null;
            transform.LookAt(_targetPoint);
            transform.position = Vector3.MoveTowards(transform.position, _targetPoint, _speed * Time.deltaTime);

            if (GetDistanceToTarget() < 0.1f)
                _isWork = false;
        }

        TaskCompleted?.Invoke(this);
    }
}