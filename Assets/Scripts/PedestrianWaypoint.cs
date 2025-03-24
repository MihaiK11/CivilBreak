using UnityEngine;
using System.Collections.Generic;

public class PedestrianWaypoint : MonoBehaviour
{
    public List<PedestrianWaypoint> connected;

    public enum GizmoColor
    {
        Red, Blue, Yellow, Green, Purple
    }

    public GizmoColor color = GizmoColor.Red;

    private static readonly Color[] colors = new Color[]
    {
        new Color(1f, 0f, 0f),
        new Color(0f, 0.4f, 1f),
        new Color(1f, 0.9f, 0f),
        new Color(0f, 0.8f, 0.2f),
        new Color(0.7f, 0f, 1f)
    };

    void OnDrawGizmos()
    {
        Gizmos.color = colors[(int)color];
        Gizmos.DrawSphere(transform.position, 0.02f);

        if (connected != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var wp in connected)
                if (wp != null)
                    Gizmos.DrawLine(transform.position, wp.transform.position);
        }
    }
}