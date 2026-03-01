using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionService : MonoBehaviour
{
    public static SelectionService Instance { get; private set; }

    [Header("Camera")]
    public TopDownOrbitCam orbitCam;          // drag Main Camera's TopDownOrbitCam (optional; auto-finds)
    public bool returnToOverviewOnDeselect = true;

    [Header("Input (optional)")]
    public bool enableRightClickDeselect = false;   // set true if you want RMB click to deselect
    public bool ignoreClicksOverUI = true;          // don't deselect when clicking UI

    public FirstResponderVehicle Current { get; private set; }

    // Events for HUD/UI to react
    public event Action<FirstResponderVehicle> OnSelected;
    public event Action<FirstResponderVehicle> OnDeselected;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (orbitCam == null)
            orbitCam = FindObjectOfType<TopDownOrbitCam>();
    }

    void Update()
    {
        if (!enableRightClickDeselect) return;

        if (Input.GetMouseButtonDown(1)) // RMB
        {
            if (ignoreClicksOverUI && IsPointerOverUI())
                return;

            if (Current != null)
                Deselect();
        }
    }

    public void Select(FirstResponderVehicle unit)
    {
        if (unit == null) return;

        Current = unit;

        if (orbitCam == null)
            orbitCam = TopDownOrbitCam.Instance != null ? TopDownOrbitCam.Instance : FindObjectOfType<TopDownOrbitCam>();

        orbitCam?.SetTarget(unit.transform);

        OnSelected?.Invoke(unit);
    }

    public void Deselect()
    {
        var prev = Current;
        Current = null;

        if (orbitCam == null)
            orbitCam = TopDownOrbitCam.Instance != null ? TopDownOrbitCam.Instance : FindObjectOfType<TopDownOrbitCam>();

        if (orbitCam != null)
        {
            orbitCam.ClearTarget();

            if (returnToOverviewOnDeselect && orbitCam.overviewMount != null)
                orbitCam.transform.SetPositionAndRotation(orbitCam.overviewMount.position, orbitCam.overviewMount.rotation);
        }

        OnDeselected?.Invoke(prev);
    }

    public void ToggleSelect(FirstResponderVehicle unit)
    {
        if (unit == null) return;
        if (Current == unit) Deselect();
        else Select(unit);
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }
}