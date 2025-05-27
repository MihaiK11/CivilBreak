using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Click : MonoBehaviour
{
    public GameObject messageUI;
    public TMP_Text declaratieText;
    public TMP_Text statusText;
    public Image backgroundEnvironment;

    public Color culoareNormala = Color.gray;
    public Color culoareVerde = Color.green;

    public ParticleSystem fumFabrica;  // Particle System pentru fum

    private string mesajComplet = "Declar oprirea fabricii imediat, fară vreo altă oprire!";
    private int literaIndex = 0;
    private bool isListening = false;
    private float timer = 10f;
    private bool hasWon = false;
    private bool timerRunning = false;

    void OnMouseDown()
    {
        if (messageUI != null && declaratieText != null)
        {
            messageUI.SetActive(true);
            declaratieText.text = "";
            literaIndex = 0;
            isListening = true;
            hasWon = false;
            timer = 10f;
            timerRunning = true;

            if (statusText != null)
            {
                statusText.text = "";
                statusText.gameObject.SetActive(false);
            }

            if (backgroundEnvironment != null)
                backgroundEnvironment.color = culoareNormala;

            if (fumFabrica != null)
                fumFabrica.Play(); // Pornește particulele de fum când începe tastarea
        }
    }

    void Update()
    {
        if (isListening && !hasWon)
        {
            if (Input.anyKeyDown)
            {
                if (literaIndex < mesajComplet.Length)
                {
                    declaratieText.text += mesajComplet[literaIndex];
                    literaIndex++;

                    if (literaIndex == mesajComplet.Length)
                    {
                        hasWon = true;
                        Win();
                    }
                }
            }
        }

        if (timerRunning)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                timerRunning = false;

                if (!hasWon)
                    Lose();
            }
        }
    }

    void Win()
    {
        isListening = false;
        timerRunning = false;
        declaratieText.text = "";

        if (statusText != null)
        {
            statusText.text = "Fabrica oprită cu succes!";
            statusText.color = Color.green;
            statusText.gameObject.SetActive(true);
        }

        if (backgroundEnvironment != null)
            backgroundEnvironment.color = culoareVerde;

        if (fumFabrica != null)
            fumFabrica.Stop(); // Opresc fum când câștig

        Invoke("HideMessage", 3f);
    }

    void Lose()
    {
        isListening = false;
        declaratieText.text = "";

        if (statusText != null)
        {
            statusText.text = "Timpul a expirat! Fabrica continuă să polueze!";
            statusText.color = Color.red;
            statusText.gameObject.SetActive(true);
        }

        // Nu oprim fumul, să continue să iasă fum
        Invoke("HideMessage", 3f);
    }

    void HideMessage()
    {
        messageUI.SetActive(false);

        if (statusText != null)
        {
            statusText.text = "";
            statusText.gameObject.SetActive(false);
        }

        // Opțional: dacă vrei, poți opri fumul și aici, dacă ascunzi UI
        // if (fumFabrica != null) fumFabrica.Stop();
    }
}
