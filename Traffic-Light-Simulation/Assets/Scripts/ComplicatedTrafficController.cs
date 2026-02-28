using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class TrafficSignal
{
    public string Name;                 // e.g., "North Straight" or "North Left"
    public MeshRenderer redLight;
    public MeshRenderer yellowLight;
    public MeshRenderer greenLight;
}

[System.Serializable]
public class TrafficPhase
{
    public string PhaseName;            // e.g., "North-South Phase"
    public List<TrafficSignal> Signals; // Signals active in this phase
    public float greenTime = 10f;       // Duration green stays on
    public float yellowTime = 3f;       // Duration yellow stays on
}

public class ComplicatedTrafficController : MonoBehaviour
{
    private Coroutine normalCycle;

    [Header("Materials")]
    public Material lightsOnMat;        // Bright material for "on"
    public Material lightsOffMat;       // Dark material for "off"

    [Header("Phases")]
    public List<TrafficPhase> Phases = new List<TrafficPhase>();

    private void Awake()
    {
        // Initialize all signals in all phases to red
        foreach (var phase in Phases)
        {
            foreach (var signal in phase.Signals)
            {
                if (signal.redLight != null) signal.redLight.material = lightsOnMat;
                if (signal.yellowLight != null) signal.yellowLight.material = lightsOffMat;
                if (signal.greenLight != null) signal.greenLight.material = lightsOffMat;
            }
        }
    }

    private void Start()
    {
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
    }

    private IEnumerator RunPhases()
    {
        while (true)
        {
            foreach (TrafficPhase phase in Phases)
            {
                // --- GREEN for this phase ---
                SetPhaseLights(phase, "Green");
                yield return new WaitForSeconds(phase.greenTime);

                // --- YELLOW for this phase ---
                SetPhaseLights(phase, "Yellow");
                yield return new WaitForSeconds(phase.yellowTime);

                // --- RED for this phase ---
                SetPhaseLights(phase, "Red");

                // Optional short buffer before next phase
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private void SetPhaseLights(TrafficPhase phase, string state)
    {
        foreach (var signal in phase.Signals)
        {
            if (signal.redLight != null)
                signal.redLight.material = (state == "Red") ? lightsOnMat : lightsOffMat;
            if (signal.yellowLight != null)
                signal.yellowLight.material = (state == "Yellow") ? lightsOnMat : lightsOffMat;
            if (signal.greenLight != null)
                signal.greenLight.material = (state == "Green") ? lightsOnMat : lightsOffMat;
        }
    }

    // Optional: manually control a single signal outside the phase loop
    public void SetSingleSignal(TrafficSignal signal, string state)
    {
        if (signal.redLight != null)
            signal.redLight.material = (state == "Red") ? lightsOnMat : lightsOffMat;
        if (signal.yellowLight != null)
            signal.yellowLight.material = (state == "Yellow") ? lightsOnMat : lightsOffMat;
        if (signal.greenLight != null)
            signal.greenLight.material = (state == "Green") ? lightsOnMat : lightsOffMat;
    }

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
            }
        }
    }

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
                }
            }
        }
    }
}