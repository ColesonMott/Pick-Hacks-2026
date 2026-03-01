using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    public bool isEmergencyVehicle = false;
    private NavMeshAgent agent;
    private Transform targetBuilding;

    [Header("Driving")]
    public float reachDistance = 3f;

    [Header("Traffic")]
    public float detectionDistance = 8f;
    public float stopDistance = 3f;

    private bool stopped = false;
    [Header("Road Detection")]
public LayerMask roadLayer;

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
        CheckRoadClosure();
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
void CheckRoadClosure()
{
    Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);

    if (Physics.Raycast(ray, out RaycastHit hit, 10f, roadLayer))
    {
        if (!isEmergencyVehicle &&
            RoadClosureManager.Instance.IsRoadClosed(hit.collider))
        {
            agent.ResetPath();
            ChooseTurnTowardTarget();
        }
    }
}

    void ChooseTurnTowardTarget()
{
    if (targetBuilding == null)
        return;

    Vector3 toTarget = (targetBuilding.position - transform.position).normalized;

    Vector3 forward = transform.forward;
    Vector3 left = Quaternion.Euler(0, -90f, 0) * forward;
    Vector3 right = Quaternion.Euler(0, 90f, 0) * forward;

    Vector3[] options = { forward, left, right };

    float bestScore = -999f;
    Vector3 bestDirection = forward;

    foreach (var dir in options)
    {
        if (!IsValidDirection(dir))
            continue;

        float score = Vector3.Dot(dir.normalized, toTarget);

        if (score > bestScore)
        {
            bestScore = score;
            bestDirection = dir;
        }
    }

    Vector3 probe = transform.position + bestDirection.normalized * 40f;

    if (NavMesh.SamplePosition(probe, out NavMeshHit hit, 20f, NavMesh.AllAreas))
    {
        agent.SetDestination(hit.position);
    }
}

bool IsValidDirection(Vector3 direction)
{
    float dot = Vector3.Dot(transform.forward, direction.normalized);

    // 🚫 block reverse or extreme side angle
    if (dot < 0.2f)
        return false;

    return true;
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