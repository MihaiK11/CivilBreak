using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GatherPeopleMinigame : MonoBehaviour
{
    public Transform[] gatherPoints;
    public GameObject npcPrefab;
    public int maxNPCs = 10;
    public Transform player;

    public AudioSource policeSirenAudio; // Assign in inspector (police siren sound)
    public string prisonSceneName = "PrisonScene"; // Set your prison scene name here
    public float delayBeforeSceneChange = 5f; // Delay in seconds before changing scene

    private int currentPointIndex = 0;
    private List<List<GameObject>> allGroups = new List<List<GameObject>>();
    private List<GameObject> currentGroup = new List<GameObject>();
    private bool inGatherZone = false;
    private int spacePressCount = 0;
    private float lastSpacePressTime = 0f;
    private float pressTimeout = 1f;
    private bool gatherComplete = false;

    // Formation settings
    private int formationColumns = 5;
    private float spacing = 0.05f;

    void Update()
    {
        CheckPlayerInGatherZone();

        if (inGatherZone)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!gatherComplete)
                {
                    spacePressCount++;
                    lastSpacePressTime = Time.time;
                    AddNPC(spacePressCount - 1);

                    if (spacePressCount >= maxNPCs)
                    {
                        gatherComplete = true;
                        allGroups.Add(new List<GameObject>(currentGroup));
                        currentGroup = new List<GameObject>();
                        Debug.Log("Gather complete! NPCs stay visible.");
                    }
                }
            }

            if (!gatherComplete && Time.time - lastSpacePressTime > pressTimeout && spacePressCount > 0)
            {
                ResetCurrentGroup();
            }
        }

        if (gatherComplete && Input.GetKeyDown(KeyCode.Return))
        {
            GoToNextPoint();
        }
    }

    void CheckPlayerInGatherZone()
    {
        float dist = Vector3.Distance(player.position, gatherPoints[currentPointIndex].position);
        inGatherZone = dist <= 0.15f;
    }

    void AddNPC(int index)
    {
        if (currentGroup.Count >= maxNPCs)
            return;

        int row = index / formationColumns;
        int col = index % formationColumns;
        float toTownHall = 0.15f;
        Vector3 spawnPos = gatherPoints[currentPointIndex].position +
                           new Vector3(row * spacing - toTownHall, 0, col * spacing);

        Quaternion rotation = Quaternion.Euler(0f, -90f, 0f);

        GameObject npc = Instantiate(npcPrefab, spawnPos, rotation);

        var ai = npc.GetComponent<PedestrianAIPrimarie>();
        if (ai != null)
        {
            PedestrianWaypoint waypoint = gatherPoints[currentPointIndex].GetComponent<PedestrianWaypoint>();
            if (waypoint != null)
            {
                ai.currentTarget = waypoint;
            }
        }

        currentGroup.Add(npc);
    }

    void ResetCurrentGroup()
    {
        Debug.Log("Timeout reached! Resetting group...");
        spacePressCount = 0;
        foreach (var npc in currentGroup)
            Destroy(npc);
        currentGroup.Clear();
    }

    void GoToNextPoint()
    {
        Debug.Log("Moving to next gather point.");

        spacePressCount = 0;
        gatherComplete = false;
        currentGroup.Clear();

        currentPointIndex++;
        if (currentPointIndex >= gatherPoints.Length)
        {
            Debug.Log("Minigame finished!");

            StartPoliceArrivalSequence();

            enabled = false;
            return;
        }

        Debug.Log($"Go to next gathering point: {currentPointIndex}");
    }

    void StartPoliceArrivalSequence()
    {
        if (policeSirenAudio != null)
        {
            policeSirenAudio.Play();
        }
        Invoke(nameof(ChangeToPrisonScene), delayBeforeSceneChange);
    }

    void ChangeToPrisonScene()
    {
        SceneManager.LoadScene(prisonSceneName);
    }
}
