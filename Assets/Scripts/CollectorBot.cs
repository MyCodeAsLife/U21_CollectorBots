using System;
using System.Collections;
using UnityEngine;

public class CollectorBot : MonoBehaviour
{
    //[SerializeField] private Vector3 _targetPoint;            //++
    [SerializeField] private BaseResource _resource;              // �������� �����, ��� �������� �� ����
    [SerializeField] private float _speed;                      //++
    [SerializeField] private Transform _resourceAttachmentPoint;
    //private Transform _target;              // �������� �����, ��� �������� �� ����

    private Coroutine _moving;

    private CollectedResource _collectedResource;

    public event Action<CollectorBot> TaskCompleted;

    private void Start()
    {
        //_targetPoint = new Vector3(4f, 1f, 6f);
        _speed = 7f;
        //_moving = StartCoroutine(Moving());                     // ++++
    }

    //private void Update()
    //{
    //    //Move();
    //}

    private float GetDistanceToTarget(Vector3 target) => Vector3.Distance(transform.position, target);

    public void GoTo(Vector3 point)        // SetMovementPoint
    {
        //_targetPoint = point;
        //if (_moving != null)                // ���������� �� �������� �� ���������, �������� ������ ��������� ???
        //    StopCoroutine(_moving);

        _moving = StartCoroutine(Moving(point));
        // ������ �������� �� ��������?

        //�� ������� � ����, ��������� ���� �������

    }

    public void SetResourseToCollect(BaseResource resource)
    {
        _resource = resource;
        //_targetPoint = resource.transform.position;
        GoTo(resource.transform.position);         // ???
    }

    public void SetCollectedResource(CollectedResource resource)
    {
        _collectedResource = Instantiate<CollectedResource>(resource, _resourceAttachmentPoint.transform.position, Quaternion.identity, transform);
        _collectedResource.gameObject.SetActive(true);
    }

    private void Move(Vector3 targetPosition)
    {
        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);        // ������� �������
    }

    private void OnTriggerEnter(Collider other)
    {
        //float distanceToTarget = Vector3.Distance(transform.position, other.transform.position);

        if (GetDistanceToTarget(other.transform.position) < 3 && other.TryGetComponent<BaseResource>(out var resource))   // ���������� �����
        {
            if (resource.ResourceType == _resource.ResourceType)
            {
                if (_moving != null)
                {
                    StopCoroutine(_moving);
                    _moving = null;
                    resource.TryStartHarvest(this);         // �������� ������, �� �������� ����������� ����� �������
                }
            }
        }
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    float distanceToTarget = Vector3.Distance(transform.position, other.transform.position);        // ����������� ������ ���� ���������� � ��������

    //    Debug.Log("Object name: " + other.gameObject.name);

    //    if (/*distanceToTarget < 2*/ other.gameObject.tag == "Resource")   // ���������� ����� � �����
    //    {
    //        Debug.Log("Distance to resource: " + distanceToTarget);                                     // +++++
    //        //if (resource.ResourceType == _target.ResourceType)
    //        //{
    //        //    StopCoroutine(Moving());
    //        //    resource.TryStartHarvest(this);         // �������� ������, �� �������� ����������� ����� �������
    //        //}
    //    }
    //}

    private IEnumerator Moving(Vector3 target)
    {
        bool isWork = true;
        target.y = 1;

        while (isWork)
        {
            yield return null;
            Move(target);

            if (GetDistanceToTarget(target) < 0.1f)
                isWork = false;
        }

        TaskCompleted?.Invoke(this);
    }
}
