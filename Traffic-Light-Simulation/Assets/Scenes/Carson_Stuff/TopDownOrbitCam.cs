using UnityEngine;

public class TopDownOrbitCam : MonoBehaviour
{
    public static TopDownOrbitCam Instance;

    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset = Vector3.up * 1.5f;

    [Header("Orbit")]
    public float distance = 25f;
    public float minDistance = 8f;
    public float maxDistance = 80f;
    public float height = 18f;
    public float minHeight = 6f;
    public float maxHeight = 60f;

    [Header("Controls")]
    public float rotateSpeed = 140f;
    public float zoomSpeed = 12f;
    public float panSpeed = 18f;
    public float smooth = 12f;

    private float yaw = 0f;
    private Vector3 orbitCenter;
    private bool hasTarget = false;

    void Awake()
    {
        // Singleton safety
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (target != null) SetTarget(target);
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
        if (target == null) return;
        orbitCenter = target.position;
    }

    void Update()
    {
        if (!hasTarget) return;

        if (Input.GetKeyDown(KeyCode.F))
            orbitCenter = target.position;

        Vector3 input = new Vector3(
            (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0),
            0f,
            (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0)
        );

        Quaternion yawRot = Quaternion.Euler(0f, yaw, 0f);
        if (input.sqrMagnitude > 0.0001f)
            orbitCenter += (yawRot * input.normalized) * panSpeed * Time.deltaTime;

        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            distance = Mathf.Clamp(distance - scroll * zoomSpeed, minDistance, maxDistance);
            height   = Mathf.Clamp(height   - scroll * (zoomSpeed * 0.6f), minHeight, maxHeight);
        }

        if (Input.GetMouseButton(1))
        {
            float mx = Input.GetAxis("Mouse X");
            yaw += mx * rotateSpeed * Time.deltaTime;
        }

        orbitCenter = Vector3.Lerp(orbitCenter, target.position, 1f - Mathf.Exp(-3f * Time.deltaTime));
    }

    void LateUpdate()
    {
        if (!hasTarget) return;

        Vector3 flatOffset = Quaternion.Euler(0f, yaw, 0f) * new Vector3(0f, 0f, -distance);
        Vector3 desiredPos = orbitCenter + flatOffset + Vector3.up * height;

        transform.position = Vector3.Lerp(transform.position, desiredPos, 1f - Mathf.Exp(-smooth * Time.deltaTime));

        Vector3 lookPoint = orbitCenter + targetOffset;
        Quaternion desiredRot = Quaternion.LookRotation((lookPoint - transform.position).normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, 1f - Mathf.Exp(-smooth * Time.deltaTime));
    }
}