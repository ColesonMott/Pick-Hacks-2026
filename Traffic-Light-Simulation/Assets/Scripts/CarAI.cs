using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private CarSpawner spawner;

    private LaneNode currentNode;

    [Header("Driving")]
    public float driveSpeed = 8f;
    public float turnSpeed = 6f;
    public float reachDistance = 1.5f;

    [Header("Traffic")]
    public float detectionDistance = 12f;
    public float stopDistance = 5f;

    private bool isStopped = false;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
    }

    public void Initialize(LaneNode startNode, CarSpawner ownerSpawner)
    {
        spawner = ownerSpawner;

        if (startNode == null)
            return;

        // Immediately move to the first next node
        if (startNode.nextNodes.Count > 0)
        {
            currentNode = startNode.nextNodes[
                Random.Range(0, startNode.nextNodes.Count)
            ];
        }
        else
        {
            // If no connections exist, destroy safely
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (currentNode == null)
            return;

        HandleTraffic();

        if (!isStopped)
            Drive();
    }

    void Drive()
    {
        Vector3 target = currentNode.transform.position;
        Vector3 dir = target - transform.position;
        dir.y = 0f;

        float distance = dir.magnitude;

        if (distance < reachDistance)
        {
            if (currentNode.nextNodes.Count == 0)
            {
                // if (spawner != null)
                //     spawner.NotifyCarDestroyed();

                // Destroy(gameObject);
                return;
            }

            currentNode = currentNode.nextNodes[
                Random.Range(0, currentNode.nextNodes.Count)
            ];

            return;
        }

        dir.Normalize();

        transform.position += dir * driveSpeed * Time.deltaTime;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rot,
                Time.deltaTime * turnSpeed
            );
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
                isStopped = true;
                return;
            }
        }

        isStopped = false;
    }
}