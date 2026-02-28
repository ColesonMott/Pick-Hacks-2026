using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#region DATA CLASSES

[System.Serializable]
public class TrafficSignal
{
    public string Name;   // e.g. "North Straight", "East Left"

    public MeshRenderer redLight;
    public MeshRenderer yellowLight;
    public MeshRenderer greenLight;
}

[System.Serializable]
public class TrafficPhase
{
    public string PhaseName;

    public List<TrafficSignal> Signals = new List<TrafficSignal>();

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
                SetPhaseState(phase, LightState.Green);
                yield return new WaitForSeconds(phase.greenTime);

                // YELLOW
                SetPhaseState(phase, LightState.Yellow);
                yield return new WaitForSeconds(phase.yellowTime);

                // RED
                SetPhaseState(phase, LightState.Red);
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    #endregion

    #region EMERGENCY MODE

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

    private enum LightState
    {
        Red,
        Yellow,
        Green
    }

    private void SetPhaseState(TrafficPhase phase, LightState state)
    {
        foreach (TrafficSignal signal in phase.Signals)
        {
            SetSignalState(signal, state);
        }
    }

    private void SetSignalState(TrafficSignal signal, LightState state)
    {
        if (signal.redLight != null)
            signal.redLight.material = (state == LightState.Red) ? lightsOnMat : lightsOffMat;

        if (signal.yellowLight != null)
            signal.yellowLight.material = (state == LightState.Yellow) ? lightsOnMat : lightsOffMat;

        if (signal.greenLight != null)
            signal.greenLight.material = (state == LightState.Green) ? lightsOnMat : lightsOffMat;
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
                    SetSignalState(signal, LightState.Red);
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
                    signal.Name.ToLower().Contains(direction.ToLower()))
                {
                    SetSignalState(signal, LightState.Green);
                    processedSignals.Add(signal);
                }
            }
        }
    }

    #endregion
}