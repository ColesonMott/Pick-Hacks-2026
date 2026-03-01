using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AmbulanceAI : MonoBehaviour
{
    private NavMeshAgent agent;

    [Header("Route Settings")]
    [Tooltip("Home base (hospital / station) the ambulance returns to.")]
    public Transform baseLocation;

    [Tooltip("Possible destinations (accident scenes, pickup points, etc.).")]
    public List<Transform> possibleDestinations = new List<Transform>();

    [Header("Driving")]
    [Tooltip("How close to the target before we consider it reached and advance the route.")]
    public float reachDistance = 3f;

    [Tooltip("How fast the ambulance visually turns to match its movement direction.")]
    public float rotationSpeed = 8f;

    [Header("Traffic Detection")]
    [Tooltip("How far ahead we look for other cars.")]
    public float detectionDistance = 8f;

    [Tooltip("Distance at which we actually stop for another car.")]
    public float stopDistance = 3f;

    [Tooltip("Height above the ambulance position where the ray/sphere starts.")]
    public float rayHeight = 1.5f;

    [Tooltip("Use a sphere cast instead of a thin ray (recommended).")]
    public bool useSphereCast = true;

    [Tooltip("Radius of the sphere cast when useSphereCast is true.")]
    public float sphereCastRadius = 0.5f;

    [Tooltip("Layers that count as traffic (other cars, etc.).")]
    public LayerMask trafficLayerMask = ~0;

    [Header("Emergency Settings")]
    [Tooltip("How close we need to be to an intersection before we trigger emergency mode there.")]
    public float activationDistance = 30f;

    [Tooltip("How long the intersection should stay in emergency mode before resuming normal.")]
    public float emergencyHoldTime = 5f;

    [Header("Debug")]
    public bool logDebug = false;

    // Route state
    private Transform currentTarget;
    private bool goingToBase = false;

    // Emergency intersection state
    private HashSet<TrafficIntersection> triggeredIntersections = new HashSet<TrafficIntersection>();

    // Traffic state
    private bool stoppedByCar = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            // We control rotation manually so the mesh matches velocity.
            agent.updateRotation = false;
            agent.updateUpAxis = true;
        }
    }

    private void Start()
    {
        SetupInitialRoute();
    }

    private void Update()
    {
        if (agent == null || !agent.enabled)
            return;

        HandleTraffic();
        DrawDebugRay();
        HandleEmergencyIntersections();

        if (!agent.isOnNavMesh)
            return;

        HandleRouteProgress();
        UpdateRotation();
    }

    #region ROUTE LOGIC

    private void SetupInitialRoute()
    {
        if (baseLocation == null && (possibleDestinations == null || possibleDestinations.Count == 0))
        {
            if (logDebug)
                Debug.LogWarning($"[AmbulanceAI] {name}: No baseLocation or possibleDestinations assigned.");
            return;
        }

        // First trip: from base to a random destination.
        if (baseLocation != null && possibleDestinations != null && possibleDestinations.Count > 0)
        {
            goingToBase = false;
            currentTarget = GetRandomDestination();
        }
        else if (possibleDestinations != null && possibleDestinations.Count > 0)
        {
            // No base: just pick a random destination and roam between them.
            goingToBase = false;
            currentTarget = GetRandomDestination();
        }
        else
        {
            // Only base is set: just sit there / do nothing.
            currentTarget = baseLocation;
            goingToBase = true;
        }

        SetDestination(currentTarget);
    }

    private Transform GetRandomDestination()
    {
        if (possibleDestinations == null || possibleDestinations.Count == 0)
            return null;

        int index = Random.Range(0, possibleDestinations.Count);
        return possibleDestinations[index];
    }

    private void HandleRouteProgress()
    {
        if (currentTarget == null || agent.pathPending || !agent.hasPath)
            return;

        float dist = agent.remainingDistance;
        if (float.IsInfinity(dist))
            return;

        if (dist <= reachDistance)
        {
            AdvanceRoute();
        }
    }

    private void AdvanceRoute()
    {
        if (baseLocation == null && (possibleDestinations == null || possibleDestinations.Count == 0))
            return;

        if (baseLocation != null && possibleDestinations != null && possibleDestinations.Count > 0)
        {
            // Ping-pong between base and random destinations.
            if (goingToBase)
            {
                // We arrived at base -> go to a new destination.
                goingToBase = false;
                currentTarget = GetRandomDestination();
            }
            else
            {
                // We arrived at a destination -> go back to base.
                goingToBase = true;
                currentTarget = baseLocation;
            }
        }
        else if (possibleDestinations != null && possibleDestinations.Count > 0)
        {
            // Only destinations, no base: just keep picking random.
            currentTarget = GetRandomDestination();
        }
        else
        {
            // Only base: stay there.
            currentTarget = baseLocation;
        }

        SetDestination(currentTarget);
    }

    private void SetDestination(Transform target)
    {
        if (agent == null || target == null)
            return;

        if (!agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 10f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        if (agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);

            if (logDebug)
                Debug.Log($"[AmbulanceAI] {name}: Destination set to {target.name} at {target.position}");
        }
    }

    #endregion

    #region TRAFFIC (CARS ONLY – IGNORE LIGHTS)

    private void HandleTraffic()
    {
        if (agent == null)
            return;

        Vector3 origin = transform.position + Vector3.up * rayHeight;
        RaycastHit hit;
        bool shouldStop = false;
        stoppedByCar = false;

        bool hitSomething = false;

        if (useSphereCast)
        {
            hitSomething = Physics.SphereCast(
                origin,
                sphereCastRadius,
                transform.forward,
                out hit,
                detectionDistance,
                trafficLayerMask,
                QueryTriggerInteraction.Ignore
            );
        }
        else
        {
            hitSomething = Physics.Raycast(
                origin,
                transform.forward,
                out hit,
                detectionDistance,
                trafficLayerMask,
                QueryTriggerInteraction.Ignore
            );
        }

        if (hitSomething)
        {
            float distance = hit.distance;

            // For ambulance, we don't stop for traffic lights; we only avoid cars.
            if (hit.collider.CompareTag("Car") && distance < stopDistance)
            {
                shouldStop = true;
                stoppedByCar = true;
            }
        }

        if (shouldStop)
        {
            if (!agent.isStopped)
                agent.isStopped = true;
        }
        else
        {
            if (agent.isStopped)
                agent.isStopped = false;
        }
    }

    #endregion

    #region EMERGENCY INTERSECTIONS

    private void HandleEmergencyIntersections()
    {
        if (TrafficIntersection.AllIntersections == null ||
            TrafficIntersection.AllIntersections.Count == 0)
            return;

        float activationSqr = activationDistance * activationDistance;

        foreach (var intersection in TrafficIntersection.AllIntersections)
        {
            if (intersection == null)
                continue;

            if (triggeredIntersections.Contains(intersection))
                continue;

            float sqrDist = (intersection.transform.position - transform.position).sqrMagnitude;
            if (sqrDist > activationSqr)
                continue;

            string direction = GetDirectionName(intersection.transform.position);
            intersection.ActivateEmergency(direction);
            triggeredIntersections.Add(intersection);

            if (logDebug)
                Debug.Log($"[AmbulanceAI] {name}: Activated emergency at {intersection.name} direction {direction}");

            StartCoroutine(ResumeAfterDelay(intersection, emergencyHoldTime));
        }
    }

    private string GetDirectionName(Vector3 intersectionPos)
    {
        Vector3 dir = (intersectionPos - transform.position).normalized;

        if (Mathf.Abs(dir.z) > Mathf.Abs(dir.x))
            return dir.z > 0 ? "North" : "South";
        else
            return dir.x > 0 ? "East" : "West";
    }

    private IEnumerator ResumeAfterDelay(TrafficIntersection intersection, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (intersection != null)
        {
            intersection.ResumeNormal();
            if (logDebug)
                Debug.Log($"[AmbulanceAI] {name}: Resumed normal at {intersection.name}");
        }

        triggeredIntersections.Remove(intersection);
    }

    #endregion

    #region ROTATION & DEBUG

    private void UpdateRotation()
    {
        if (agent == null)
            return;

        Vector3 velocity = agent.velocity;

        // Ignore tiny jitter
        if (velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 flatVel = new Vector3(velocity.x, 0f, velocity.z);
        if (flatVel.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(flatVel.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    private void DrawDebugRay()
    {
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        Vector3 dir = transform.forward * detectionDistance;

        Color color = stoppedByCar ? Color.yellow : Color.green;
        Debug.DrawRay(origin, dir, color);

        if (useSphereCast)
        {
            Vector3 front = origin + transform.forward * Mathf.Min(detectionDistance, stopDistance);
            Debug.DrawLine(origin, front, color);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        Gizmos.DrawLine(origin, origin + transform.forward * detectionDistance);

        if (useSphereCast)
        {
            Vector3 front = origin + transform.forward * Mathf.Min(detectionDistance, stopDistance);
            Gizmos.DrawWireSphere(front, sphereCastRadius);
        }
    }

    public void DispatchToAccident(Transform wreckTransform)
    {
        if (wreckTransform == null)
            return;

        DispatchToAccident(wreckTransform.position);
    }

    public void DispatchToAccident(Vector3 worldPosition)
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogWarning($"[AmbulanceAI] {name}: No NavMeshAgent found, cannot dispatch.");
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(worldPosition);
    }

    #endregion
}