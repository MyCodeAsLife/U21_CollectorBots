using System;
using System.Collections;
using UnityEngine;

public class CollectorBotPresenter : MonoBehaviour
{
    private Coroutine _moving;
    private CollectorBotModel _model;

    public event Action CollectingStarted;
    public event Action CollectingFinished;
    public event Action<CollectorBotPresenter> TaskCompleted;

    private void OnEnable()
    {
        _model = new CollectorBotModel();
    }

    private void OnDisable()
    {
        if (_moving != null)
            StopCoroutine(_moving);
    }

    private void Start()
    {
        _model.MainBaseSize = _model.MainBase.GetComponent<BoxCollider>().size.x + _model.MainBase.GetComponent<BoxCollider>().size.y;
        _model.MoveSpeed = 7f;
        _model.Resource = null;
        _model.DurationOfCollecting = 5f;
        _model.ResourceAttachmentPoint = new Vector3(0, transform.localScale.y, 0);
    }

    public void GoTo(Vector3 point)
    {
        _model.TargetPoint = point;
        _moving = StartCoroutine(Moving());
    }

    public void SetCollectionTask(BaseResource resource)
    {
        if (_model.HaveCollectedResource)
            StoreResource();

        _model.Resource = resource;
        _model.TargetPoint = resource.transform.position;
        _moving = StartCoroutine(Moving());
    }

    public void SetBaseAffiliation(MainBase mainBase)
    {
        _model.MainBase = mainBase;
    }

    public void SubscribeToCollectionProgress(Action<float> func)
    {
        _model.CollectionProgress.Change += func;
    }

    public void UnsubscribeToCollectionProgress(Action<float> func)
    {
        _model.CollectionProgress.Change -= func;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_model.Resource != null && other.TryGetComponent<BaseResource>(out var resource))
        {
            if (resource == _model.Resource)
            {
                if (_moving != null)
                {
                    StopCoroutine(_moving);
                    _moving = null;
                    StartCoroutine(Collecting());
                }
            }
        }
        else if (Vector3.Distance(transform.position, _model.MainBase.transform.position) < _model.MainBaseSize && _model.HaveCollectedResource)
        {
            StopCoroutine(_moving);
            _moving = null;
            StoreResource();
            TaskCompleted?.Invoke(this);
        }
    }

    private void StoreResource()
    {
        _model.HaveCollectedResource = false;
        _model.MainBase.StoreResource(_model.Resource.ResourceType);
        _model.Resource.Delete();
    }

    private IEnumerator Moving()
    {
        _model.IsWork = true;
        _model.TargetPoint.y = 1;

        while (_model.IsWork)
        {
            yield return null;
            transform.LookAt(_model.TargetPoint);
            transform.position = Vector3.MoveTowards(transform.position, _model.TargetPoint, _model.MoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _model.TargetPoint) < 0.1f)
                _model.IsWork = false;
        }

        TaskCompleted?.Invoke(this);
    }

    private IEnumerator Collecting()
    {
        CollectingStarted?.Invoke();
        float timer = 0;

        while (timer < _model.DurationOfCollecting)
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            _model.CollectionProgress.Value = timer / _model.DurationOfCollecting;
        }

        CollectingFinished?.Invoke();
        _model.HaveCollectedResource = true;
        _model.Resource.transform.SetParent(transform);
        _model.Resource.transform.localPosition = _model.ResourceAttachmentPoint;
        GoTo(_model.MainBase.transform.position);
    }
}