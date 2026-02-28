using UnityEngine;

public class TopDownOrbitCam : MonoBehaviour
{
    public static TopDownOrbitCam Instance;

    [Header("Target")]
    public Transform target;
    public Vector3 lookOffset = Vector3.up * 1.5f;

    [Header("Overview (optional)")]
    public Transform overviewMount;

    [Header("Distance / Height")]
    public float distance = 25f;
    public float minDistance = 8f;
    public float maxDistance = 80f;

    public float height = 18f;
    public float minHeight = 6f;
    public float maxHeight = 60f;

    [Header("Movement")]
    public float rotateSpeed = 140f;   // hold MMB to rotate
    public float zoomSpeed = 12f;      // scroll wheel
    public float panSpeed = 20f;       // WASD pans orbit center
    public float smooth = 10f;

    private float yaw;
    private Vector3 orbitCenter;
    private bool hasTarget = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (target != null)
            SetTarget(target);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        hasTarget = (target != null);

        if (hasTarget)
        {
            orbitCenter = target.position;
            yaw = target.eulerAngles.y;
        }
    }

    public void ClearTarget()
    {
        target = null;
        hasTarget = false;
    }

    public void ForceSnapToTarget()
    {
        if (target != null)
            orbitCenter = target.position;
    }

    void Update()
    {
        // RIGHT CLICK = exit vehicle view
        if (Input.GetMouseButtonDown(1) && hasTarget)
        {
            ClearTarget();

            if (overviewMount != null)
                transform.SetPositionAndRotation(overviewMount.position, overviewMount.rotation);

            return;
        }

        if (!hasTarget) return;

        // WASD pans orbit center
        Vector3 input = new Vector3(
            (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
            0f,
            (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0)
        );

        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);

        if (input.sqrMagnitude > 0.0001f)
            orbitCenter += (yawRot * input.normalized) * panSpeed * Time.deltaTime;

        // Scroll zoom
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.01f)
        {
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
            height   = Mathf.Clamp(height - scroll * (zoomSpeed * 0.6f), minHeight, maxHeight);
        }

        // Hold MIDDLE MOUSE to rotate (MMB = button 2)
        if (Input.GetMouseButton(2))
        {
            float mx = Input.GetAxis("Mouse X");
            yaw += mx * rotateSpeed * Time.deltaTime;
        }

        // Keep orbit centered on moving vehicle
        orbitCenter = Vector3.Lerp(
            orbitCenter,
            target.position,
            1f - Mathf.Exp(-3f * Time.deltaTime)
        );
    }

    void LateUpdate()
    {
        if (!hasTarget) return;

        Vector3 offset = Quaternion.Euler(0f, yaw, 0f) * new Vector3(0f, 0f, -distance);
        Vector3 desiredPos = orbitCenter + offset + Vector3.up * height;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            1f - Mathf.Exp(-smooth * Time.deltaTime)
        );

        Vector3 lookPoint = orbitCenter + lookOffset;
        Quaternion desiredRot = Quaternion.LookRotation(lookPoint - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            1f - Mathf.Exp(-smooth * Time.deltaTime)
        );
    }
}