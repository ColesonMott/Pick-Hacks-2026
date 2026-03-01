using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Put this on a collider at the stop line for a lane.
/// It can be linked to one or more TrafficSignals in the ComplicatedTrafficController.
/// This is useful when a lane can both turn left and go straight, etc.
/// </summary>
public class TrafficSignalProbe : MonoBehaviour
{
    [Tooltip("Which controller drives this intersection. If left empty, will auto-find one in the scene.")]
    public ComplicatedTrafficController controller;

    [Tooltip("Names of TrafficSignals that control this lane (must match TrafficSignal.Name exactly).")]
    public List<string> signalNames = new List<string>();

    private readonly List<TrafficSignal> cachedSignals = new List<TrafficSignal>();

    private void Awake()
    {
        if (controller == null)
        {
            controller = FindObjectOfType<ComplicatedTrafficController>();
        }

        if (controller == null)
        {
            Debug.LogWarning($"{name}: No ComplicatedTrafficController found for TrafficSignalProbe.");
            return;
        }

        CacheSignals();
    }

    private void CacheSignals()
    {
        cachedSignals.Clear();

        if (signalNames == null || signalNames.Count == 0)
        {
            Debug.LogWarning($"{name}: TrafficSignalProbe has no signalNames set.");
            return;
        }

        foreach (string signalName in signalNames)
        {
            if (string.IsNullOrEmpty(signalName))
                continue;

            TrafficSignal found = FindSignal(signalName);
            if (found != null)
            {
                cachedSignals.Add(found);
            }
            else
            {
                Debug.LogWarning($"{name}: Could not find TrafficSignal with Name '{signalName}' on controller {controller.name}");
            }
        }
    }

    private TrafficSignal FindSignal(string name)
    {
        foreach (var phase in controller.Phases)
        {
            foreach (var signal in phase.Signals)
            {
                if (signal != null && signal.Name == name)
                    return signal;
            }
        }

        return null;
    }

    /// <summary>
    /// Aggregate color for this lane:
    /// - If any linked signal is Green -> Green
    /// - Else if any is Yellow -> Yellow
    /// - Else -> Red (or if none found)
    /// </summary>
    public LightColor CurrentColor
    {
        get
        {
            if (cachedSignals.Count == 0)
                return LightColor.Red; // play it safe

            bool anyGreen = false;
            bool anyYellow = false;

            foreach (var s in cachedSignals)
            {
                switch (s.CurrentColor)
                {
                    case LightColor.Green:
                        anyGreen = true;
                        break;
                    case LightColor.Yellow:
                        anyYellow = true;
                        break;
                }
            }

            if (anyGreen)
                return LightColor.Green;
            if (anyYellow)
                return LightColor.Yellow;

            return LightColor.Red;
        }
    }
}