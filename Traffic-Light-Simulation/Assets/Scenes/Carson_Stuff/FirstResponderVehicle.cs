using UnityEngine;

public enum ResponderType { Ambulance, Police, Fire, Other }

public class FirstResponderVehicle : MonoBehaviour
{
    public string displayName = "Unit";
    public ResponderType type = ResponderType.Other;

    void OnEnable() => TryRegister();

    void Start() => TryRegister(); // fallback if OnEnable ran before registry existed

    void OnDisable()
    {
        ResponderRegistry.Instance?.Unregister(this);
    }

    void TryRegister()
    {
        ResponderRegistry.Instance?.Register(this);
    }
}