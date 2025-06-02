using UnityEngine;
using System.Collections.Generic;

public class PedestrianAIPrimarie : MonoBehaviour
{
    [Header("Параметры движения")]
    public float walkSpeed = 0.5f;
    public float rotationSpeed = 300f;

    [Header("Вейпоинты")]
    public PedestrianWaypoint currentTarget;
    private PedestrianWaypoint previousTarget;

    [Header("Точность попадания в точку")]
    public float reachThreshold = 0.04f;

    // Jump variables
    private bool isJumping = false;
    private float jumpHeight = 0.5f;
    private float jumpSpeed = 2f;
    private float jumpProgress = 0f;
    private Vector3 jumpStartPos;

    void Update()
    {
        if (isJumping)
        {
            JumpMovement();
            return; // Skip normal movement while jumping
        }

        if (currentTarget == null) return;

        Vector3 direction = currentTarget.transform.position - transform.position;
        float distance = direction.magnitude;

        if (distance < reachThreshold)
        {
            // Stay still here
            return;
        }

        // Rotate towards target smoothly (optional)
        Vector3 lookDir = new Vector3(direction.x, 0f, direction.z);
        if (lookDir != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDir);
        }

        // Move towards the target
        transform.position += direction.normalized * walkSpeed * Time.deltaTime;

        // Example: Press J to jump (for testing)
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartJump();
        }
    }

    void JumpMovement()
    {
        jumpProgress += Time.deltaTime * jumpSpeed;
        float height = Mathf.Sin(jumpProgress * Mathf.PI) * jumpHeight; // Up and down smoothly

        Vector3 pos = jumpStartPos;
        pos.y += height;
        transform.position = pos;

        if (jumpProgress >= 1f)
        {
            isJumping = false;
            transform.position = jumpStartPos; // Reset to ground position
        }
    }

    public void StartJump()
    {
        if (isJumping) return; // Already jumping, ignore

        isJumping = true;
        jumpProgress = 0f;
        jumpStartPos = transform.position;
    }

    void ChooseNextWaypoint()
    {
        if (currentTarget.connected == null || currentTarget.connected.Count == 0) return;

        List<PedestrianWaypoint> options = new List<PedestrianWaypoint>(currentTarget.connected);
        if (previousTarget != null && options.Count > 1)
            options.Remove(previousTarget);

        previousTarget = currentTarget;
        currentTarget = options[Random.Range(0, options.Count)];
    }
}
