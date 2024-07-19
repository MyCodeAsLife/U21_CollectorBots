using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseResource : MonoBehaviour
{
    public event Action<BaseResource> Harvest;

    protected CollectedResource PrefabCollectedResource;

    [SerializeField] private Slider _collectingProgressBar;

    private CollectorBot _collectorBot;

    public BaseResource(ResourceType resourceType)
    {
        ResourceType = resourceType;
    }

    public ResourceType ResourceType { get; private set; }

    private void OnEnable()
    {
        _collectorBot = null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public bool TryStartCollecting(CollectorBot collectorBot)
    {
        if (_collectorBot == null)
        {
            _collectorBot = collectorBot;
            StartCoroutine(Collecting(_collectorBot.DurationOfCollecting));
            return true;
        }

        return false;
    }

    protected abstract CollectedResource GetCollectedResource();

    private IEnumerator Collecting(float durationOfCollecting)
    {
        float timer = 0;
        _collectingProgressBar.gameObject.SetActive(true);

        while (timer < durationOfCollecting)
        {
            yield return new WaitForEndOfFrame();

            timer += Time.deltaTime;
            _collectingProgressBar.value = timer / durationOfCollecting;
        }

        _collectingProgressBar.gameObject.SetActive(false);
        Harvest?.Invoke(this);
        _collectorBot.SetCollectedResource(GetCollectedResource());
    }
}
