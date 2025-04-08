using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject subtitlePanel;         // Панель с Image и Text
    public Text subtitleText;                // Сам Text (Legacy)
    public float typeSpeed = 0.04f;          // Скорость печати

    private bool isTyping = false;
    private bool skipLine = false;

    private bool spacePressedAfterLine = false;
    private bool allowContinue = false;

    public IEnumerator PlaySubtitleSequence(List<string> lines)
    {
        ShowPanel();

        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLineWithSkip(line));
        }

        HidePanel();
    }

    public IEnumerator TypeLineWithSkip(string line)
    {
        isTyping = true;
        skipLine = false;
        subtitleText.text = "";
        allowContinue = false;
        spacePressedAfterLine = false;

        for (int i = 0; i < line.Length; i++)
        {
            if (skipLine)
            {
                subtitleText.text = line;
                break;
            }

            subtitleText.text += line[i];
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        allowContinue = true;

        // Ждём, пока игрок нажмёт пробел после окончания строки
        yield return new WaitUntil(() => spacePressedAfterLine);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                skipLine = true;
            }
            else if (allowContinue)
            {
                spacePressedAfterLine = true;
            }
        }
    }

   public void ShowPanel()
    {
        subtitlePanel.SetActive(true);
    }

public void HidePanel()
    {
        subtitlePanel.SetActive(false);
    }
    
    
}