using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CarSpawner : MonoBehaviour
{
    [Header("Настройки спавна автомобилей")]
    public GameObject[] carPrefabs;
    public Transform waypointRoot;
    public float spawnInterval = 4f;
    public int maxCars = 5;
    public int carsOnStart = 2;

    private List<WaypointNode> spawnPoints = new List<WaypointNode>();
    private List<GameObject> activeCars = new List<GameObject>();
    private float spawnTimer;

    void Start()
    {
        foreach (Transform child in waypointRoot)
        {
            WaypointNode node = child.GetComponent<WaypointNode>();
            if (node != null)
            {
                // Чекаем имя на наличие номера
                string name = child.name;
                int number;
                if (int.TryParse(Regex.Match(name, @"\d+").Value, out number))
                {
                    if (number % 2 == 0) // чётный номер
                    {
                        spawnPoints.Add(node);
                    }
                }
            }
        }

        if (spawnPoints.Count == 0)
        {
            Debug.LogError("Не найден ни один чётный WaypointNode в waypointRoot!");
            return;
        }

        for (int i = 0; i < carsOnStart; i++)
        {
            TrySpawnCar(true);
        }
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval && activeCars.Count < maxCars)
        {
            spawnTimer = 0f;
            TrySpawnCar();
        }

        activeCars.RemoveAll(car => car == null);
    }

    void TrySpawnCar(bool force = false)
    {
        if (spawnPoints.Count == 0 || carPrefabs.Length == 0)
            return;

        WaypointNode spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        if (spawnPoint == null)
            return;

        if (Camera.main != null)
        {
            Vector3 screenPoint = Camera.main.WorldToViewportPoint(spawnPoint.transform.position);
            bool isVisible = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            if (!force && isVisible)
                return;
        }

        if (Physics.Raycast(spawnPoint.transform.position + Vector3.up * 0.5f, spawnPoint.transform.forward, 3f, LayerMask.GetMask("Car")))
            return;

        GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        GameObject car = Instantiate(carPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
        car.transform.localScale = new Vector3(0.026175f, 0.026175f, 0.026175f);

        CarAIController ai = car.GetComponent<CarAIController>();
        if (ai != null)
        {
            ai.previousTarget = spawnPoint;

            if (spawnPoint.connectedWaypoints != null && spawnPoint.connectedWaypoints.Count > 0)
            {
                ai.currentTarget = spawnPoint.connectedWaypoints[Random.Range(0, spawnPoint.connectedWaypoints.Count)];
                // Debug.Log($"{car.name} будет ехать от {spawnPoint.name} к {ai.currentTarget.name}");
                activeCars.Add(car);
            }
            else
            {
                Debug.LogWarning($"{spawnPoint.name} не имеет подключенных путей для старта!");
                Destroy(car);
            }
        }
        else
        {
            Debug.LogError($"Префаб {car.name} не содержит компонента CarAIController!");
            Destroy(car);
        }
    }
}