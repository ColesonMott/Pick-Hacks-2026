using UnityEngine;
using UnityEngine.AI;

public class IntersectionLaneEnforcer : MonoBehaviour
{
    public Transform directionReference;   // correct lane direction
    public Transform correctionPoint;      // snap location

    private float lastCorrectionTime = -10f;

    void OnTriggerEnter(Collider other)
    {
        CarAI car = other.GetComponent<CarAI>();
        if (car == null)
            return;

        NavMeshAgent agent = car.GetComponent<NavMeshAgent>();
        if (agent == null || !agent.isOnNavMesh)
            return;

        Vector3 allowed = directionReference.forward.normalized;
        Vector3 carForward = car.transform.forward.normalized;

        // 🔥 ONLY teleport if VERY clearly opposite direction
        float directionDot = Vector3.Dot(carForward, allowed);

        if (directionDot > -0.6f) // must be mostly opposite
            return;

        if (Time.time - lastCorrectionTime < 0.5f)
            return;

        lastCorrectionTime = Time.time;

        // Stop agent safely
        agent.isStopped = true;

        // Completely clear old path
        agent.ResetPath();

        // Warp to correction point
        agent.Warp(correctionPoint.position);

        // Force correct orientation
        car.transform.rotation = Quaternion.LookRotation(allowed, Vector3.up);

        // Push forward slightly so it leaves trigger
        Vector3 forwardPush = correctionPoint.position + allowed * 4f;

        if (NavMesh.SamplePosition(forwardPush, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        agent.isStopped = false;
    }
}