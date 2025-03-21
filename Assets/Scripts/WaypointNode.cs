using UnityEngine;
using System.Collections.Generic;

public class WaypointNode : MonoBehaviour
{
    [Header("Подключенные узлы")]
    public List<WaypointNode> connectedWaypoints;

    [Header("Цвет Gizmo (для отладки)")]
    public WaypointColor colorIndex = WaypointColor.Color01;

    /// <summary>
    /// Возвращает случайный подключенный узел, исключая предыдущий (если передан) для предотвращения разворотов.
    /// </summary>
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
        // Выбираем цвет по индексу
        Gizmos.color = GetColorFromIndex(colorIndex);

        // Отрисовка узла
        Gizmos.DrawSphere(transform.position, 0.02f);

        // Отрисовка линий
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

    /// <summary>
    /// Возвращает цвет по индексу.
    /// </summary>
    private Color GetColorFromIndex(WaypointColor index)
    {
        switch (index)
        {
            case WaypointColor.Color01: return Color.red;
            case WaypointColor.Color02: return Color.green;
            case WaypointColor.Color03: return Color.blue;
            case WaypointColor.Color04: return Color.yellow;
            case WaypointColor.Color05: return Color.magenta;
            case WaypointColor.Color06: return Color.cyan;
            case WaypointColor.Color07: return new Color(1f, 0.5f, 0f);  // orange
            case WaypointColor.Color08: return new Color(0.5f, 0f, 1f);  // violet
            case WaypointColor.Color09: return new Color(0f, 1f, 0.5f);  // spring green
            case WaypointColor.Color10: return new Color(1f, 0f, 0.5f);  // pink-red
            case WaypointColor.Color11: return new Color(0.3f, 0.7f, 1f); // light blue
            case WaypointColor.Color12: return new Color(0.7f, 1f, 0.3f); // light green
            case WaypointColor.Color13: return new Color(1f, 0.3f, 0.7f); // rose
            case WaypointColor.Color14: return new Color(0.6f, 0.6f, 0.6f); // gray
            case WaypointColor.Color15: return new Color(0.4f, 0f, 0f); // dark red
            case WaypointColor.Color16: return new Color(0f, 0.4f, 0f); // dark green
            case WaypointColor.Color17: return new Color(0f, 0f, 0.4f); // dark blue
            case WaypointColor.Color18: return new Color(0.8f, 0.8f, 0f); // mustard
            case WaypointColor.Color19: return new Color(0f, 0.8f, 0.8f); // turquoise
            case WaypointColor.Color20: return new Color(0.8f, 0f, 0.8f); // purple
            case WaypointColor.Color21: return new Color(0.9f, 0.4f, 0.4f); // coral
            case WaypointColor.Color22: return new Color(0.4f, 0.9f, 0.4f); // mint
            case WaypointColor.Color23: return new Color(0.4f, 0.4f, 0.9f); // steel blue
            case WaypointColor.Color24: return Color.white;
            default: return Color.black;
        }
    }

    public enum WaypointColor
    {
        Color01,
        Color02,
        Color03,
        Color04,
        Color05,
        Color06,
        Color07,
        Color08,
        Color09,
        Color10,
        Color11,
        Color12,
        Color13,
        Color14,
        Color15,
        Color16,
        Color17,
        Color18,
        Color19,
        Color20,
        Color21,
        Color22,
        Color23,
        Color24
    }
}