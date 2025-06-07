using UnityEngine;

public class PoliceWalker : MonoBehaviour
{
    public Transform startPoint;     // Punctul de plecare
    public Transform endPoint;       // Punctul de sosire
    public float speed = 2.0f;

    private Transform target;        // Ținta curentă

    void Start()
    {
        // Setăm poziția inițială la startPoint
        if (startPoint != null)
            transform.position = startPoint.position;

        target = endPoint; // Ținta inițială e endPoint
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        // Mergem spre țintă
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Rotire către direcție
        Vector3 dir = target.position - transform.position;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5);

        // Dacă am ajuns la țintă, schimbăm ținta
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            // Dacă era la endPoint, se întoarce la startPoint și invers
            target = (target == endPoint) ? startPoint : endPoint;
        }
    }
}
