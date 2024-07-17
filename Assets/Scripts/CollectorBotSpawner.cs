using UnityEngine;

public class CollectorBotSpawner : MonoBehaviour
{
    private ObjectPool<CollectorBot> _poolCollectorBots;
    private CollectorBot _prefabCollectorBot;

    private void OnEnable()
    {
        _prefabCollectorBot = Resources.Load<CollectorBot>("Prefabs/CollectorBot");
        _poolCollectorBots = new ObjectPool<CollectorBot>(_prefabCollectorBot, Create, Enable, Disable);
    }

    public CollectorBot GetCollectorBot() => _poolCollectorBots.Get();

    private CollectorBot Create(CollectorBot prefab)
    {
        var obj = Instantiate<CollectorBot>(prefab);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Enable(CollectorBot bot)
    {
        bot.gameObject.SetActive(true);
    }


    private void Disable(CollectorBot bot)
    {
        bot.gameObject.SetActive(false);
    }
}
