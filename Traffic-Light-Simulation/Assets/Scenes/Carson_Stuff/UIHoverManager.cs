using UnityEngine;
using TMPro;

public class VehicleHoverPopupManager : MonoBehaviour
{
    [Header("Raycast")]
    public Camera cam;
    public LayerMask vehicleLayer;
    public float rayDistance = 1000f;

    [Header("Popup UI")]
    public RectTransform popupPanel;      // VehiclePopup (panel)
    public TMP_Text nameText;             // NameText
    public TMP_Text lifetimeText;         // LifetimeText

    [Header("Popup Follow Mouse")]
    public bool followMouse = true;
    public Vector2 mouseOffset = new Vector2(16, -16);

    HoverHighlight currentHighlight;
    VehicleLifetime currentLifetime;
    Transform currentVehicle;

    void Start()
    {
        if (!cam) cam = Camera.main;
        if (popupPanel) popupPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        // Move popup near mouse (optional)
        if (followMouse && popupPanel && popupPanel.gameObject.activeSelf)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                popupPanel.parent as RectTransform,
                Input.mousePosition,
                null,
                out pos
            );
            popupPanel.anchoredPosition = pos + mouseOffset;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, vehicleLayer))
        {
            var vehicleRoot = hit.collider.GetComponentInParent<Transform>();
            if (!vehicleRoot) { ClearHover(); return; }

            if (vehicleRoot != currentVehicle)
            {
                ClearHover();

                currentVehicle = vehicleRoot;
                currentHighlight = vehicleRoot.GetComponentInParent<HoverHighlight>();
                currentLifetime  = vehicleRoot.GetComponentInParent<VehicleLifetime>();

                if (currentHighlight) currentHighlight.SetHighlight(true);

                if (popupPanel)
                    popupPanel.gameObject.SetActive(true);

                if (nameText)
                    nameText.text = vehicleRoot.name;
            }

            // Update timer text while hovering
            if (lifetimeText)
            {
                float secs = currentLifetime ? currentLifetime.SimSeconds : 0f;
                lifetimeText.text = "Lifetime: " + FormatTime(secs);
            }
        }
        else
        {
            ClearHover();
        }
    }

    void ClearHover()
    {
        if (currentHighlight) currentHighlight.SetHighlight(false);

        currentHighlight = null;
        currentLifetime = null;
        currentVehicle = null;

        if (popupPanel)
            popupPanel.gameObject.SetActive(false);
    }

    static string FormatTime(float seconds)
    {
        int total = Mathf.FloorToInt(seconds);
        int h = total / 3600;
        int m = (total % 3600) / 60;
        int s = total % 60;

        if (h > 0) return $"{h:00}:{m:00}:{s:00}";
        return $"{m:00}:{s:00}";
    }
}