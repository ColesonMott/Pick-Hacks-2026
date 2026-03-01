using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetBuilding;
    private CarSpawner spawner;

    [Header("Driving")]
    [Tooltip("How close to the target building before considering it reached")]
    public float reachDistance = 3f;

    [Tooltip("How close to the target before we despawn the car (and notify spawner)")]
    public float destroyDistance = 2f;

    [Tooltip("How fast the car visually turns to match its movement direction")]
    public float rotationSpeed = 8f;

    [Header("Traffic Detection")]
    [Tooltip("How far ahead we look for traffic lights / cars")]
    public float detectionDistance = 8f;

    [Tooltip("Distance at which we actually stop for an obstacle")]
    public float stopDistance = 3f;

    [Tooltip("Height above the car's position where the ray/sphere starts")]
    public float rayHeight = 1.5f;

    [Tooltip("Use a sphere cast instead of a thin ray (recommended for small cars)")]
    public bool useSphereCast = true;

    [Tooltip("Radius of the sphere cast when useSphereCast is true")]
    public float sphereCastRadius = 0.5f;

    [Tooltip("Layers that count as traffic (cars, stop-line colliders, etc.)")]
    public LayerMask trafficLayerMask = ~0;

    // Debug / state
    private bool stoppedByCar = false;
    private bool stoppedByLight = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // We’ll control rotation ourselves based on velocity.
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = true;
        }

        // Try to find the spawner so we can notify it when we destroy this car.
        spawner = FindObjectOfType<CarSpawner>();
    }

    private void Start()
    {
        // CarSpawner will normally call SetBuildingDestination().
    }

    private void Update()
    {
        if (agent == null || !agent.enabled)
            return;

        HandleTraffic();
        DrawDebugRay();

        // Don’t touch path info if we’re not on a NavMesh yet
        if (!agent.isOnNavMesh)
            return;

        // If we're allowed to move and have a target, handle reaching logic
        if (!agent.isStopped && targetBuilding != null)
        {
            // Only query remainingDistance if we actually have a path
            if (!agent.pathPending && agent.hasPath)
            {
                float dist = agent.remainingDistance;

                if (!float.IsInfinity(dist) && dist <= reachDistance)
                {
                    PickRandomBuildingDestination();
                }
            }
        }

        // Now do rotation + optional despawn check
        UpdateRotation();
        TryDestroyWhenArrived();
    }

    #region DESTINATION / NAVIGATION

    /// <summary>
    /// Called by CarSpawner to give this car its initial destination.
    /// </summary>
    public void SetBuildingDestination(Transform building)
    {
        targetBuilding = building;

        if (agent != null && targetBuilding != null)
        {
            agent.isStopped = false;
            agent.SetDestination(targetBuilding.position);
        }
    }

    /// <summary>
    /// After reaching a building, pick a new random target from BuildingManager.
    /// You can remove this if you prefer one-trip cars.
    /// </summary>
    private void PickRandomBuildingDestination()
    {
        if (BuildingManager.buildingEntrances == null ||
            BuildingManager.buildingEntrances.Count == 0)
        {
            return;
        }

        Transform newTarget = targetBuilding;

        if (BuildingManager.buildingEntrances.Count == 1)
        {
            newTarget = BuildingManager.buildingEntrances[0];
        }
        else
        {
            int safety = 0;
            while ((newTarget == targetBuilding || newTarget == null) && safety < 10)
            {
                int index = Random.Range(0, BuildingManager.buildingEntrances.Count);
                newTarget = BuildingManager.buildingEntrances[index];
                safety++;
            }
        }

        SetBuildingDestination(newTarget);
    }

    #endregion

    #region ARRIVAL / DESTROY

    /// <summary>
    /// If the car is near its target and basically not moving, despawn it.
    /// </summary>
    private void TryDestroyWhenArrived()
    {
        if (agent == null || targetBuilding == null)
            return;

        if (agent.pathPending)
            return;

        // Close enough to the target?
        if (agent.remainingDistance <= destroyDistance)
        {
            // "Stopped" in a practical sense: very low velocity
            if (agent.velocity.sqrMagnitude < 0.01f)
            {
                // Notify spawner and destroy this car
                if (spawner != null)
                {
                    spawner.NotifyCarDestroyed();
                }

                Destroy(gameObject);
            }
        }
    }

    #endregion

    #region TRAFFIC HANDLING

    private void HandleTraffic()
    {
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        RaycastHit hit;

        bool shouldStop = false;
        stoppedByCar = false;
        stoppedByLight = false;

        bool hitSomething = false;

        if (useSphereCast)
        {
            // Fatter, more forgiving detection for small cars
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
            // Thin ray
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

            // 1) Check for a traffic light probe on the collider
            TrafficSignalProbe probe = hit.collider.GetComponent<TrafficSignalProbe>();
            if (probe != null)
            {
                LightColor color = probe.CurrentColor;

                // Stop if light is not green and we’re close enough
                if (color != LightColor.Green && distance < stopDistance)
                {
                    shouldStop = true;
                    stoppedByLight = true;
                }
            }

            // 2) If the light doesn't block us, check for a car
            if (!shouldStop && hit.collider.CompareTag("Car") && distance < stopDistance)
            {
                shouldStop = true;
                stoppedByCar = true;
            }
        }

        // Apply stopping / resuming
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

    #region ROTATION

    /// <summary>
    /// Rotate the car to face its current velocity direction.
    /// </summary>
    private void UpdateRotation()
    {
        if (agent == null)
            return;

        Vector3 velocity = agent.velocity;

        // Ignore tiny jitter
        if (velocity.sqrMagnitude < 0.01f)
            return;

        // Look in direction of movement (XZ plane)
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

    #endregion

    #region DEBUG

    private void DrawDebugRay()
    {
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        Vector3 dir = transform.forward * detectionDistance;

        // Color based on what’s stopping us
        Color color = Color.green;
        if (stoppedByLight)
            color = Color.red;
        else if (stoppedByCar)
            color = Color.yellow;

        Debug.DrawRay(origin, dir, color);

        // Optional: draw spherecast radius near the front
        if (useSphereCast)
        {
            Vector3 front = origin + transform.forward * Mathf.Min(detectionDistance, stopDistance);
            Debug.DrawLine(origin, front, color);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Nice visual in Scene view when car is selected
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position + Vector3.up * rayHeight;
        Gizmos.DrawLine(origin, origin + transform.forward * detectionDistance);

        if (useSphereCast)
        {
            Vector3 front = origin + transform.forward * Mathf.Min(detectionDistance, stopDistance);
            Gizmos.DrawWireSphere(front, sphereCastRadius);
        }
    }

    #endregion
}