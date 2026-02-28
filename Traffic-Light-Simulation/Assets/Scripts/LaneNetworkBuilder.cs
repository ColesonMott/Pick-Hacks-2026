using UnityEngine;

public class LaneNetworkBuilder : MonoBehaviour
{
    public float forwardCheckDistance = 8f;

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
            if (startNode.nodeType != LaneNode.NodeType.Start)
                continue;

            if (startNode == endNode)
                continue;

            Vector3 toStart = startNode.transform.position - endNode.transform.position;

            float forwardDot = Vector3.Dot(endNode.transform.forward, toStart.normalized);

            // Must be roughly in front
            if (forwardDot < 0.3f)
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