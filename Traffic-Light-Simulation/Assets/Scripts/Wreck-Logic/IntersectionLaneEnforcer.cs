using UnityEngine;
using UnityEngine.AI;

public class IntersectionLaneEnforcer : MonoBehaviour
{
    [Tooltip("Transform whose forward is the correct lane direction")]
    public Transform directionReference;   // correct lane direction

    [Tooltip("Where to snap the car back onto the lane")]
    public Transform correctionPoint;      // snap location

    private float lastCorrectionTime = -10f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Enforcer] {name}: Triggered by {other.name}");

        // Try to find CarAI on this object OR its parents
        CarAI car = other.GetComponentInParent<CarAI>();
        if (car == null)
        {
            Debug.Log($"[Enforcer] {name}: Triggered by {other.name}, but no CarAI found in parents.");
            return;
        }

        NavMeshAgent agent = car.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogWarning($"[Enforcer] {name}: Car {car.name} has no NavMeshAgent.");
            return;
        }

        if (directionReference == null || correctionPoint == null)
        {
            Debug.LogWarning($"[Enforcer] {name}: directionReference or correctionPoint not assigned.");
            return;
        }

        Vector3 allowed = directionReference.forward.normalized;
        Vector3 carForward = car.transform.forward.normalized;

        float dot = Vector3.Dot(carForward, allowed);

        // Debug info
        // Debug.Log($"[Enforcer] {name}: Car {car.name} entered. Dot = {dot}");

        // ONLY correct if clearly opposite direction
        if (dot < -0.3f && Time.time - lastCorrectionTime > 0.5f)
        {
            lastCorrectionTime = Time.time;

            if (!agent.isOnNavMesh)
            {
                Debug.LogWarning($"[Enforcer] {name}: Car {car.name} agent is not on NavMesh, cannot warp.");
                return;
            }

            // Stop the agent safely
            agent.isStopped = true;

            // First warp to correction point
            agent.Warp(correctionPoint.position);

            // Rotate to correct direction
            car.transform.rotation = Quaternion.LookRotation(allowed, Vector3.up);

            // Small forward push so it exits the trigger
            Vector3 pushPos = correctionPoint.position + allowed * 1.5f;
            agent.Warp(pushPos);

            // Resume movement
            agent.isStopped = false;

            Debug.Log($"[Enforcer] {name}: Corrected {car.name} to lane direction.");
        }
    }
}