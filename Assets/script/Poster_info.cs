using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HidePosterOnKey : MonoBehaviour
{
    public GameObject poster;
    public TextMeshProUGUI promptText;
    public GameObject progressBarObject;
    public Slider progressBarSlider;
    public KeyCode hideKey = KeyCode.E;
    public float holdDuration = 20f;

    public Transform player;
    public Transform police_officer;
    public float policeDetectDistance = 5f;

    private float holdTime = 0f;
    private bool isHiding = false;
    private Renderer posterRenderer;
    private bool isGameOver = false;
    private bool hasWon = false;

    void Start()
    {
        if (poster != null)
            posterRenderer = poster.GetComponent<Renderer>();

        if (progressBarObject != null)
            progressBarObject.GetComponent<CanvasGroup>().alpha = 0f;

        if (progressBarSlider != null)
        {
            progressBarSlider.minValue = 0;
            progressBarSlider.maxValue = holdDuration;
            progressBarSlider.value = 0;
        }

        if (promptText != null)
            promptText.text = "Press E";
    }

    void Update()
    {
        if (isGameOver || hasWon)
            return;

        // Verificăm distanța față de polițist
        if (police_officer != null && player != null)
        {
            float dist = Vector3.Distance(player.position, police_officer.position);

            Debug.Log($"Distanta politie: {dist:F2} metri, GameOver: {isGameOver}");

            if (dist <= policeDetectDistance && Input.GetKeyDown(hideKey))
            {
                if (promptText != null)
                    promptText.text = "GAME OVER! Te-a prins poliția!";

                isGameOver = true;
                return;
            }
        }

        // Dacă jucătorul ține apăsat tasta
        if (Input.GetKey(hideKey))
        {
            if (!isHiding)
            {
                if (posterRenderer != null)
                    posterRenderer.enabled = false;

                if (promptText != null)
                    promptText.gameObject.SetActive(false);

                if (progressBarObject != null)
                    progressBarObject.GetComponent<CanvasGroup>().alpha = 1f;

                isHiding = true;
            }

            holdTime += Time.deltaTime;

            if (progressBarSlider != null)
                progressBarSlider.value = holdTime;

            if (holdTime >= holdDuration)
            {
                hasWon = true;

                if (promptText != null)
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = "Ați câștigat, puteți evada!";
                }

                if (progressBarObject != null)
                    progressBarObject.GetComponent<CanvasGroup>().alpha = 0f;
            }
        }
        else
        {
            // Dezactivăm doar ascunderea, dar NU resetăm holdTime
            if (isHiding)
            {
                if (posterRenderer != null)
                    posterRenderer.enabled = true;

                if (!hasWon && promptText != null)
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = "Press E";
                }

                if (progressBarObject != null)
                    progressBarObject.GetComponent<CanvasGroup>().alpha = 0f;

                isHiding = false;
            }
        }
    }
}
