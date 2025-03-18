using UnityEngine;
using System.Collections.Generic;

public class WaypointNode : MonoBehaviour
{
    [Header("Подключенные узлы")]
    public List<WaypointNode> connectedWaypoints;

    /// <summary>
    /// Возвращает случайный подключенный узел, исключая предыдущий (если передан) для предотвращения разворотов.
    /// </summary>
    /// <param name="previous">Узел, который следует исключить</param>
    /// <returns>Случайный валидный узел или null, если список пуст</returns>
    public WaypointNode GetNextWaypoint(WaypointNode previous = null)
    {
        if (connectedWaypoints == null || connectedWaypoints.Count == 0)
            return null;

        List<WaypointNode> validOptions = new List<WaypointNode>(connectedWaypoints);
        if (previous != null && connectedWaypoints.Count > 1)
        {
            validOptions.Remove(previous);
        }

        if (validOptions.Count == 0)
            validOptions = connectedWaypoints;

        return validOptions[Random.Range(0, validOptions.Count)];
    }

    void OnDrawGizmos()
    {
        // Отрисовка узла
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);

        // Отрисовка линий к подключенным узлам
        if (connectedWaypoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var wp in connectedWaypoints)
            {
                if (wp != null)
                    Gizmos.DrawLine(transform.position, wp.transform.position);
            }
        }
    }
}