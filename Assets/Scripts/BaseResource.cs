using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseResource : MonoBehaviour
{
    protected CollectedResource _prefabCollectedResource;

    [SerializeField] private Slider _collectingProgressBar;
    private CollectorBot _collectorBot;

    public event Action<BaseResource> Harvest;

    public ResourceType ResourceType { get; private set; }

    private void OnEnable()
    {
        _collectorBot = null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public BaseResource(ResourceType resourceType)
    {
        ResourceType = resourceType;
    }

    public bool TryStartCollecting(CollectorBot collectorBot)
    {
        if (_collectorBot == null)
        {
            _collectorBot = collectorBot;
            StartCoroutine(Collecting(5f));         // ���������� �����, ����� � ����
            return true;
        }

        return false;
    }

    protected abstract CollectedResource GetCollectedResource();

    private IEnumerator Collecting(float harvestDuration)
    {
        float duration = 0;     // ����� �������� ��� ��� � ������
        _collectingProgressBar.gameObject.SetActive(true);

        while (duration < harvestDuration)
        {
            yield return new WaitForEndOfFrame();

            duration += Time.deltaTime;
            _collectingProgressBar.value = duration / harvestDuration;
        }

        _collectingProgressBar.gameObject.SetActive(false);
        Harvest?.Invoke(this);
        _collectorBot.SetCollectedResource(GetCollectedResource());
    }
}
