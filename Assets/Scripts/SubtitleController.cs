using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SubtitleController : MonoBehaviour
{
    public Text subtitleText;
    public float typeSpeed = 0.04f;

    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool skipRequested = false;

    void Update()
    {
        if (isTyping && Input.GetKeyDown(KeyCode.Space))
        {
            skipRequested = true;
        }
    }

    public IEnumerator TypeLineWithSkip(string line)
    {
        subtitleText.text = "";
        isTyping = true;
        skipRequested = false;

        foreach (char letter in line.ToCharArray())
        {
            if (skipRequested)
            {
                subtitleText.text = line;
                break;
            }

            subtitleText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;

        // Ждём пока игрок нажмёт Space для перехода к следующему субтитру
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
    }
}