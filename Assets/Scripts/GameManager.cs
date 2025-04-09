using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Subtitles")]
    public SubtitleController subtitleController;

    [Header("Camera Control")]
    public CameraController cameraController;

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
    public GameObject radioObject; // Назначить объект радио в инспекторе
    public AudioClip radioClip;    // Назначить звук радио в инспекторе
    public float radioZoomDuration = 3f;
    public float radioZoomFOV = 25f;
    private AudioSource radioSource;

    [SerializeField] private AudioSource radioAudio;
    [SerializeField] private Transform radioTransform;
    [SerializeField] private AudioSource cityAmbienceAudio;

    [SerializeField] private float zoomFOV = 20f;
    [SerializeField] private float zoomDuration = 1f;
    [SerializeField] private float stayDuration = 3f;

    [Header("Mini-game")]
    [SerializeField] private GameObject flyerTypingManager;

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

        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        for (int i = 0; i < introLines.Count; i++)
        {
            // === Точка срабатывания радиосцены ===
            if (i == 2)
            {
                // Скрываем субтитры перед сценой с радио
                subtitleController.HidePanel();

                // Включаем радио и делаем зум
                if (radioSource != null && radioClip != null)
                {
                    radioSource.clip = radioClip;
                    radioSource.Play();
                }

                yield return StartCoroutine(FocusOnRadio());

                // Возвращаем панель субтитров
                subtitleController.ShowPanel();
            }

            // Показываем строку
            yield return subtitleController.TypeLineWithSkip(introLines[i]);
        }

        // После последней строки — скрываем субтитры
        subtitleController.HidePanel();

        // Включаем управление камерой
        if (cameraController != null)
            cameraController.enabled = true;

        // Показываем инструкцию
        InstructionUI.Instance.Show("Dan a decis sa rastoarne guvernul. Mai intai, vrea sa creeze pliante. Cauta masina lui rosie  in zona ghetoului. Ca sa dai click pe ea tine tasta Ctrl si apasa cu MouseLeftButton.");

        Debug.Log("Intro complete. Camera control is now enabled.");
    }

    private IEnumerator FocusOnRadio()
    {
        float originalFOV = Camera.main.fieldOfView;
        Vector3 originalPosition = Camera.main.transform.position;
        Quaternion originalRotation = Camera.main.transform.rotation;

        // Поворачиваем камеру к радио
        Camera.main.transform.LookAt(radioTransform);

        // Зумим
        float t = 0;
        while (t < zoomDuration)
        {
            Camera.main.fieldOfView = Mathf.Lerp(originalFOV, zoomFOV, t / zoomDuration);
            t += Time.deltaTime;
            yield return null;
        }
        Camera.main.fieldOfView = zoomFOV;

        // Приглушаем шум города
        if (cityAmbienceAudio != null)
            cityAmbienceAudio.volume = 0.2f;

        // Включаем радио (если не было выше)
        if (radioAudio != null && !radioAudio.isPlaying)
            radioAudio.Play();

        // Держим зум 3 секунды
        yield return new WaitForSeconds(stayDuration);

        // Возвращаем камеру
        t = 0;
        while (t < zoomDuration)
        {
            Camera.main.fieldOfView = Mathf.Lerp(zoomFOV, originalFOV, t / zoomDuration);
            t += Time.deltaTime;
            yield return null;
        }
        Camera.main.fieldOfView = originalFOV;

        // Возвращаем громкость города
        if (cityAmbienceAudio != null)
            cityAmbienceAudio.volume = 1f;
    }

    // Вызывается при клике на машину (CarTrigger)
    public void StartFlyerTypingGame()
    {
        flyerTypingManager.SetActive(true); // Появляется панель ввода текста
    }
}