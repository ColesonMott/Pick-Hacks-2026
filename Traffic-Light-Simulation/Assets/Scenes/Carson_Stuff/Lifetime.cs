using UnityEngine;

public class VehicleLifetime : MonoBehaviour
{
    public float SimSeconds { get; private set; }

    void Update()
    {
        // scales with Time.timeScale
        SimSeconds += Time.deltaTime;
    }
}