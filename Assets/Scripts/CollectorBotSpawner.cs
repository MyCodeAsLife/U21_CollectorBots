using UnityEngine;

public class CollectorBotSpawner : MonoBehaviour
{
    private CollectorBot _prefabCollectorBot;
    private ObjectPool<CollectorBot> _poolCollectorBots;

    private void OnEnable()
    {
        _prefabCollectorBot = Resources.Load<CollectorBot>("Prefabs/CollectorBot");
        _poolCollectorBots = new ObjectPool<CollectorBot>(_prefabCollectorBot, Create, Enable, Disable);
    }

    private CollectorBot Create(CollectorBot prefab)        // Доработать спавн ресурсов (Общий базовый класс?)
    {
        var obj = Instantiate<CollectorBot>(prefab);
        obj.transform.SetParent(transform);

        return obj;
    }

    private void Enable(CollectorBot bot)
    {
        bot.gameObject.SetActive(true);
        //bot.TaskCompleted += OnTaskCompleted;
        bot.TaskCompleted += bot.Base.OnCollectorBotTaskCompleted(this);
    }


    private void Disable(CollectorBot bot)
    {
        bot.TaskCompleted -= OnTaskCompleted;
        bot.gameObject.SetActive(false);
    }
}
