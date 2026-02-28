using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static List<Transform> buildingEntrances = new List<Transform>();

    void Awake()
    {
        buildingEntrances.Clear();

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        foreach (GameObject b in buildings)
        {
            buildingEntrances.Add(b.transform);
        }

        Debug.Log("Building entrances registered: " + buildingEntrances.Count);
    }
}