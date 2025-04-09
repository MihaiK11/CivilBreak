using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InstructionUI : MonoBehaviour
{
    public static InstructionUI Instance;

    [Header("UI Elements")]
    public GameObject panel;
    public Text instructionText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Hide();
    }

    public void Show(string message)
    {
        if (panel != null)
        {
            panel.SetActive(true);
            instructionText.text = message;
        }
    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void ShowTemporary(string message, float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShowTempRoutine(message, duration));
    }

    private IEnumerator ShowTempRoutine(string message, float duration)
    {
        Show(message);
        yield return new WaitForSeconds(duration);
        Hide();
    }
}