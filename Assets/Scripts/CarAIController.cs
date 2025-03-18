using UnityEngine;
using System.Collections.Generic;

public class CarAIController : MonoBehaviour
{
    [Header("Параметры движения")]
    public float moveSpeed = 1f;
    public float slowDownDistance = 0.4f;
    public float reachThreshold = 0.1f;

    [Header("Waypoint настройки")]
    public WaypointNode currentTarget;
    [HideInInspector] public WaypointNode previousTarget;

    private void Update()
    {
        if (currentTarget == null) return;

        // Направление до точки
        Vector3 direction = currentTarget.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            Debug.LogWarning($"{gameObject.name}: Target direction too small!");
            return;
        }

        // Замедляемся при приближении
        float speed = moveSpeed;
        float distance = direction.magnitude;
        if (distance < slowDownDistance)
        {
            speed *= 0.5f;
        }

        // Жёстко поворачиваем (без анимации)
        transform.forward = direction.normalized;

        // Двигаем вперёд
        transform.position += transform.forward * speed * Time.deltaTime;

        // Проверка достижения цели
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

        List<WaypointNode> options = currentTarget.connectedWaypoints.FindAll(wp => wp != previousTarget);
        if (options.Count == 0) options = currentTarget.connectedWaypoints;

        previousTarget = currentTarget;
        currentTarget = options[Random.Range(0, options.Count)];
    }
}