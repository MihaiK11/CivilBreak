using UnityEngine;
using System.Collections.Generic;

public class PedestrianSpawner : MonoBehaviour
{
    public GameObject[] pedestrianPrefabs;
    public Transform waypointRoot;
    public int maxPedestrians = 15;
    public float spawnInterval = 5f;
    public int pedestriansOnStart = 5;

    private List<PedestrianWaypoint> spawnPoints = new List<PedestrianWaypoint>();
    private List<GameObject> activePedestrians = new List<GameObject>();
    private float spawnTimer = 0f;

    void Start()
    {
        foreach (Transform child in waypointRoot)
        {
            PedestrianWaypoint wp = child.GetComponent<PedestrianWaypoint>();
            if (wp != null)
                spawnPoints.Add(wp);
        }

        for (int i = 0; i < pedestriansOnStart; i++)
        {
            TrySpawnPedestrian(force: true);
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && activePedestrians.Count < maxPedestrians)
        {
            spawnTimer = 0f;
            TrySpawnPedestrian();
        }

        activePedestrians.RemoveAll(p => p == null);
    }

    void TrySpawnPedestrian(bool force = false)
    {
        if (spawnPoints.Count == 0 || pedestrianPrefabs.Length == 0)
            return;

        PedestrianWaypoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        GameObject prefab = pedestrianPrefabs[Random.Range(0, pedestrianPrefabs.Length)];

        GameObject pedestrian = Instantiate(prefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        pedestrian.transform.localScale = new Vector3(0.041695863f, 0.041695863f, 0.041695863f);

        PedestrianAI ai = pedestrian.GetComponent<PedestrianAI>();
        if (ai != null)
        {
            ai.currentTarget = spawnPoint;
            activePedestrians.Add(pedestrian);
        }
        else
        {
            Debug.LogWarning("Префаб пешехода не содержит компонент PedestrianAI!");
            Destroy(pedestrian);
        }
    }
}