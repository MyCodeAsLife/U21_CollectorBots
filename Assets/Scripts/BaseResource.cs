using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseResource : MonoBehaviour
{
    // ������ �� ������ ��� �����
    [SerializeField] private Slider _progressBar;

    private CollectorBot _collectorBot;

    protected CollectedResource _prefabCollectedResource;

    // ����� �� ���� �������, ��������� �������
    public event Action<BaseResource> Harvest;

    public ResourceType ResourceType { get; private set; }

    public BaseResource(ResourceType resourceType)
    {
        ResourceType = resourceType;
    }

    private void OnEnable()
    {
        _collectorBot = null;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    protected abstract CollectedResource GetCollectedResource();

    // ����� ���������� ����� ���������, �������� ������ �� ����, �������� �������� �� ���� �������
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

    // �������� �� ���� �������
    private IEnumerator Collecting(float harvestDuration)
    {
        float duration = 0;     // ����� �������� ��� ��� � ������
        _progressBar.gameObject.SetActive(true);

        while (duration < harvestDuration)
        {
            yield return new WaitForEndOfFrame();

            duration += Time.deltaTime;
            _progressBar.value = duration / harvestDuration;
        }
        // �� ��������� ���������� ��������� ���� �������� � �������� ����� �� ���� �������
        // ����� �������� ������� ����� ������ �������, � ���� ������ ���������� ������� � ����
        _progressBar.gameObject.SetActive(false);
        Harvest?.Invoke(this);
        _collectorBot.SetCollectedResource(GetCollectedResource());
    }
}
