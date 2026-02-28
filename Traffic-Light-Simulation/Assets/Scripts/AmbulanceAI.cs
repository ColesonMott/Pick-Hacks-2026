using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class AmbulanceAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Route Settings")]
    public Transform baseLocation;
    public List<Transform> possibleDestinations = new List<Transform>();

    [Header("Emergency Settings")]
    public float activationDistance = 30f;

    private Transform currentTarget;
    private bool returningToBase = false;

    private HashSet<TrafficIntersection> triggeredIntersections =
        new HashSet<TrafficIntersection>();

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent found!");
            return;
        }

        agent.autoBraking = true;
        agent.speed = 12f;           // force consistent speed
        agent.acceleration = 20f;
        agent.angularSpeed = 120f;
        agent.stoppingDistance = 1f;
        agent.avoidancePriority = 0; // highest priority
    }

    void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Ambulance is not on NavMesh!");
            return;
        }

        GoToRandomDestination();
    }

    void Update()
    {
        if (!agent.isOnNavMesh || agent.pathPending)
            return;

        CheckUpcomingIntersections();

        // If reached target
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (returningToBase)
            {
                returningToBase = false;
                GoToRandomDestination();
            }
            else
            {
                GoToBase();
            }
        }
    }

    void GoToRandomDestination()
    {
        if (possibleDestinations.Count == 0)
        {
            Debug.LogWarning("No destinations assigned!");
            return;
        }

        int index = Random.Range(0, possibleDestinations.Count);
        currentTarget = possibleDestinations[index];

        agent.SetDestination(currentTarget.position);
    }

    void GoToBase()
    {
        if (baseLocation == null)
        {
            Debug.LogWarning("Base location not assigned!");
            return;
        }

        returningToBase = true;
        agent.SetDestination(baseLocation.position);
    }

    void CheckUpcomingIntersections()
    {
        foreach (TrafficIntersection intersection in TrafficIntersection.AllIntersections)
        {
            if (triggeredIntersections.Contains(intersection))
                continue;

            float distance = Vector3.Distance(transform.position, intersection.transform.position);

            if (distance < activationDistance)
            {
                string direction = GetTravelDirection(intersection.transform.position);
                intersection.ActivateEmergency(direction);

                triggeredIntersections.Add(intersection);
                StartCoroutine(ResumeAfterDelay(intersection, 6f));
            }
        }
    }

    string GetTravelDirection(Vector3 intersectionPos)
    {
        Vector3 dir = (intersectionPos - transform.position).normalized;

        if (Mathf.Abs(dir.z) > Mathf.Abs(dir.x))
            return dir.z > 0 ? "North" : "South";
        else
            return dir.x > 0 ? "East" : "West";
    }

    System.Collections.IEnumerator ResumeAfterDelay(TrafficIntersection intersection, float delay)
    {
        yield return new WaitForSeconds(delay);
        intersection.ResumeNormal();
        triggeredIntersections.Remove(intersection);
    }
}