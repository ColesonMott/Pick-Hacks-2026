using UnityEngine;

public class FreeMoveCamera : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 20f;
    public float fastSpeed = 45f;
    public float verticalSpeed = 15f;

    [Header("Mouse Look (MMB)")]
    public float lookSensitivity = 6f;
    public float lookSmoothness = 12f;

    float yaw;
    float pitch;

    Vector2 currentLook;
    Vector2 lookVelocity;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        // Disable free roam while orbit camera is active
        if (TopDownOrbitCam.Instance != null &&
            TopDownOrbitCam.Instance.target != null)
            return;

        Move();
        Look();
    }

    void Move()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : moveSpeed;

        Vector3 move = new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        );

        transform.position += transform.TransformDirection(move) * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.E))
            transform.position += Vector3.up * verticalSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q))
            transform.position += Vector3.down * verticalSpeed * Time.deltaTime;
    }

    void Look()
    {
        if (!Input.GetMouseButton(2))
        {
            // Smoothly slow rotation when releasing mouse
            currentLook = Vector2.Lerp(currentLook, Vector2.zero, Time.deltaTime * lookSmoothness);
            return;
        }

        Vector2 mouseInput = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        ) * lookSensitivity;

        // Smooth mouse movement
        currentLook = Vector2.SmoothDamp(
            currentLook,
            mouseInput,
            ref lookVelocity,
            1f / lookSmoothness
        );

        yaw += currentLook.x;
        pitch -= currentLook.y;

        pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}