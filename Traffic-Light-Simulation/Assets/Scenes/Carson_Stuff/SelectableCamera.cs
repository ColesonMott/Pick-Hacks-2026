using UnityEngine;

public class SelectableCamera : MonoBehaviour
{
    [Header("References")]
    public TopDownOrbitCam orbitCam;

    [Header("Overview (when nothing selected)")]
    public Transform overviewMount;
    public bool startInOverview = true;

    [Header("Keys")]
    public KeyCode deselectKey = KeyCode.Escape;
    public KeyCode snapKey = KeyCode.F;

    void Awake()
    {
        if (orbitCam == null)
            orbitCam = GetComponent<TopDownOrbitCam>();

        if (TopDownOrbitCam.Instance == null && orbitCam != null)
            TopDownOrbitCam.Instance = orbitCam;
    }

    void Start()
    {
        if (startInOverview)
            GoToOverview();
    }

    void Update()
    {
        // ESC = deselect only if something is selected
        if (Input.GetKeyDown(deselectKey) && orbitCam != null && orbitCam.target != null)
            GoToOverview();

        // F = snap back to target
        if (Input.GetKeyDown(snapKey) && orbitCam != null && orbitCam.target != null)
            orbitCam.ForceSnapToTarget();
    }

    public void Select(Transform vehicle)
    {
        if (orbitCam == null) return;
        orbitCam.SetTarget(vehicle);
    }

    public void GoToOverview()
    {
        if (orbitCam == null) return;

        orbitCam.ClearTarget();

        if (overviewMount != null)
            transform.SetPositionAndRotation(overviewMount.position, overviewMount.rotation);
    }
}