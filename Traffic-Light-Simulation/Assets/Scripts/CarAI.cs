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

    [Header("Destination Settings")]
    public float destinationSampleHeight = 10f;
    public float destinationSampleRadius = 75f;

    private bool isStopped = false;
    private Transform finalDestination;
    private Vector3 snappedDestination;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

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
        if (agent == null || !agent.isOnNavMesh)
            return;

        HandleTraffic();

        if (!agent.pathPending && agent.remainingDistance <= reachDistance)
        {
            AdvanceNode();
        }
    }

    // 🔥 FIXED DESTINATION ROUTING
    public void SetBuildingDestination(Transform destination)
    {
        finalDestination = destination;

        Vector3 searchPosition = destination.position + Vector3.up * destinationSampleHeight;

        if (!NavMesh.SamplePosition(
            searchPosition,
            out NavMeshHit hit,
            destinationSampleRadius,
            NavMesh.AllAreas))
        {
            Debug.LogWarning("No NavMesh found near destination building: " + destination.name);
            return;
        }

        snappedDestination = hit.position;

        if (!agent.isOnNavMesh)
            return;

        // Validate path before committing
        NavMeshPath testPath = new NavMeshPath();
        agent.CalculatePath(snappedDestination, testPath);

        if (testPath.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetDestination(snappedDestination);
        }
        else
        {
            Debug.LogWarning("Invalid or partial path to building: " + destination.name);
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
        if (currentNode == null)
            return;

        if (currentNode.nodeType == LaneNode.NodeType.Start)
        {
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
                return;

            currentNode = currentNode.nextNodes[
                Random.Range(0, currentNode.nextNodes.Count)
            ];

            MoveToCurrentNode();
        }
    }

    void HandleTraffic()
    {
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