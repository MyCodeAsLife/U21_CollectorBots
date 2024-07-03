using UnityEngine;

public class CollectorBot : MonoBehaviour
{
    //[SerializeField] private Vector3 _targetPoint;          //++
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;                  //++

    private void Start()
    {
        //_targetPoint = new Vector3(4f, 1f, 6f);
        _speed = 7f;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 targetPosition = _target.position;
        targetPosition.y = 1;
        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speed * Time.deltaTime);        // Хороший вариант
    }

    public void GoFor(Vector3 point)        // SetMovementPoint
    {
        //_targetPoint = point;

        // Запуск корутины на движение?
    }
}
