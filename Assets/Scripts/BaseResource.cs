using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BaseResource : MonoBehaviour
{
    // ������ �� ������ ��� �����
    [SerializeField] private Slider _progressBar;

    private CollectorBot _collectorBot;

    // ����� �� ���� �������, ��������� �������
    public event Action<BaseResource> Harvest;

    public ResourceType ResourceType { get; private set; }

    public BaseResource(ResourceType resourceType)
    {
        ResourceType = resourceType;
    }

    // ����� ���������� ����� ���������, �������� ������ �� ����, �������� �������� �� ���� �������
    public bool TryStartHarvest(CollectorBot collectorBot)
    {
        if (_collectorBot == null)
        {
            _collectorBot = collectorBot;
            StartCoroutine(Harvesting(5f));         // ���������� �����, ����� � ����
            return true;
        }

        return false;
    }

    // �������� �� ���� �������
    private IEnumerator Harvesting(float harvestDuration)
    {
        float duration = 0;     // ����� �������� ��� ��� � ������

        // ��������� ������ ���
        while (duration < harvestDuration)
        {
            yield return new WaitForEndOfFrame();

            duration += Time.deltaTime;
            _progressBar.value = duration / harvestDuration;
            Debug.Log(duration);
        }

        // �� ��������� ���������� ��������� ���� �������� � �������� ����� �� ���� �������
        // ����� �������� ������� ����� ������ �������, � ���� ������ ���������� ������� � ����
        
    }
}
