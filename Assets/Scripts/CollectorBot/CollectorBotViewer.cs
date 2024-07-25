using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CollectorBotAI))]
public class CollectorBotViewer : MonoBehaviour
{
    private Slider _progressBar;
    private Slider _prefabProgressBar;
    private CollectorBotAI _presenter;

    private void Awake()
    {
        _presenter = GetComponent<CollectorBotAI>();
        _prefabProgressBar = Resources.Load<Slider>("Prefabs/ProgressBar");
        _progressBar = Instantiate<Slider>(_prefabProgressBar, transform);
    }

    private void OnDisable()
    {
        _presenter.CollectingStarted -= OnStartCollection;
        _presenter.CollectingFinished -= OnFinishCollection;
        _presenter.UnsubscribeToCollectionProgress(OnCollectionProgress);
    }

    private void Start()
    {
        _progressBar.gameObject.SetActive(false);
        _presenter.CollectingStarted += OnStartCollection;
        _presenter.CollectingFinished += OnFinishCollection;
        _presenter.SubscribeToCollectionProgress(OnCollectionProgress);
    }

    public void OnCollectionProgress(float value)
    {
        _progressBar.value = value;
    }

    private void OnStartCollection()
    {
        _progressBar.gameObject.SetActive(true);
    }

    private void OnFinishCollection()
    {
        _progressBar.gameObject.SetActive(false);
    }
}
