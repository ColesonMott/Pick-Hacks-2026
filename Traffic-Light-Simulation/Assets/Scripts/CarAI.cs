using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CarAI : MonoBehaviour
{
    private CarSpawner spawner;
    private LaneNode currentNode;
    private NavMeshAgent agent;
    private Queue<LaneNode> currentPath = new Queue<LaneNode>();

    [Header("Driving")]
    public float reachDistance = 1.5f;

    [Header("Traffic")]
    public float detectionDistance = 12f;
    public float stopDistance = 5f;

    [Header("Destination Settings")]
    public float destinationSampleHeight = 10f;
    public float destinationSampleRadius = 75f;
    public float destinationArrivalDistance = 3f;

    private bool isStopped = false;
    private Transform finalDestination;
    private Vector3 snappedDestination;
    private bool hasFinalDestination = false;

    public int allowedLaneArea;
private bool nodeLocked = false;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.autoBraking = true;
        agent.updateRotation = true;
        agent.updatePosition = true;
    }
List<LaneNode> FindPath(LaneNode start, LaneNode goal)
{
    Queue<LaneNode> queue = new Queue<LaneNode>();
    Dictionary<LaneNode, LaneNode> cameFrom = new Dictionary<LaneNode, LaneNode>();

    queue.Enqueue(start);
    cameFrom[start] = null;

    while (queue.Count > 0)
    {
        LaneNode current = queue.Dequeue();

        if (current == goal)
            break;

        foreach (LaneNode next in current.nextNodes)
        {
            if (!cameFrom.ContainsKey(next))
            {
                queue.Enqueue(next);
                cameFrom[next] = current;
            }
        }
    }

    List<LaneNode> path = new List<LaneNode>();

    if (!cameFrom.ContainsKey(goal))
    {
        Debug.LogWarning("No path found.");
        return path;
    }

    LaneNode step = goal;

    while (step != null)
    {
        path.Insert(0, step);
        step = cameFrom[step];
    }

    return path;
}
LaneNode FindClosestNodeToPosition(Vector3 pos)
{
    LaneNode[] nodes = FindObjectsOfType<LaneNode>();

    LaneNode best = null;
    float bestDist = Mathf.Infinity;

    foreach (LaneNode node in nodes)
    {
        float dist = Vector3.Distance(pos, node.transform.position);
        if (dist < bestDist)
        {
            bestDist = dist;
            best = node;
        }
    }

    return best;
}
    public void Initialize(LaneNode startNode, CarSpawner ownerSpawner)
    {
        spawner = ownerSpawner;
        currentNode = startNode;

        MoveToCurrentNode();
    }

    void Update()
{
    if (agent == null || !agent.isOnNavMesh)
        return;

    HandleTraffic();

    if (currentNode == null)
        return;

    if (!agent.pathPending)
    {
        float dist = Vector3.Distance(
            transform.position,
            currentNode.transform.position
        );

        // Only trigger once when truly near node
        if (dist <= reachDistance && !nodeLocked)
        {
            nodeLocked = true;
            AdvanceNode();
        }

        // Unlock once we move away from node
        if (dist > reachDistance + 1f)
        {
            nodeLocked = false;
        }
    }
}

    // ===== DESTINATION ROUTING =====

    public void SetBuildingDestination(Transform destination)
    {
        finalDestination = destination;

        Vector3 searchPosition =
            destination.position + Vector3.up * destinationSampleHeight;

        if (!NavMesh.SamplePosition(
            searchPosition,
            out NavMeshHit hit,
            destinationSampleRadius,
            NavMesh.AllAreas))
        {
            Debug.LogWarning("No NavMesh found near destination.");
            return;
        }

        snappedDestination = hit.position;

        if (!agent.isOnNavMesh)
            return;

        NavMeshPath testPath = new NavMeshPath();
        agent.CalculatePath(snappedDestination, testPath);

        if (testPath.status == NavMeshPathStatus.PathComplete)
        {
            hasFinalDestination = true;
            agent.SetDestination(snappedDestination);
        }
        else
        {
            Debug.LogWarning("Invalid path to destination.");
        }
    }

    // ===== LANE MOVEMENT =====

    void MoveToCurrentNode()
    {
        if (currentNode != null && agent.isOnNavMesh)
        {
            agent.SetDestination(currentNode.transform.position);
        }
    }

void AdvanceNode()
{
    if (currentNode == null)
        return;

    // ===== START → MUST GO TO END (same lane only)
    if (currentNode.nodeType == LaneNode.NodeType.Start)
    {
        LaneNode[] siblings =
            currentNode.transform.parent.GetComponentsInChildren<LaneNode>();

        foreach (LaneNode node in siblings)
        {
            if (node.nodeType == LaneNode.NodeType.End)
            {
                currentNode = node;
                MoveToCurrentNode();
                return;
            }
        }

        Debug.LogError("START had no END sibling.");
        return;
    }

    // ===== END → ONLY ALLOW FORWARD-FACING START NODES
    if (currentNode.nodeType == LaneNode.NodeType.End)
    {
        LaneNode chosen = null;
        float bestForwardDistance = Mathf.Infinity;

        foreach (LaneNode node in currentNode.nextNodes)
        {
            // MUST be Start
            if (node.nodeType != LaneNode.NodeType.Start)
                continue;

            Vector3 toNode =
                node.transform.position - currentNode.transform.position;

            Vector3 dirToNode = toNode.normalized;

            // MUST be physically in front
            float forwardDot =
                Vector3.Dot(currentNode.transform.forward, dirToNode);

            if (forwardDot < 0.5f)
                continue;

            // MUST face same direction (no opposite lane entry)
            float laneAlignment =
                Vector3.Dot(currentNode.transform.forward,
                            node.transform.forward);

            if (laneAlignment <= 0f)
                continue;

            float forwardDistance =
                Vector3.Dot(currentNode.transform.forward, toNode);

            if (forwardDistance > 0 &&
                forwardDistance < bestForwardDistance)
            {
                bestForwardDistance = forwardDistance;
                chosen = node;
            }
        }

        if (chosen == null)
        {
            Debug.LogWarning("No legal forward lane found.");
            return;
        }

        currentNode = chosen;
        MoveToCurrentNode();
    }
}

    // ===== TRAFFIC DETECTION =====

    void HandleTraffic()
    {
        Ray ray = new Ray(
            transform.position + Vector3.up * 0.5f,
            transform.forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionDistance))
        {
            if (hit.collider.CompareTag("Car") &&
                hit.distance < stopDistance)
            {
                agent.isStopped = true;
                isStopped = true;
                return;
            }
        }

        if (isStopped)
        {
            agent.isStopped = false;
            isStopped = false;
        }
    }

    // ===== DESPAWN =====

    void Despawn()
    {
        if (spawner != null)
            spawner.NotifyCarDestroyed();

        Destroy(gameObject);
    }
}