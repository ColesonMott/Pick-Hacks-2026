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

        Vector3 allowed = directionReference.forward.normalized;
        Vector3 carForward = car.transform.forward.normalized;

        float dot = Vector3.Dot(carForward, allowed);

        // 🔥 ONLY correct if clearly opposite direction
        if (dot < -0.3f && Time.time - lastCorrectionTime > 0.5f)
        {
            lastCorrectionTime = Time.time;

            NavMeshAgent agent = car.GetComponent<NavMeshAgent>();

            if (agent != null)
                agent.enabled = false;

            car.transform.position = correctionPoint.position;
            car.transform.rotation = Quaternion.LookRotation(allowed, Vector3.up);

            // small forward push so it exits trigger
            car.transform.position += allowed * 1.5f;

            if (agent != null)
                agent.enabled = true;
        }
    }
}