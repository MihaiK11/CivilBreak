using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEditor.SceneManagement;  // Make sure to include the Cinemachine namespace

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Subtitles")]
    public SubtitleController subtitleController;

    [Header("Camera Control")]
    public CameraController cameraController;

    [Header("Cinemachine Brain")]
    private CinemachineBrain cinemachineBrain;

    [Header("Intro Lines (Romanian)")]
    [TextArea(3, 10)]
    public List<string> introLines = new List<string>
    {
        "Si iarasi o noua zi, blestemata zi a veveritei...",
        "Ce viata fara sens... si din nou trebuie sa merg la tribunal...",
        "Fratele meu nu se mai intoarce... Guvernul a luat-o razna complet...",
        "Ce naiba? Am astm... La ce-mi folosesc banii daca nu am medicamente?",
        "Am tot asteptat sa se schimbe ceva... Dar ajunge.",
        "O sa pun eu lucrurile la locul lor. Cu orice pret."
    };

    [Header("Radio Scene Event")]
    public GameObject radioObject; // Assign the radio object in the inspector
    public AudioClip radioClip;    // Assign the radio clip in the inspector
    public float radioZoomDuration = 3f;
    public float radioZoomFOV = 25f;
    private AudioSource radioSource;

    [SerializeField] private AudioSource radioAudio;
    [SerializeField] private Transform radioTransform;
    [SerializeField] private AudioSource cityAmbienceAudio;

    [Header("Intro Animation")]
    [SerializeField] private CameraTransition introCameraTransition;


    [SerializeField] private float zoomFOV = 20f;
    [SerializeField] private float zoomDuration = 1f;
    [SerializeField] private float stayDuration = 3f;

    [Header("Mini-game")]
    [SerializeField] private GameObject flyerTypingManager;

    [Header("Cinemachine Cameras")]
    public CinemachineVirtualCamera mainCamera;  // Reference to the main virtual camera
    public CinemachineVirtualCamera introCamera; // Reference to the intro virtual camera
    public CinemachineVirtualCamera radioCamera; // Reference to the radio virtual camera

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {   
        if (cameraController != null)
            cameraController.enabled = false;

        if (radioObject != null)
            radioSource = radioObject.GetComponent<AudioSource>();
        
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        for (int i = 0; i < introLines.Count; i++)
        {   
            if (i == 0)
            {
                subtitleController.HidePanel(); // Hide subtitles before the intro animation
                // Play any intro animation / hold a few seconds if needed
                yield return StartCoroutine(IntroCameraSequence());

                subtitleController.ShowPanel(); // Show subtitles after the intro animation
            }
            // === Trigger radio scene ===
            if (i == 2)
            {
                // Switch to the radio camera for the radio scene
                SwitchCameras(mainCamera, radioCamera); // Switch to the radio camera

                // Hide subtitles before the radio scene
                subtitleController.HidePanel();

                // Play the radio and zoom in
                if (radioSource != null && radioClip != null)
                {
                    radioSource.clip = radioClip;
                    radioSource.Play();
                }

                yield return StartCoroutine(FocusOnRadio());

                // Show subtitles again after the radio scene
                subtitleController.ShowPanel();

            }

            // Display the current intro line
            yield return subtitleController.TypeLineWithSkip(introLines[i]);
        }

        // After the last line, hide the subtitles
        subtitleController.HidePanel();

        // Switch back to the main camera
        SwitchCameras(radioCamera, mainCamera, 2f);
        yield return new WaitForSeconds(2f); // Wait for the blend to complete

        // Enable camera control
        if (cameraController != null)
            cameraController.enabled = true;

        // Show the instruction UI
        InstructionUI.Instance.Show("Dan a decis sa rastoarne guvernul. Mai intai, vrea sa creeze pliante. Cauta masina lui rosie in zona ghetoului. Ca sa dai click pe ea tine tasta Ctrl si apasa cu MouseLeftButton.");

        Debug.Log("Intro complete. Camera control is now enabled.");
    }

    // Generic function to switch between two cameras
    private void SwitchCameras(CinemachineVirtualCamera camera1, CinemachineVirtualCamera camera2, float blendDuration = 0f)
    {
        if (camera1 != null && camera2 != null)
        {
            if (cinemachineBrain != null)
            {
                // Set the default blend
                var customBlend = new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseInOut, blendDuration);
                cinemachineBrain.m_DefaultBlend = customBlend;
            }
            // Set the priority of camera1 to a lower value and camera2 to a higher value
            camera1.Priority = 0;   // Lower priority for camera1
            camera2.Priority = 10;  // Higher priority for camera2
        }
    }

private IEnumerator IntroCameraSequence()
{   
    // Disable the main camera and enable the intro camera
    SwitchCameras(mainCamera, introCamera); // Switch to the intro camera

    introCameraTransition = introCamera.GetComponent<CameraTransition>();
    if (introCameraTransition == null)
    {
        Debug.LogError("Intro camera transition component not found!");
        yield break;
    }
    // Calculate time to wait before switching to the main camera
    float waitTime = introCameraTransition.GetTotalTransitionTime(); // Add 3 seconds for the intro animation

    // Play any intro animation / hold a few seconds if needed
    yield return new WaitForSeconds(waitTime); // Adjust timing

    // Switch to main camera for subtitles
    SwitchCameras(introCamera, mainCamera, 1f); // Switch to the radio camera for the radio scene
}

    private IEnumerator FocusOnRadio()
{
    // Store the original field of view of the radio camera
    float originalFOV = radioCamera.m_Lens.FieldOfView;
    Vector3 originalPosition = radioCamera.transform.position;
    Quaternion originalRotation = radioCamera.transform.rotation;

    // Rotate the radio camera towards the radio
    radioCamera.transform.LookAt(radioTransform);

    // Zoom in on the radio
    float t = 0;
    while (t < zoomDuration)
    {
        radioCamera.m_Lens.FieldOfView = Mathf.Lerp(originalFOV, zoomFOV, t / zoomDuration);
        t += Time.deltaTime;
        yield return null;
    }
    radioCamera.m_Lens.FieldOfView = zoomFOV;

    // Lower the city ambience sound
    if (cityAmbienceAudio != null)
        cityAmbienceAudio.volume = 0.2f;

    // Play the radio audio if not already playing
    if (radioAudio != null && !radioAudio.isPlaying)
        radioAudio.Play();

    // Hold the zoom for the specified duration
    yield return new WaitForSeconds(stayDuration);

    // Return the radio camera to its original position
    t = 0;
    while (t < zoomDuration)
    {
        radioCamera.m_Lens.FieldOfView = Mathf.Lerp(zoomFOV, originalFOV, t / zoomDuration);
        t += Time.deltaTime;
        yield return null;
    }
    radioCamera.m_Lens.FieldOfView = originalFOV;

    // Restore the city ambience sound
    if (cityAmbienceAudio != null)
        cityAmbienceAudio.volume = 1f;
}


    // Called when the car is clicked (CarTrigger)
    public void StartFlyerTypingGame()
    {
        flyerTypingManager.SetActive(true); // Activate the flyer typing game panel
    }
}
