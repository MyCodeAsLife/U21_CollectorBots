using UnityEngine;
using UnityEngine.UI;

public class ProgressBarViewer : MonoBehaviour
{
    [SerializeField] private Slider _progressBar;           // Сделать префаб прогресбара и создать прогресс бар при старте

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void OnChange(float value)
    {
        _progressBar.value = value;
    }
}
