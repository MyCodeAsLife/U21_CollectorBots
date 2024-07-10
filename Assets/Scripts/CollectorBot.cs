using System.Collections;
using UnityEngine;

public class CollectorBot : MonoBehaviour
{
    //[SerializeField] private Vector3 _targetPoint;            //++
    [SerializeField] private BaseResource _target;              // Задается базой, при отправке на сбор
    [SerializeField] private float _speed;                      //++
    [SerializeField] private Transform _resourceAttachmentPoint;

    private Coroutine _moving;

    private CollectedResource _collectedResource;

    private void Start()
    {
        //_targetPoint = new Vector3(4f, 1f, 6f);
        _speed = 7f;
        _moving = StartCoroutine(Moving());
    }

    //private void Update()
    //{
    //    //Move();
    //}

    private void Move(Vector3 targetPosition)
    {
        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);        // Хороший вариант
    }

    public void GoFor(Vector3 point)        // SetMovementPoint
    {
        //_targetPoint = point;

        // Запуск корутины на движение?

        //По приходу к цели, запускать сбор ресурса

    }

    public void SetCollectedResource(CollectedResource resource)
    {
        _collectedResource = Instantiate<CollectedResource>(resource, _resourceAttachmentPoint.transform.position, Quaternion.identity, transform);
        _collectedResource.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        float distanceToTarget = Vector3.Distance(transform.position, other.transform.position);

        if (distanceToTarget < 3 && other.TryGetComponent<BaseResource>(out var resource))   // Магическое число
        {
            Debug.Log("Distance to resource: " + distanceToTarget);                                     // +++++
            if (resource.ResourceType == _target.ResourceType)
            {
                if (_moving != null)
                {
                    StopCoroutine(_moving);
                    _moving = null;
                    resource.TryStartHarvest(this);         // Вставить словие, на проверку возможности сбора ресурса
                }
            }
        }
    }

    //private void OnCollisionEnter(Collision other)
    //{
    //    float distanceToTarget = Vector3.Distance(transform.position, other.transform.position);        // Высчитывать только если столкнулся с ресурсом

    //    Debug.Log("Object name: " + other.gameObject.name);

    //    if (/*distanceToTarget < 2*/ other.gameObject.tag == "Resource")   // Магическое число и слово
    //    {
    //        Debug.Log("Distance to resource: " + distanceToTarget);                                     // +++++
    //        //if (resource.ResourceType == _target.ResourceType)
    //        //{
    //        //    StopCoroutine(Moving());
    //        //    resource.TryStartHarvest(this);         // Вставить словие, на проверку возможности сбора ресурса
    //        //}
    //    }
    //}

    private IEnumerator Moving()
    {
        bool isWork = true;

        while (isWork)
        {
            yield return null;

            Vector3 targetPosition = _target.transform.position;
            targetPosition.y = 1;
            Move(targetPosition);
        }
    }
}
