#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CarSpawnerAutoAssign
{
    [MenuItem("Tools/Assign All LaneNodes to CarSpawner")]
    static void AssignAllNodes()
    {
        CarSpawner spawner = GameObject.FindObjectOfType<CarSpawner>();
        if (spawner == null)
        {
            Debug.LogWarning("No CarSpawner found in the scene!");
            return;
        }

        LaneNode[] allNodes = GameObject.FindObjectsOfType<LaneNode>();
        spawner.allLaneNodes.Clear();
        spawner.allLaneNodes.AddRange(allNodes);

        Debug.Log("Assigned " + allNodes.Length + " LaneNodes to CarSpawner.");
    }
}
#endif