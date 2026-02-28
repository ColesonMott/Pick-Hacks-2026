using UnityEngine;

public class BasicCameraMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 25f;
    public float fastMoveMultiplier = 2f;
    public float verticalSpeed = 15f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 3f;
    public float zoomSpeed = 200f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    void HandleMovement()
    {
        float speed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed *= fastMoveMultiplier;

        Vector3 move = Vector3.zero;

        // WASD movement
        if (Input.GetKey(KeyCode.W))
            move += transform.forward;

        if (Input.GetKey(KeyCode.S))
            move -= transform.forward;

        if (Input.GetKey(KeyCode.A))
            move -= transform.right;

        if (Input.GetKey(KeyCode.D))
            move += transform.right;

        // Vertical movement
        if (Input.GetKey(KeyCode.Q))
            move -= transform.up;

        if (Input.GetKey(KeyCode.E))
            move += transform.up;

        transform.position += move * speed * Time.deltaTime;
    }

    void HandleRotation()
    {
        if (Input.GetMouseButton(1)) // Right mouse held
        {
            rotationX += Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
            rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

            rotationY = Mathf.Clamp(rotationY, -80f, 80f);

            transform.rotation = Quaternion.Euler(rotationY, rotationX, 0f);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0f)
        {
            transform.position += transform.forward * scroll * zoomSpeed * Time.deltaTime;
        }
    }
}