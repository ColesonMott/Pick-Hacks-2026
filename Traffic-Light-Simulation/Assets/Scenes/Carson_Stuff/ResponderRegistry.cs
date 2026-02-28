using System.Collections.Generic;
using UnityEngine;

public class ResponderRegistry : MonoBehaviour
{
    public static ResponderRegistry Instance;

    public readonly List<FirstResponderVehicle> responders = new List<FirstResponderVehicle>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(FirstResponderVehicle v)
    {
        if (v != null && !responders.Contains(v))
            responders.Add(v);
    }

    public void Unregister(FirstResponderVehicle v)
    {
        responders.Remove(v);
    }
}