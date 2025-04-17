using UnityEngine;
using Cinemachine;

public class RadioEventTrigger : MonoBehaviour
{
    public AudioSource radioSource;
    public AudioClip radioClip;
    public Transform radioTransform;
    public float zoomFOV = 30f;
    public float transitionDuration = 2f;

    private CinemachineVirtualCamera virtualCamera;
    private float originalFOV;
    private Transform originalFollow;
    private Transform originalLookAt;
    private bool triggered = false;

    private void Start()
    {
        virtualCamera = GameObject.Find("radio_camera")?.GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("CinemachineVirtualCamera not found in the scene.");
            return;
        }

        originalFOV = virtualCamera.m_Lens.FieldOfView;
        originalFollow = virtualCamera.Follow;
        originalLookAt = virtualCamera.LookAt;
    }

    public void TriggerRadioEvent()
    {
        if (triggered) return;
        triggered = true;

        // Start audio
        radioSource.clip = radioClip;
        radioSource.Play();

        // Set Follow and LookAt to radio
        virtualCamera.Follow = radioTransform;
        virtualCamera.LookAt = radioTransform;

        // Start zoom coroutine
        StartCoroutine(ZoomInOnRadio());
    }

    private System.Collections.IEnumerator ZoomInOnRadio()
    {
        float time = 0f;
        float startFOV = virtualCamera.m_Lens.FieldOfView;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = time / transitionDuration;

            virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(startFOV, zoomFOV, t);
            yield return null;
        }
    }

    // Optionally, create a reset method
    public void ResetCamera()
    {
        virtualCamera.Follow = originalFollow;
        virtualCamera.LookAt = originalLookAt;
        virtualCamera.m_Lens.FieldOfView = originalFOV;
        triggered = false;
    }
}
