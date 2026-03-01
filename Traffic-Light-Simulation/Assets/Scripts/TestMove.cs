using UnityEngine;
using UnityEngine.AI;

public class TestMove : MonoBehaviour
{
    public Transform targetNode; // assign a LaneNode in Inspector
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(targetNode.position);
            Debug.Log($"Moving to {targetNode.name}");
        }
        else
        {
            Debug.LogWarning("Agent not on NavMesh!");
        }
    }
}