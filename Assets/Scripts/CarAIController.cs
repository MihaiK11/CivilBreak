using UnityEngine;
using System.Collections.Generic;

public class CarAIController : MonoBehaviour
{
    [Header("Параметры движения")]
    public float moveSpeed = 0.8f;
    public float slowDownDistance = 0.5f;
    public float reachThreshold = 0f;

    [Header("Waypoint настройки")]
    public WaypointNode currentTarget;
    [HideInInspector] public WaypointNode previousTarget;

    // Храним историю посещённых поинтов
    private Queue<WaypointNode> waypointHistory = new Queue<WaypointNode>();

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

        List<WaypointNode> options = new List<WaypointNode>(currentTarget.connectedWaypoints);

        // Удаляем предыдущий узел
        if (previousTarget != null && options.Count > 1)
            options.Remove(previousTarget);

        // Исключаем узел, который был ровно 3 точки назад
        WaypointNode thirdBack = GetWaypointFromHistory(3);
        if (thirdBack != null && options.Count > 1)
            options.Remove(thirdBack);

        // Если после фильтрации не осталось — возвращаем всё, кроме предыдущего
        if (options.Count == 0)
        {
            options = new List<WaypointNode>(currentTarget.connectedWaypoints);
            if (previousTarget != null && options.Count > 1)
                options.Remove(previousTarget);
        }

        previousTarget = currentTarget;
        currentTarget = options[Random.Range(0, options.Count)];

        // Обновляем историю
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