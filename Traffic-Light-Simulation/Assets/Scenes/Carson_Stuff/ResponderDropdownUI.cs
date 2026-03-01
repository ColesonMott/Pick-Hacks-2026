using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResponderDropdownUI : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    // Keeps the same order as dropdown options
    private readonly List<FirstResponderVehicle> units = new();

    void Awake()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();
    }

    void OnEnable()
    {
        if (dropdown == null) return;

        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        // Subscribe to registry events if available
        if (ResponderRegistry.Instance != null)
        {
            ResponderRegistry.Instance.OnAdded += HandleChanged;
            ResponderRegistry.Instance.OnRemoved += HandleChanged;
            ResponderRegistry.Instance.OnChanged += HandleChanged;
        }

        RebuildOptions();
    }

    void OnDisable()
    {
        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(OnDropdownChanged);

        if (ResponderRegistry.Instance != null)
        {
            ResponderRegistry.Instance.OnAdded -= HandleChanged;
            ResponderRegistry.Instance.OnRemoved -= HandleChanged;
            ResponderRegistry.Instance.OnChanged -= HandleChanged;
        }
    }

    void HandleChanged(FirstResponderVehicle _)
    {
        RebuildOptions();
    }

    void RebuildOptions()
    {
        if (dropdown == null) return;

        dropdown.ClearOptions();
        units.Clear();

        // Option 0 = “none”
        var options = new List<string> { "Select unit..." };
        units.Add(null);

        var reg = ResponderRegistry.Instance;
        if (reg != null)
        {
            foreach (var u in reg.Responders)
            {
                if (u == null) continue;

                options.Add($"{u.displayName} ({u.type})");
                units.Add(u);
            }
        }

        dropdown.AddOptions(options);

        // Reset selection to the placeholder
        dropdown.SetValueWithoutNotify(0);
        dropdown.RefreshShownValue();
    }

    void OnDropdownChanged(int index)
    {
        if (index < 0 || index >= units.Count) return;

        var unit = units[index];
        if (unit == null) return;

        // Focus camera on the unit (choose ONE of these approaches)

        // Preferred (clean):
        if (SelectionService.Instance != null)
            SelectionService.Instance.Select(unit);
        else
            TopDownOrbitCam.Instance?.SetTarget(unit.transform);
    }
}