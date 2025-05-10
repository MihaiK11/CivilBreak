using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 2f;
    public float mouseSensitivity = 2f;
    public float jumpHeight = 1.2f;
    public Transform orientation;          // Rotates with mouse (horizontal)
    public Transform cameraFollowTarget;   // Used by Cinemachine (vertical)

    private CharacterController controller;
    private float verticalRotation = 0f;
    private float gravity = -9.81f;
    private Vector3 velocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 0.25f;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        orientation.Rotate(Vector3.up * mouseX);
        cameraFollowTarget.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = orientation.forward * v + orientation.right * h;
        move.y = 0;

        controller.Move(move.normalized * speed * Time.deltaTime);

        // Check if grounded
        if (controller.isGrounded)
        {
    
            if (velocity.y < 0)
                velocity.y = -2f; // Stick to ground

            // Jump
            if (Input.GetButtonDown("Jump"))
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}