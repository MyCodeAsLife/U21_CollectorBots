using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CollectorBot : MonoBehaviour
{
    [SerializeField] private Transform _resourceAttachmentPoint;
    [SerializeField] private BaseResource _resource;
    [SerializeField] private Slider _progressBar;
    [SerializeField] private float _speed;

    private Vector3 _targetPoint;
    private Coroutine _moving;
    private MainBase _base;
    private float _durationOfCollecting;
    private bool _haveCollectedResource;
    private bool _isWork;

    public event Action<CollectorBot> TaskCompleted;

    private void OnDisable()
    {
        if (_moving != null)
            StopCoroutine(_moving);
    }

    private void Start()
    {
        _speed = 7f;
        _resource = null;
        _durationOfCollecting = 5f;
    }

    public void GoTo(Vector3 point)
    {
        _targetPoint = point;
        _moving = StartCoroutine(Moving());
    }

    public void SetCollectionTask(BaseResource resource)
    {
        if (_haveCollectedResource)
            StoreResource();

        _resource = resource;
        _targetPoint = resource.transform.position;
        _moving = StartCoroutine(Moving());
    }

    public void SetBaseAffiliation(MainBase mainBase)
    {
        _base = mainBase;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_resource != null && other.TryGetComponent<BaseResource>(out var resource))
        {
            if (resource == _resource)
            {
                if (_moving != null)
                {
                    StopCoroutine(_moving);
                    _moving = null;
                    StartCoroutine(Collecting());
                }
            }
        }
        else if (other.TryGetComponent<MainBase>(out var mainBase) && _haveCollectedResource)
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
        _haveCollectedResource = false;
        _base.StoreResource(_resource.ResourceType);
        _resource.Delete();
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

            if (Vector3.Distance(transform.position, _targetPoint) < 0.1f)
                _isWork = false;
        }

        TaskCompleted?.Invoke(this);
    }

    private IEnumerator Collecting()
    {
        float timer = 0;
        _progressBar.gameObject.SetActive(true);

        while (timer < _durationOfCollecting)
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            _progressBar.value = timer / _durationOfCollecting;
        }

        _progressBar.gameObject.SetActive(false);
        _haveCollectedResource = true;
        _resource.transform.SetParent(this.transform);
        _resource.transform.position = _resourceAttachmentPoint.transform.position;
        GoTo(_base.transform.position);
    }
}