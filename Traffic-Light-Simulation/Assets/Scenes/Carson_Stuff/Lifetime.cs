using UnityEngine;

public class VehicleLifetime : MonoBehaviour
{
    public float SimSeconds { get; private set; }

    void Update()
    {
        // Scales with Time.timeScale (your sim speed)
        SimSeconds += Time.deltaTime;
    }
}