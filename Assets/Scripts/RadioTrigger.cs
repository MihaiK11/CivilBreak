using UnityEngine;

public class RadioEventTrigger : MonoBehaviour
{
    public AudioSource radioSource;
    public AudioClip radioClip;
    public Transform radioTransform;
    public float zoomFOV = 30f;
    public float transitionDuration = 2f;

    private Camera mainCamera;
    private bool triggered = false;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void TriggerRadioEvent()
    {
        if (triggered) return;
        triggered = true;

        // Включаем звук
        radioSource.clip = radioClip;
        radioSource.Play();

        // Запускаем корутину камеры
        StartCoroutine(MoveCameraToRadio());
    }

    private System.Collections.IEnumerator MoveCameraToRadio()
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float startFOV = mainCamera.fieldOfView;

        Vector3 targetPos = radioTransform.position + radioTransform.forward * -2f + Vector3.up * 1.5f;
        Quaternion targetRot = Quaternion.LookRotation(radioTransform.position - targetPos);

        float time = 0f;
        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;

            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            mainCamera.fieldOfView = Mathf.Lerp(startFOV, zoomFOV, t);
            yield return null;
        }
    }
}