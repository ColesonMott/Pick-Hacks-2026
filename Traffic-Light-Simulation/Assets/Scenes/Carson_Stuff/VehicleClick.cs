using UnityEngine;

public class SelectableVehicle : MonoBehaviour
{
    public Transform vehicleRoot; // the actual vehicle

    private void OnMouseDown()
    {
        SelectableCamera cam =
            Camera.main.GetComponent<SelectableCamera>();

        if (cam != null && vehicleRoot != null)
        {
            cam.Select(vehicleRoot);
        }
    }
}