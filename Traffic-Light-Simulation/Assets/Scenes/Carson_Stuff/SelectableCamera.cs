using UnityEngine;

public class SelectableCamera : MonoBehaviour
{
    [Header("References")]
    public TopDownOrbitCam orbitCam;     // drag Main Camera's TopDownOrbitCam here (or auto-find)

    [Header("Overview (when nothing selected)")]
    public Transform overviewMount;      // optional: empty GameObject where the camera sits in overview mode
    public bool startInOverview = true;

    [Header("Keys")]
    public KeyCode deselectKey = KeyCode.Escape;  // go back to overview
    public KeyCode snapKey = KeyCode.F;           // snap orbit center back to target

    void Awake()
    {
        if (orbitCam == null)
            orbitCam = GetComponent<TopDownOrbitCam>();

        // Ensure the singleton is set (your TopDownOrbitCam already does this, but extra-safe)
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
        // Deselect / go back to overview
        if (Input.GetKeyDown(deselectKey))
            GoToOverview();

        // Optional: force snap back to selected vehicle orbit center
        if (Input.GetKeyDown(snapKey) && orbitCam != null && orbitCam.target != null)
            orbitCam.ForceSnapToTarget();
    }

    /// <summary>
    /// Select a target vehicle for the orbit cam to follow/orbit.
    /// Call this from SelectableVehicle.
    /// </summary>
    public void Select(Transform vehicle)
    {
        if (orbitCam == null) return;
        orbitCam.SetTarget(vehicle);
    }

    /// <summary>
    /// Return to overview mode (no target selected).
    /// </summary>
    public void GoToOverview()
    {
        if (orbitCam == null) return;

        orbitCam.ClearTarget();

        // If you gave an overview mount, move camera there
        if (overviewMount != null)
            transform.SetPositionAndRotation(overviewMount.position, overviewMount.rotation);
    }
}