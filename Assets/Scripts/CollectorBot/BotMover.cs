using System;
using System.Collections;
using UnityEngine;

public class BotMover : MonoBehaviour
{
    private Coroutine _moving;
    private Vector3 _targetPoint;
    private bool _isWork;
    private float _moveSpeed;

    public event Action MoveCompleted;

    public bool IsMoving => _moving != null;

    private void OnDisable()
    {
        Stop();
    }

    private void Start()
    {
        _moveSpeed = 7f;
    }

    public void Move(Vector3 point)
    {
        _targetPoint = point;
        _moving = StartCoroutine(Moving());
    }

    public void Stop()
    {
        if (IsMoving)
        {
            StopCoroutine(_moving);
            _moving = null;
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
            transform.position = Vector3.MoveTowards(transform.position, _targetPoint, _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetPoint) < 0.1f)
                _isWork = false;
        }

        MoveCompleted?.Invoke();
    }
}
