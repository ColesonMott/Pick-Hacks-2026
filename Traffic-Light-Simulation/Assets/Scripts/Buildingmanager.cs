using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static List<Transform> buildingEntrances = new List<Transform>();

    void Awake()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        foreach (GameObject b in buildings)
        {
            buildingEntrances.Add(b.transform);
        }

        Debug.Log("Registered Buildings: " + buildingEntrances.Count);
    }
}