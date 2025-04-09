using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyerTypingMiniGame : MonoBehaviour
{
    public GameObject laptopPanel;
    public Text textPliante;
    public InstructionUI instructionUI;

    [TextArea(3, 10)]
    public List<string> cuvintePliante;

    private int currentWordIndex = 0;
    private string currentLine = "";
    private bool isCompleted = false;
    private bool shownExitInstruction = false;

    private void OnEnable()
    {
        StartGame();
    }

    public void StartGameManually()
    {
        StartGame();
    }

    private void StartGame()
    {
        if (laptopPanel != null)
            laptopPanel.SetActive(true);

        textPliante.text = "";
        currentWordIndex = 0;
        currentLine = "";
        isCompleted = false;
        shownExitInstruction = false;

        instructionUI.Show("Apasă tastele pentru a scrie conținutul pliantului.");
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        // Если игра уже завершена — ждем Enter
        if (isCompleted)
        {
            if (!shownExitInstruction)
            {
                instructionUI.Show("Apasă Enter pentru a închide laptopul.");
                shownExitInstruction = true;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                EndGame();
            }

            return;
        }

        // Ввод следующего слова
        if (Input.anyKeyDown && currentWordIndex < cuvintePliante.Count)
        {
            string nextWord = cuvintePliante[currentWordIndex];
            currentLine += nextWord + " ";
            textPliante.text = currentLine;
            currentWordIndex++;

            if (currentWordIndex >= cuvintePliante.Count)
            {
                isCompleted = true;
            }
        }
    }

    private void EndGame()
    {
        if (laptopPanel != null)
            laptopPanel.SetActive(false);

        instructionUI.Hide();
        gameObject.SetActive(false); // отключаем сам менеджер
    }
}