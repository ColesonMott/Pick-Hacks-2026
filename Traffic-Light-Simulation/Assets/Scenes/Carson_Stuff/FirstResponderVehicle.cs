using UnityEngine;

public enum ResponderType
{
    Ambulance,
    Fire,
    Police,
    Other
}

public class FirstResponderVehicle : MonoBehaviour
{
    [Header("Responder Identity")]
    public string displayName = "Unit";
    public ResponderType type = ResponderType.Ambulance;

    [Header("Optional status text for UI")]
    [TextArea]
    public string status;

    private void OnEnable()
    {
        // Register with the global registry when spawned/enabled
        if (ResponderRegistry.Instance != null)
            ResponderRegistry.Instance.Register(this);
    }

    private void OnDisable()
    {
        // Unregister when destroyed/disabled
        if (ResponderRegistry.Instance != null)
            ResponderRegistry.Instance.Unregister(this);
    }

    /// <summary>
    /// Call this when name/status changes and you want UI to refresh.
    /// </summary>
    public void NotifyChanged()
    {
        if (ResponderRegistry.Instance != null)
            ResponderRegistry.Instance.NotifyChanged(this);
    }
}