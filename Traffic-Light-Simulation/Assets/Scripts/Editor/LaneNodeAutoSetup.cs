#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class LaneNodeAutoSetup
{
    [MenuItem("Tools/Calculate LaneIndex For All LaneNodes")]
    static void AutoLaneIndex()
    {
        // Find all LaneNodes in the active scene
        LaneNode[] allNodes = GameObject.FindObjectsOfType<LaneNode>();
        int count = 0;

        foreach (var node in allNodes)
        {
            node.CalculateLaneIndex(); // calls the function we added in LaneNode
            count++;
        }

        Debug.Log($"LaneIndex calculated for {count} LaneNodes!");
    }
}
#endif