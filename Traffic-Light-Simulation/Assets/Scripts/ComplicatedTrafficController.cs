using UnityEngine;
using System.Collections;
using System.Collections.Generic;

<<<<<<< HEAD
#region DATA CLASSES
=======
>>>>>>> main

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
<<<<<<< HEAD
    private bool isEmergencyActive = false;
=======
>>>>>>> main

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
<<<<<<< HEAD
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
=======
        normalCycle = StartCoroutine(RunPhases());
    }

    void Update()
    {
        
        // Example: Press 'E' for emergency mode, 'R' to resume normal
        if (Input.GetKeyDown(KeyCode.N))
        {
            ActivateEmergency("North");
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            ActivateEmergency("South");
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            ActivateEmergency("East");
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            ActivateEmergency("West");
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ResumeNormal();
        }
>>>>>>> main
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

<<<<<<< HEAD
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
=======
    public void ActivateEmergency(string direction)
    {
        if (normalCycle != null)
            StopCoroutine(normalCycle);

        SetAllRed();

        // You implement which signals turn green based on direction
        SetDirectionGreen(direction);
    }

    public void ResumeNormal()
    {
        normalCycle = StartCoroutine(RunPhases());
    }

    void SetAllRed()
    {
        foreach (var phase in Phases)
        {
            foreach (var signal in phase.Signals)
            {
                if (signal.redLight != null)
                    signal.redLight.material = lightsOnMat;

                if (signal.yellowLight != null)
                    signal.yellowLight.material = lightsOffMat;

                if (signal.greenLight != null)
                    signal.greenLight.material = lightsOffMat;
>>>>>>> main
            }
        }
    }

<<<<<<< HEAD
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
=======
    void SetDirectionGreen(string direction)
    {
        foreach (var phase in Phases)
        {
            foreach (var signal in phase.Signals)
            {
                if (signal.Name.Contains(direction))
                {
                    // GREEN for matching direction
                    if (signal.redLight != null)
                        signal.redLight.material = lightsOffMat;

                    if (signal.yellowLight != null)
                        signal.yellowLight.material = lightsOffMat;

                    if (signal.greenLight != null)
                        signal.greenLight.material = lightsOnMat;
>>>>>>> main
                }
            }
        }
    }
<<<<<<< HEAD

    #endregion
=======
>>>>>>> main
}