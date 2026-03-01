using System;
using System.Collections.Generic;
using UnityEngine;

public class ResponderRegistry : MonoBehaviour
{
    public static ResponderRegistry Instance { get; private set; }

    // Events so UI updates instantly (no polling)
    public event Action<FirstResponderVehicle> OnAdded;
    public event Action<FirstResponderVehicle> OnRemoved;
    public event Action<FirstResponderVehicle> OnChanged;

    private readonly List<FirstResponderVehicle> responders = new List<FirstResponderVehicle>();
    public IReadOnlyList<FirstResponderVehicle> Responders => responders;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        foreach (var v in FindObjectsOfType<FirstResponderVehicle>())
            Register(v);
    }

    public void Register(FirstResponderVehicle v)
    {
        if (v == null) return;
        if (responders.Contains(v)) return;

        responders.Add(v);
        OnAdded?.Invoke(v);
    }

    public void Unregister(FirstResponderVehicle v)
    {
        if (v == null) return;

        if (responders.Remove(v))
            OnRemoved?.Invoke(v);
    }

    // Call this when a unit’s status/name changes and you want UI to refresh
    public void NotifyChanged(FirstResponderVehicle v)
    {
        if (v == null) return;
        OnChanged?.Invoke(v);
    }
}