using UnityEngine;

public class ProgressBarRotator : MonoBehaviour
{
    private void LateUpdate()           // Добавить на прогрессбар
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
