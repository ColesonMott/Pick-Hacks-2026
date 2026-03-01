using UnityEngine;
using System.Collections.Generic;

public class LaneNetworkBuilder : MonoBehaviour
{
    [Header("Network Settings")]
    public float maxConnectionDistance = 200f;  // max distance to connect End → Start
    public float laneTolerance = 1f;            // lateral distance tolerance for lane matching

    void Start()
    {
        BuildNetwork();
    }

    public void BuildNetwork()
    {
        LaneNode[] nodes = FindObjectsOfType<LaneNode>();
        if (nodes.Length == 0)
        {
            Debug.LogWarning("No LaneNodes found in the scene!");
            return;
        }

        // 1️⃣ Automatically assign laneIndex
        foreach (var node in nodes)
        {
            node.CalculateLaneIndex();
            Debug.Log($"{node.name}: Type={node.nodeType}, Lane={node.laneIndex}");
        }

        // 2️⃣ Clear existing connections
        foreach (var node in nodes)
            node.nextNodes.Clear();

        int connections = 0;

        // 3️⃣ Connect End nodes → nearest Start nodes in the same lane
        foreach (var endNode in nodes)
        {
            if (endNode.nodeType != LaneNode.NodeType.End) continue;

            LaneNode bestStart = null;
            float closestDistance = Mathf.Infinity;

            foreach (var startNode in nodes)
            {
                if (startNode.nodeType != LaneNode.NodeType.Start) continue;

                float lateralOffset = Mathf.Abs(GetLateralOffset(endNode, startNode));
                if (lateralOffset > laneTolerance) continue;

                float dist = Vector3.Distance(endNode.transform.position, startNode.transform.position);
                if (dist < closestDistance && dist <= maxConnectionDistance)
                {
                    closestDistance = dist;
                    bestStart = startNode;
                }
            }

            if (bestStart != null)
            {
                endNode.nextNodes.Add(bestStart);
                connections++;
                Debug.DrawLine(endNode.transform.position, bestStart.transform.position, Color.green, 10f);
                Debug.Log($"End node {endNode.name} -> Start node {bestStart.name}");
            }
            else
            {
                Debug.LogWarning($"No connection found for End node {endNode.name}");
            }
        }

        // 4️⃣ Ensure all Start nodes have at least one next node
        foreach (var startNode in nodes)
        {
            if (startNode.nodeType != LaneNode.NodeType.Start) continue;

            if (startNode.nextNodes.Count == 0)
            {
                LaneNode nearestEnd = null;
                float closestDist = Mathf.Infinity;

                foreach (var endNode in nodes)
                {
                    if (endNode.nodeType != LaneNode.NodeType.End) continue;

                    float dist = Vector3.Distance(startNode.transform.position, endNode.transform.position);
                    if (dist < closestDist && dist <= maxConnectionDistance)
                    {
                        closestDist = dist;
                        nearestEnd = endNode;
                    }
                }

                if (nearestEnd != null)
                {
                    startNode.nextNodes.Add(nearestEnd);
                    connections++;
                    Debug.DrawLine(startNode.transform.position, nearestEnd.transform.position, Color.blue, 10f);
                    Debug.Log($"Start node {startNode.name} -> First End node {nearestEnd.name}");
                }
                else
                {
                    Debug.LogWarning($"Start node {startNode.name} could not find any End node!");
                }
            }
        }

        Debug.Log($"Lane network built. Total connections: {connections}");
    }

    /// <summary>
    /// Calculates lateral (sideways) offset between two nodes in the road plane
    /// </summary>
    float GetLateralOffset(LaneNode a, LaneNode b)
    {
        Vector3 dir = (b.transform.position - a.transform.position).normalized;
        return Vector3.Dot(dir, a.transform.right);
    }
}