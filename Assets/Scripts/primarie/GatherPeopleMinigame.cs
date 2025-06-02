using UnityEngine;
using System.Collections.Generic;

public class GatherPeopleMinigame : MonoBehaviour
{
    public Transform[] gatherPoints;
    public GameObject npcPrefab;
    public int maxNPCs = 10;
    public Transform player;

    private int currentPointIndex = 0;
    private List<List<GameObject>> allGroups = new List<List<GameObject>>(); // store groups per point
    private List<GameObject> currentGroup = new List<GameObject>();
    private bool inGatherZone = false;
    private int spacePressCount = 0;
    private float lastSpacePressTime = 0f;
    private float pressTimeout = 1f;
    private bool gatherComplete = false;

    // Formation settings
    private int formationColumns = 5;    // Number of NPCs per row
    private float spacing = 0.05f;        // Space between NPCs

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
                        allGroups.Add(new List<GameObject>(currentGroup)); // save current group permanently
                        currentGroup = new List<GameObject>(); // reset for next point
                        Debug.Log("Gather complete! NPCs stay visible.");

                        // Optional: Automatically move player to next point or wait for input
                        // For now, just let the player move manually.
                    }
                }
            }

            if (!gatherComplete && Time.time - lastSpacePressTime > pressTimeout && spacePressCount > 0)
            {
                ResetCurrentGroup();
            }

            // Example: move to next point automatically once complete
            // Or you can implement your own trigger to move player
            if (gatherComplete && Input.GetKeyDown(KeyCode.Return))
            {
                GoToNextPoint();
            }
        }
    }

    void CheckPlayerInGatherZone()
    {
        float dist = Vector3.Distance(player.position, gatherPoints[currentPointIndex].position);
        inGatherZone = dist <= 2f;
    }

    void AddNPC(int index)
    {
        if (currentGroup.Count >= maxNPCs)
            return;

        int row = index / formationColumns;
        int col = index % formationColumns;

        Vector3 spawnPos = gatherPoints[currentPointIndex].position +
                           new Vector3(row * spacing, 0, col * spacing);

        Quaternion rotation = Quaternion.Euler(0f, -90f, 0f);

        GameObject npc = Instantiate(npcPrefab, spawnPos, rotation);
        currentGroup.Add(npc);
    }

    void ResetCurrentGroup()
    {
        spacePressCount = 0;
        foreach (var npc in currentGroup)
            Destroy(npc);
        currentGroup.Clear();
    }

    void GoToNextPoint()
    {
        // Do NOT destroy previously spawned groups, so they stay visible!

        spacePressCount = 0;
        gatherComplete = false;
        currentGroup.Clear();

        currentPointIndex++;
        if (currentPointIndex >= gatherPoints.Length)
        {
            Debug.Log("Minigame finished!");
            enabled = false;
            return;
        }
        Debug.Log("Go to next gathering point!");
    }
}
