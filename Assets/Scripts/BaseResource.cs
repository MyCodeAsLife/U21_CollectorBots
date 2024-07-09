using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BaseResource : MonoBehaviour
{
    // ссылка на статус бар сбора
    [SerializeField] private Slider _progressBar;

    private CollectorBot _collectorBot;

    // Евент на сбор ресурса, подписать спавнер
    public event Action<BaseResource> Harvest;

    public ResourceType ResourceType { get; private set; }

    public BaseResource(ResourceType resourceType)
    {
        ResourceType = resourceType;
    }

    // метод вызываемый ботом сборщиком, получает ссылку на бота, стартует корутину на сбор ресурса
    public bool TryStartHarvest(CollectorBot collectorBot)
    {
        if (_collectorBot == null)
        {
            _collectorBot = collectorBot;
            StartCoroutine(Harvesting(5f));         // Магическое число, брать у бота
            return true;
        }

        return false;
    }

    // корутина на сбор ресурса
    private IEnumerator Harvesting(float harvestDuration)
    {
        float duration = 0;     // Можно обойтись без нее в теории

        // заполняет статус бар
        while (duration < harvestDuration)
        {
            yield return new WaitForEndOfFrame();

            duration += Time.deltaTime;
            _progressBar.value = duration / harvestDuration;
            Debug.Log(duration);
        }

        // по окончанию возвращает результат боту сборщику и вызывает евент на сбор ресурса
        // нужна болванка которую будет тащить сборщик, а этот объект необходимо вернуть в пулл
        
    }
}
