using UnityEngine;
using System.Collections.Generic;

public class RoadClosureManager : MonoBehaviour
{
    public static RoadClosureManager Instance;

    private List<Collider> closedRoads = new List<Collider>();

    void Awake()
    {
        Instance = this;
    }

    public void CloseRoad(Collider roadCollider)
    {
        if (!closedRoads.Contains(roadCollider))
            closedRoads.Add(roadCollider);
    }

    public bool IsRoadClosed(Collider roadCollider)
    {
        return closedRoads.Contains(roadCollider);
    }
}