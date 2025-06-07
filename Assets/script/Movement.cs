using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform cameraTransform; 

    private Rigidbody rb;
    private float moveX;
    private float moveZ;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Update()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

       
        forward.y = 0;
        right.y = 0;

       
        forward.Normalize();
        right.Normalize();
       
        Vector3 moveDirection = right * moveX + forward * moveZ;
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}
