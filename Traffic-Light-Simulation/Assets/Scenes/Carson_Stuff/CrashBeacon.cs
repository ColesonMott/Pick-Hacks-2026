using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Drop this on a "Crash" prefab (e.g., two cars grouped).
/// - Exists + Position are what NavMesh responders can track.
/// - Click anywhere to spawn/move the crash.
/// </summary>
public class CrashBeacon : MonoBehaviour
{
    // Global "latest crash" reference for responders to track
    public static CrashBeacon Current { get; private set; }

    [Header("State")]
    [SerializeField] private bool exists = true;
    public bool Exists => exists;

    public Vector3 Position => transform.position;

    [Header("Click Placement")]
    public Camera clickCamera;
    public LayerMask groundMask = ~0; // default: everything
    public bool placeOnClick = true;

    [Header("NavMesh Snap (optional)")]
    public bool snapToNavMesh = true;
    public float navMeshSampleRadius = 3f;

    void Awake()
    {
        // If you don't assign a camera, use MainCamera
        if (!clickCamera) clickCamera = Camera.main;
    }

    void OnEnable()
    {
        Current = this;
        exists = true;
    }

    void OnDisable()
    {
        if (Current == this) Current = null;
        exists = false;
    }

    void Update()
    {
        if (!placeOnClick) return;
        if (!clickCamera) return;

        // Left click to place/summon crash
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = clickCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundMask))
            {
                Vector3 p = hit.point;

                if (snapToNavMesh && NavMesh.SamplePosition(p, out NavMeshHit nh, navMeshSampleRadius, NavMesh.AllAreas))
                    p = nh.position;

                SummonAt(p);
            }
        }

        // Optional: Right click to "clear" crash
        if (Input.GetMouseButtonDown(1))
        {
            Clear();
        }
    }

    public void SummonAt(Vector3 worldPos)
    {
        transform.position = worldPos;
        exists = true;
        gameObject.SetActive(true);
        Current = this;
    }

    public void Clear()
    {
        exists = false;
        if (Current == this) Current = null;
        gameObject.SetActive(false);
    }
}