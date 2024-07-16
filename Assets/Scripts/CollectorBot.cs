using System;
using System.Collections;
using UnityEngine;

public class CollectorBot : MonoBehaviour
{
    [SerializeField] private Transform _resourceAttachmentPoint;    // ����� "���������" �� ��������, ���������� �������
    [SerializeField] private BaseResource _resource;                // �������� �����, ��� �������� �� ����
    [SerializeField] private float _speed;

    private CollectedResource _collectedResource;                   // ��������� ������, ��� ��������� �����������
    private Vector3 _targetPoint;
    private bool _isWork;
    private Coroutine _moving;

    public MainBase Base { get; private set; }                  // ���������� � ������� ��������� ����

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
        GoTo(Base.transform.position);
    }

    public void SetBaseAffiliation(MainBase mainBase)
    {
        Base = mainBase;
    }

    private void OnTriggerEnter(Collider other)
    {
        float distanceToTarget = GetDistanceToTarget();

        if (_resource != null && /*distanceToTarget < 4 &&*/ other.TryGetComponent<BaseResource>(out var resource))   // ���������� �����
        {
            if (resource == _resource)            // ���������� ������
            {
                if (_moving != null)
                {
                    StopCoroutine(_moving);
                    _moving = null;
                    resource.TryStartCollecting(this);         // �������� ������, �� �������� ����������� ����� �������
                }
            }
        }
        else if (/*distanceToTarget < 4 &&*/ other.TryGetComponent<MainBase>(out var mainBase) && _collectedResource != null)   // ���������� �����
        {
            if (mainBase == Base)
            {
                StopCoroutine(_moving);
                Base.SetResource(_collectedResource.Type);
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
            transform.position = Vector3.MoveTowards(transform.position, _targetPoint, _speed * Time.deltaTime);        // ������� �������

            if (GetDistanceToTarget() < 0.1f)
                _isWork = false;
        }

        TaskCompleted?.Invoke(this);
    }
}