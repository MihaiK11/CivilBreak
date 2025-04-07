using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Subtitles")]
    public SubtitleController subtitleController; // назначить вручную в инспекторе

    [Header("Сценарий вступления")]
    [TextArea(3, 10)]
    public List<string> introLines = new List<string>
    {
    "Si iarasi incepe o noua zi... aceeasi rutina obositoare...",
    "Totul pare lipsit de sens... si din nou trebuie sa merg la tribunal...",
    "Fratele meu nu se mai intoarce... Guvernul si-a pierdut complet controlul...",
    "Astazi au anulat si asigurarea medicala... Fara nicio explicatie.",
    "Am astm... Ce rost mai au banii daca nu pot sa-mi cumpar medicamente?",
    "Toata lumea tace, accepta... Dar eu nu mai pot sa stau deoparte.",
    "Daca nimeni nu face nimic, o sa fac eu. O sa pun totul la locul lui."
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        foreach (string line in introLines)
        {
            yield return subtitleController.TypeLineWithSkip(line);
        }

        Debug.Log("Интро закончено. Пора действовать!");
        // Здесь логика перехода к игре
    }
}
