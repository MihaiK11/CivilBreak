using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Assign your virtual camera here

    public float moveSpeed = 10f;
    public float zoomSpeed = 40f;
    public float rotationSpeed = 300f;

    public float minFOV = 10f;
    public float maxFOV = 80f;

    public Vector2 xBounds = new Vector2(-40f, 20.5f);
    public Vector2 zBounds = new Vector2(-31f, 38f);

    private bool isCursorVisible = false;

    private Vector3 initialCameraPosition;  // Store the starting position of the camera
    private Quaternion initialCameraRotation; // Store the starting rotation of the camera

    void Start()
    {
        if (virtualCamera == null)
        {
            Debug.LogError("Virtual Camera is not assigned!");
            enabled = false;
            return;
        }

        // Store the camera's initial position and rotation
        initialCameraPosition = transform.position;
        initialCameraRotation = transform.rotation;

        transform.rotation = Quaternion.Euler(33.341f, 90f, 0f);
        ToggleCursor(false);
    }

    void Update()
    {
        // Dacă UI-ul de tastare e activ, nu facem nimic (camera nu se mișcă sau rotește)
        if (Click.IsUIActive)
            return;

        HandleCursorToggle();
        HandleMovement();
        HandleZoom();
        HandleRotation();
        ClampPosition();
    }

    void HandleCursorToggle()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            if (!isCursorVisible)
                ToggleCursor(true);
        }
        else
        {
            if (isCursorVisible)
                ToggleCursor(false);
        }
    }

    void ToggleCursor(bool visible)
    {
        Cursor.visible = visible;
        Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
        isCursorVisible = visible;
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Get the camera's forward and right vectors (relative to camera's orientation)
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Flatten Y to keep movement horizontal (no vertical movement)
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Calculate movement based on camera's orientation
        Vector3 move = (right * h + forward * v).normalized * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            float newFOV = virtualCamera.m_Lens.FieldOfView - scroll * zoomSpeed;
            virtualCamera.m_Lens.FieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
        }
    }

    void HandleRotation()
    {
        if (!isCursorVisible)
        {
            // Get mouse movement
            float mouseX = Input.GetAxis("Mouse X");

            // Adjust rotation speed based on FOV (zoom level)
            float adjustedRotationSpeed = rotationSpeed * (virtualCamera.m_Lens.FieldOfView / maxFOV);

            // Apply rotation based on mouse X movement
            transform.Rotate(Vector3.up, mouseX * adjustedRotationSpeed * Time.deltaTime, Space.World);

            // Fix camera pitch (X axis) to keep it horizontal
            Vector3 euler = transform.rotation.eulerAngles;
            euler.x = 33.341f;
            euler.z = 0f;
            transform.rotation = Quaternion.Euler(euler);
        }
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, xBounds.x, xBounds.y);
        pos.z = Mathf.Clamp(pos.z, zBounds.x, zBounds.y);
        transform.position = pos;
    }
}
