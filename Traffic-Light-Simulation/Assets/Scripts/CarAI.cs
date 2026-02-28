using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    private CarSpawner spawner;
    private LaneNode currentNode;
    private NavMeshAgent agent;

    [Header("Driving")]
    public float reachDistance = 1.5f;

    [Header("Traffic")]
    public float detectionDistance = 12f;
    public float stopDistance = 5f;

    private bool isStopped = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Proper NavMeshAgent setup
        agent.autoBraking = true;
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    public void Initialize(LaneNode startNode, CarSpawner ownerSpawner)
    {
        spawner = ownerSpawner;
        currentNode = startNode;

        MoveToCurrentNode();
    }

void Update()
{
    if (currentNode == null || agent == null)
        return;

    if (!agent.isOnNavMesh)
        return;

    HandleTraffic();

    if (!agent.pathPending && agent.remainingDistance <= reachDistance)
    {
        AdvanceNode();
    }
}

    void MoveToCurrentNode()
    {
        if (currentNode != null && agent.isOnNavMesh)
        {
            agent.SetDestination(currentNode.transform.position);
        }
    }

    void AdvanceNode()
    {
        if (currentNode.nodeType == LaneNode.NodeType.Start)
        {
            // Move from Start → matching End on same lane
            LaneNode[] siblings = currentNode.transform.parent.GetComponentsInChildren<LaneNode>();

            foreach (LaneNode node in siblings)
            {
                if (node.nodeType == LaneNode.NodeType.End)
                {
                    currentNode = node;
                    MoveToCurrentNode();
                    return;
                }
            }

            return;
        }

        if (currentNode.nodeType == LaneNode.NodeType.End)
        {
            if (currentNode.nextNodes.Count == 0)
            {
                return; // dead end, just stop
            }

            currentNode = currentNode.nextNodes[
                Random.Range(0, currentNode.nextNodes.Count)
            ];

            MoveToCurrentNode();
        }
    }

    void HandleTraffic()
    {
        if (!agent.isOnNavMesh)
            return;

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, detectionDistance))
        {
            if (hit.collider.CompareTag("Car") && hit.distance < stopDistance)
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
}