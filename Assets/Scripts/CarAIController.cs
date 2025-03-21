using UnityEngine;
using System.Collections.Generic;

public class CarAIController : MonoBehaviour
{
    [Header("Параметры движения")]
    public float maxMoveSpeed = 1.5f;
    public float acceleration = 1f;
    public float deceleration = 1.5f;
    public float slowDownDistance = 1.5f;
    public float reachThreshold = 0.05f;

    private float currentSpeed = 0f;

    [Header("Waypoint настройки")]
    public WaypointNode currentTarget;
    [HideInInspector] public WaypointNode previousTarget;
    private Queue<WaypointNode> waypointHistory = new Queue<WaypointNode>();

    private void Update()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.transform.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (direction.sqrMagnitude < 0.01f)
        {
            Debug.LogWarning($"{gameObject.name}: Target direction too small!");
            return;
        }

        // Плавное замедление
        if (distance < slowDownDistance)
            currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, 0.2f);
        else
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, maxMoveSpeed);

        // Двигаем и поворачиваем
        transform.forward = direction.normalized;
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        Vector3 flatCarPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatTarget = new Vector3(currentTarget.transform.position.x, 0, currentTarget.transform.position.z);

        if (Vector3.Distance(flatCarPos, flatTarget) < reachThreshold)
        {
            ChooseNextWaypoint();
        }
    }

    private void ChooseNextWaypoint()
    {
        if (currentTarget.connectedWaypoints == null || currentTarget.connectedWaypoints.Count == 0)
        {
            Debug.LogWarning($"{gameObject.name}: No connected waypoints!");
            return;
        }

        List<WaypointNode> options = new List<WaypointNode>(currentTarget.connectedWaypoints);
        if (previousTarget != null && options.Count > 1)
            options.Remove(previousTarget);

        WaypointNode thirdBack = GetWaypointFromHistory(3);
        if (thirdBack != null && options.Count > 1)
            options.Remove(thirdBack);

        if (options.Count == 0)
        {
            options = new List<WaypointNode>(currentTarget.connectedWaypoints);
            if (previousTarget != null && options.Count > 1)
                options.Remove(previousTarget);
        }

        previousTarget = currentTarget;
        currentTarget = options[Random.Range(0, options.Count)];

        waypointHistory.Enqueue(currentTarget);
        if (waypointHistory.Count > 5)
            waypointHistory.Dequeue();
    }

    private WaypointNode GetWaypointFromHistory(int stepsBack)
    {
        if (waypointHistory.Count >= stepsBack)
        {
            WaypointNode[] history = waypointHistory.ToArray();
            return history[history.Length - stepsBack];
        }
        return null;
    }
}