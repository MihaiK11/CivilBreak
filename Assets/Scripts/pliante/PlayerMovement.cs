using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 0.5f;
    public float mouseSensitivity = 0.2f;
    public Transform orientation;          // rotates with mouse (horizontal and vertical)
    public Transform cameraFollowTarget;   // used by Cinemachine

    float verticalRotation = 0;
    float horizontalRotation = 0;

    void Start()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {   
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 0.25f;

        horizontalRotation += mouseX;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        // Apply rotations
        orientation.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
        cameraFollowTarget.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);

        // Movement
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 direction = orientation.forward * v + orientation.right * h;
        direction.y = 0;

        transform.position += direction.normalized * speed * Time.deltaTime;
    }
}
