using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetBuilding;

    [Header("Driving")]
    public float reachDistance = 3f;

    [Header("Traffic")]
    public float detectionDistance = 8f;
    public float stopDistance = 3f;

    private bool stopped = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetBuildingDestination(Transform destination)
    {
        targetBuilding = destination;
        SetNextDestination();
    }

    void Update()
    {
        if (!agent.isOnNavMesh || targetBuilding == null)
            return;

        HandleTraffic();

        if (!agent.pathPending && agent.remainingDistance <= reachDistance)
        {
            SetNextDestination();
        }
    }

    void SetNextDestination()
    {
        if (targetBuilding == null)
            return;

        Vector3 targetPosition = targetBuilding.position;

        if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 30f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Intersection"))
            return;

        ChooseTurnTowardTarget();
    }

    void ChooseTurnTowardTarget()
    {
        if (targetBuilding == null)
            return;

        Vector3 toTarget = (targetBuilding.position - transform.position).normalized;

        Vector3 forward = transform.forward;
        Vector3 left = Quaternion.Euler(0, -90f, 0) * forward;
        Vector3 right = Quaternion.Euler(0, 90f, 0) * forward;

        float fDot = Vector3.Dot(forward, toTarget);
        float lDot = Vector3.Dot(left, toTarget);
        float rDot = Vector3.Dot(right, toTarget);

        Vector3 chosen = forward;

        if (lDot > fDot && lDot > rDot)
            chosen = left;
        else if (rDot > fDot && rDot > lDot)
            chosen = right;

        Vector3 newTarget = transform.position + chosen * 40f;

        if (NavMesh.SamplePosition(newTarget, out NavMeshHit hit, 20f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
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
                stopped = true;
                return;
            }
        }

        if (stopped)
        {
            agent.isStopped = false;
            stopped = false;
        }
    }
}