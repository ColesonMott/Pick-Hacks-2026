using UnityEngine;

public enum ResponderType { Ambulance, Police, Fire, Other }

public class FirstResponderVehicle : MonoBehaviour
{
    public string displayName = "Unit";
    public ResponderType type = ResponderType.Other;

    void OnEnable()
    {
        if (ResponderRegistry.Instance != null)
            ResponderRegistry.Instance.Register(this);
    }

    void OnDisable()
    {
        if (ResponderRegistry.Instance != null)
            ResponderRegistry.Instance.Unregister(this);
    }
}