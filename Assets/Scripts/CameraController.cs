using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float zoomSpeed = 40f;
    public float rotationSpeed = 300f;

    public float minFOV = 10f;
    public float maxFOV = 80f;

    public Vector2 xBounds = new Vector2(-40f, 20.5f);
    public Vector2 zBounds = new Vector2(-31f, 38f);

    private bool isCursorVisible = false;
    private Camera cam;

    // Для плавного перемещения
    private Vector3? targetPosition = null;
    public float smoothMoveSpeed = 5f;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        transform.rotation = Quaternion.Euler(33.341f, 90f, 0f);
        ToggleCursor(false);
    }

    void Update()
    {
        HandleCursorToggle();
        HandleMovement();
        HandleZoom();
        HandleRotation();
        HandleClickToMove();
        SmoothMoveToTarget();
        ClampPosition();
    }

    void HandleCursorToggle()
    {
        if (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand ||Input.GetKey(KeyCode.LeftControl))
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
        if (targetPosition != null) return; // если в процессе переезда, не мешаем

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 right = transform.right; right.y = 0; right.Normalize();
        Vector3 forward = Vector3.Cross(right, Vector3.up); forward.y = 0; forward.Normalize();

        Vector3 move = (right * h + forward * v).normalized * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.fieldOfView -= scroll * zoomSpeed;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minFOV, maxFOV);
        }
    }

    void HandleRotation()
    {
        if (!isCursorVisible && targetPosition == null)
        {
            float mouseX = Input.GetAxis("Mouse X");

            // Динамическая скорость: ближе — медленнее, дальше — быстрее
            float adjustedRotationSpeed = rotationSpeed * (cam.fieldOfView / maxFOV);

            transform.Rotate(Vector3.up, mouseX * adjustedRotationSpeed * Time.deltaTime, Space.World);

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

    void HandleClickToMove()
    {
        if ((Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand)) && Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 500f))
            {
                Vector3 point = hit.point;

                // Проверка: точка должна быть в зоне допустимых координат
                if (point.x >= xBounds.x && point.x <= xBounds.y &&
                    point.z >= zBounds.x && point.z <= zBounds.y)
                {
                    // Переезжаем к точке, оставляя текущую высоту
                    targetPosition = new Vector3(point.x, transform.position.y, point.z);
                }
            }
        }
    }

    void SmoothMoveToTarget()
    {
        if (targetPosition != null)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition.Value, smoothMoveSpeed * Time.deltaTime);

            // Если почти добрались — завершить
            if (Vector3.Distance(transform.position, targetPosition.Value) < 0.1f)
                targetPosition = null;
        }
    }
}