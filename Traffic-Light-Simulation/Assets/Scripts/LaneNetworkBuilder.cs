using UnityEngine;

public class LaneNetworkBuilder : MonoBehaviour
{
    public float forwardCheckDistance = 8f;

    [Header("Direction Rules")]
    public float minForwardDot = 0.3f;     // Must be somewhat in front
    public float oppositeDotThreshold = -0.3f; // Block opposite lanes
    public float minTurnAngle = 60f;       // Allow turns
    public float maxTurnAngle = 120f;

    void Start()
    {
        BuildNetwork();
    }

    void BuildNetwork()
{
    LaneNode[] nodes = FindObjectsOfType<LaneNode>();

    int connections = 0;

    foreach (LaneNode node in nodes)
        node.nextNodes.Clear();

    foreach (LaneNode endNode in nodes)
    {
        if (endNode.nodeType != LaneNode.NodeType.End)
            continue;

        LaneNode bestCandidate = null;
        float bestForwardDistance = Mathf.Infinity;

        foreach (LaneNode startNode in nodes)
        {
            // 🔥 HARD RULE: Only End → Start allowed
            if (startNode.nodeType != LaneNode.NodeType.Start)
                continue;

            if (startNode == endNode)
                continue;

            Vector3 toStart = startNode.transform.position - endNode.transform.position;
            Vector3 dirToStart = toStart.normalized;

            float forwardDot = Vector3.Dot(endNode.transform.forward, dirToStart);

            if (forwardDot < 0.4f)
                continue;

            float laneAlignment = Vector3.Dot(
                endNode.transform.forward,
                startNode.transform.forward
            );

            // 🚫 BLOCK opposite lanes completely
            if (laneAlignment < 0f)
                continue;

            float forwardDistance = Vector3.Dot(endNode.transform.forward, toStart);

            if (forwardDistance > 0 && forwardDistance < bestForwardDistance)
            {
                bestForwardDistance = forwardDistance;
                bestCandidate = startNode;
            }
        }

        if (bestCandidate != null)
        {
            endNode.nextNodes.Add(bestCandidate);
            connections++;
        }
    }

    Debug.Log("Lane network built. Connections: " + connections);
}
}