using UnityEngine;
using System.Collections.Generic;

public class TrafficIntersection : MonoBehaviour
{
    public static List<TrafficIntersection> AllIntersections 
        = new List<TrafficIntersection>();

    public ComplicatedTrafficController controller;

    private void Awake()
    {
        AllIntersections.Add(this);
    }

    private void OnDestroy()
    {
        AllIntersections.Remove(this);
    }

    public void ActivateEmergency(string direction)
    {
        controller.ActivateEmergency(direction);
    }

    public void ResumeNormal()
    {
        controller.ResumeNormal();
    }
}