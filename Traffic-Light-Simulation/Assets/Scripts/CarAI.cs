using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    NavMeshAgent agent;
    Vector3 destination;

    public void SetDestination(Vector3 target)
    {
        agent = GetComponent<NavMeshAgent>();
        destination = target;
        agent.SetDestination(destination);
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                Destroy(gameObject);
            }
        }
    }
}