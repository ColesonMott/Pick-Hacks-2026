using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResponderDropdownUI : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    // Keeps the same order as dropdown options
    private readonly List<FirstResponderVehicle> units = new();

    Coroutine bindRoutine;

    void Awake()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();
    }

    void OnEnable()
    {
        if (dropdown == null) return;

        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        // Bind when registry is ready (fixes script execution order issues)
        bindRoutine = StartCoroutine(BindWhenReady());
    }

    void OnDisable()
    {
        if (bindRoutine != null) StopCoroutine(bindRoutine);

        if (dropdown != null)
            dropdown.onValueChanged.RemoveListener(OnDropdownChanged);

        Unsubscribe();
    }

    IEnumerator BindWhenReady()
    {
        // Wait until ResponderRegistry exists
        while (ResponderRegistry.Instance == null)
            yield return null;

        Subscribe();

        // Wait one more frame so any FindObjectsOfType/Register calls finish
        yield return null;

        RebuildOptions();
    }

    void Subscribe()
    {
        var reg = ResponderRegistry.Instance;
        if (reg == null) return;

        reg.OnAdded += HandleChanged;
        reg.OnRemoved += HandleChanged;
        reg.OnChanged += HandleChanged;
    }

    void Unsubscribe()
    {
        var reg = ResponderRegistry.Instance;
        if (reg == null) return;

        reg.OnAdded -= HandleChanged;
        reg.OnRemoved -= HandleChanged;
        reg.OnChanged -= HandleChanged;
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

        dropdown.SetValueWithoutNotify(0);
        dropdown.RefreshShownValue();
    }

    void OnDropdownChanged(int index)
    {
        if (index < 0 || index >= units.Count) return;

        var unit = units[index];
        if (unit == null) return;

        if (SelectionService.Instance != null)
            SelectionService.Instance.Select(unit);
        else
            TopDownOrbitCam.Instance?.SetTarget(unit.transform);
    }
}