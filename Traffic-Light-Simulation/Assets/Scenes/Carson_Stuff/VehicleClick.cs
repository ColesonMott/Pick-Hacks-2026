using UnityEngine;

public class SelectableVehicle : MonoBehaviour
{
    public Transform vehicleRoot;

    void OnMouseDown()
    {
        var cam = Camera.main.GetComponent<SelectableCamera>();
        if (cam != null && vehicleRoot != null)
            cam.Select(vehicleRoot);
    }
}