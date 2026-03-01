using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class CarAI : MonoBehaviour
{
    [Header("Navigation")]
    public float reachDistance = 0.5f;       // Distance to consider "reached" a node
    public float laneOffset = 1.5f;          // Offset for lane

    private LaneNode currentNode;
    private NavMeshAgent agent;

    private int laneIndex = 0;               // Lane offset index

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    /// <summary>
    /// Initialize the car at a Start node
    /// </summary>
    public void Initialize(LaneNode startNode)
    {
        if (startNode == null || startNode.nextNodes.Count == 0)
        {
            Debug.LogWarning($"{name} initialized at invalid node!");
            return;
        }

        currentNode = startNode;
        laneIndex = startNode.laneIndex;

        // Offset spawn along lane
        Vector3 spawnPos = startNode.transform.position + startNode.transform.right * laneOffset * laneIndex;
        transform.position = spawnPos;

        MoveToCurrentNode();

        Debug.Log($"{name} initialized at {currentNode.name}, lane {laneIndex}");
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        // Check if agent has reached destination
        if (!agent.pathPending && agent.remainingDistance <= reachDistance)
        {
            AdvanceNode();
        }
    }

    /// <summary>
    /// Move the agent to the current node
    /// </summary>
    void MoveToCurrentNode()
    {
        if (currentNode == null || agent == null || !agent.isOnNavMesh) return;

        // Add lane offset
        Vector3 targetPos = currentNode.transform.position + currentNode.transform.right * laneOffset * laneIndex;

        agent.SetDestination(targetPos);

        // Rotate the car to face the road
        if (currentNode.nextNodes.Count > 0)
        {
            Vector3 forward = (currentNode.nextNodes[0].transform.position - targetPos).normalized;
            if (forward != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        Debug.DrawLine(transform.position, targetPos, Color.red, 0.1f);
    }

    /// <summary>
    /// Advance to the next node in the network
    /// </summary>
    void AdvanceNode()
    {
        if (currentNode == null) return;

        if (currentNode.nextNodes.Count == 0)
        {
            Debug.LogWarning($"{name} reached node {currentNode.name} but has no next nodes!");
            return;
        }

        // Randomly pick a next node (or pick first for lane-following)
        currentNode = currentNode.nextNodes[Random.Range(0, currentNode.nextNodes.Count)];

        MoveToCurrentNode();
        Debug.Log($"{name} advancing to {currentNode.name}");
    }
}