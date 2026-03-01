using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region SHARED TYPES

/// <summary>
/// Public light color enum that cars / other systems can use.
/// </summary>
public enum LightColor
{
    Red,
    Yellow,
    Green
}

#endregion

#region DATA CLASSES

[System.Serializable]
public class TrafficSignal
{
    [Tooltip("Logical name, e.g. 'North Straight', 'East Left'")]
    public string Name;   // e.g. "North Straight", "East Left"

    [Header("Light Meshes")]
    public MeshRenderer redLight;
    public MeshRenderer yellowLight;
    public MeshRenderer greenLight;

    [HideInInspector]
    public LightColor CurrentColor = LightColor.Red;
}

[System.Serializable]
public class TrafficPhase
{
    [Tooltip("Descriptive name for this phase, e.g. 'North-South Straight'")]
    public string PhaseName;

    [Tooltip("All signals that are green/yellow together in this phase")]
    public List<TrafficSignal> Signals = new List<TrafficSignal>();

    [Header("Timing (seconds)")]
    public float greenTime = 10f;
    public float yellowTime = 3f;
}

#endregion

public class ComplicatedTrafficController : MonoBehaviour
{
    private Coroutine normalCycle;
    private bool isEmergencyActive = false;

    [Header("Materials")]
    public Material lightsOnMat;
    public Material lightsOffMat;

    [Header("Phases")]
    public List<TrafficPhase> Phases = new List<TrafficPhase>();

    #region UNITY LIFECYCLE

    private void Awake()
    {
        SetAllRed();
    }

    private void Start()
    {
        StartNormalCycle();
    }

    private void Update()
    {
        // Manual testing keys
        if (Input.GetKeyDown(KeyCode.N))
            ActivateEmergency("North");

        if (Input.GetKeyDown(KeyCode.S))
            ActivateEmergency("South");

        if (Input.GetKeyDown(KeyCode.E))
            ActivateEmergency("East");

        if (Input.GetKeyDown(KeyCode.W))
            ActivateEmergency("West");

        if (Input.GetKeyDown(KeyCode.R))
            ResumeNormal();
    }

    #endregion

    #region NORMAL CYCLE

    private void StartNormalCycle()
    {
        if (normalCycle != null)
            StopCoroutine(normalCycle);

        normalCycle = StartCoroutine(RunPhases());
    }

    private IEnumerator RunPhases()
    {
        while (true)
        {
            foreach (TrafficPhase phase in Phases)
            {
                // GREEN
                SetPhaseState(phase, LightColor.Green);
                yield return new WaitForSeconds(phase.greenTime);

                // YELLOW
                SetPhaseState(phase, LightColor.Yellow);
                yield return new WaitForSeconds(phase.yellowTime);

                // RED
                SetPhaseState(phase, LightColor.Red);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    #endregion

    #region EMERGENCY MODE

    /// <summary>
    /// Immediately turn everything red except the signals whose Name contains the direction string.
    /// Example directions: "North", "South", "East", "West".
    /// </summary>
    public void ActivateEmergency(string direction)
    {
        if (isEmergencyActive) return;

        isEmergencyActive = true;

        if (normalCycle != null)
            StopCoroutine(normalCycle);

        SetAllRed();
        SetDirectionGreen(direction);
    }

    public void ResumeNormal()
    {
        if (!isEmergencyActive) return;

        isEmergencyActive = false;
        StartNormalCycle();
    }

    #endregion

    #region LIGHT CONTROL

    private void SetPhaseState(TrafficPhase phase, LightColor state)
    {
        foreach (TrafficSignal signal in phase.Signals)
        {
            SetSignalState(signal, state);
        }
    }

    private void SetSignalState(TrafficSignal signal, LightColor state)
    {
        // Update logical state
        signal.CurrentColor = state;

        // Apply materials based on state
        if (signal.redLight != null)
        {
            signal.redLight.material = (state == LightColor.Red)
                ? lightsOnMat
                : lightsOffMat;
        }

        if (signal.yellowLight != null)
        {
            signal.yellowLight.material = (state == LightColor.Yellow)
                ? lightsOnMat
                : lightsOffMat;
        }

        if (signal.greenLight != null)
        {
            signal.greenLight.material = (state == LightColor.Green)
                ? lightsOnMat
                : lightsOffMat;
        }
    }

    private void SetAllRed()
    {
        HashSet<TrafficSignal> processedSignals = new HashSet<TrafficSignal>();

        foreach (TrafficPhase phase in Phases)
        {
            foreach (TrafficSignal signal in phase.Signals)
            {
                if (!processedSignals.Contains(signal))
                {
                    SetSignalState(signal, LightColor.Red);
                    processedSignals.Add(signal);
                }
            }
        }
    }

    private void SetDirectionGreen(string direction)
    {
        HashSet<TrafficSignal> processedSignals = new HashSet<TrafficSignal>();

        foreach (TrafficPhase phase in Phases)
        {
            foreach (TrafficSignal signal in phase.Signals)
            {
                if (!processedSignals.Contains(signal) &&
                    !string.IsNullOrEmpty(signal.Name) &&
                    signal.Name.ToLower().Contains(direction.ToLower()))
                {
                    SetSignalState(signal, LightColor.Green);
                    processedSignals.Add(signal);
                }
            }
        }
    }

    #endregion
}