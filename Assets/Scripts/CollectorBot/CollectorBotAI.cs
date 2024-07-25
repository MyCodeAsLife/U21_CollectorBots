using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CollectorBotAI : MonoBehaviour
{
    private BotMover _mover;
    private Resource _resource;
    private MainBaseAI _mainBase;
    private SingleReactiveProperty<float> _collectionProgress = new();

    private Vector3 _resourceAttachmentPoint;
    private bool _haveCollectedResource;
    private float _durationOfCollecting;
    private int _resourceMask;
    private int _interactableObjectMask;

    public event Action CollectingStarted;
    public event Action CollectingFinished;
    public event Action<CollectorBotAI> TaskCompleted;

    private void Awake()
    {
        _mover = transform.AddComponent<BotMover>();
        _resourceMask = LayerMask.NameToLayer("Resource");
        _interactableObjectMask = LayerMask.NameToLayer("Interactable");
    }

    private void OnEnable()
    {
        _mover.MoveCompleted += OnMoveCompleted;
    }

    private void OnDisable()
    {
        if (_mover.IsMoving)
            _mover.Stop();

        _mover.MoveCompleted -= OnMoveCompleted;
    }

    private void Start()
    {
        _resource = null;
        _durationOfCollecting = 5f;
        _resourceAttachmentPoint = new Vector3(0, transform.localScale.y, 0);
    }

    public void GoTo(Vector3 point)
    {
        _mover.Move(point);
    }

    public void SetCollectionTask(Resource resource)
    {
        if (_haveCollectedResource)
            StoreResource();

        _resource = resource;
        _mover.Move(_resource.transform.position);
    }

    public void SetBaseAffiliation(MainBaseAI mainBase)
    {
        _mainBase = mainBase;
    }

    public void SubscribeToCollectionProgress(Action<float> func)
    {
        _collectionProgress.Change += func;
    }

    public void UnsubscribeToCollectionProgress(Action<float> func)
    {
        _collectionProgress.Change -= func;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_resource != null && other.gameObject.layer == _resourceMask)
        {
            if (other.GetComponent<Resource>() == _resource)
            {
                _mover.Stop();
                StartCoroutine(Collecting());
            }
        }
        else if (other.gameObject.layer == _interactableObjectMask && _haveCollectedResource)
        {
            if (other.GetComponent<MainBaseAI>() == _mainBase)
            {
                _mover.Stop();
                StoreResource();
                OnMoveCompleted();
            }
        }
    }

    private void StoreResource()
    {
        _haveCollectedResource = false;
        _mainBase.StoreResource(_resource.ResourceType);
        _resource.Delete();
    }

    private void OnMoveCompleted()
    {
        TaskCompleted?.Invoke(this);
    }

    private IEnumerator Collecting()
    {
        CollectingStarted?.Invoke();
        float timer = 0;

        while (timer < _durationOfCollecting)
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            _collectionProgress.Value = timer / _durationOfCollecting;
        }

        CollectingFinished?.Invoke();
        _haveCollectedResource = true;
        _resource.transform.SetParent(transform);
        _resource.transform.localPosition = _resourceAttachmentPoint;
        _mover.Move(_mainBase.transform.position);
    }
}