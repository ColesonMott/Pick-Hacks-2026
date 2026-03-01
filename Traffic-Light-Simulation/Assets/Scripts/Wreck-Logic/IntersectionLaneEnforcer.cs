using UnityEngine;
using UnityEngine.AI;

public class IntersectionLaneEnforcer : MonoBehaviour
{
    public Transform directionReference;  // assign lane object
    public Transform laneCenter;
    public Transform roadCenter;

    void OnTriggerEnter(Collider other)
    {
        CarAI car = other.GetComponent<CarAI>();
        if (car == null)
            return;

        Vector3 allowed = directionReference.forward.normalized;
        Vector3 carForward = car.transform.forward.normalized;

        float directionDot = Vector3.Dot(carForward, allowed);

        Vector3 toCar = car.transform.position - roadCenter.position;
        Vector3 roadRight = Vector3.Cross(Vector3.up, allowed).normalized;

        float sideDot = Vector3.Dot(toCar, roadRight);

        if (directionDot < 0.3f && sideDot < 0f)
        {
            NavMeshAgent agent = car.GetComponent<NavMeshAgent>();

            agent.enabled = false;

            car.transform.position = laneCenter.position;
            car.transform.rotation = Quaternion.LookRotation(allowed, Vector3.up);

            agent.enabled = true;
        }
    }
}