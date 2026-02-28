using UnityEngine;
using UnityEngine.AI;

public class AmbulanceAI : MonoBehaviour
{
    private NavMeshAgent agent;

    public float activationDistance = 30f;
    private TrafficIntersection activeIntersection;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed *= 1.5f;
    }

    void Update()
    {
        if (agent.hasPath)
        {
            CheckUpcomingIntersections();
        }
    }

    void CheckUpcomingIntersections()
    {
        Vector3[] corners = agent.path.corners;

        foreach (Vector3 corner in corners)
        {
            foreach (TrafficIntersection intersection in TrafficIntersection.AllIntersections)
            {
                float distance = Vector3.Distance(corner, intersection.transform.position);

                if (distance < activationDistance)
                {
                    if (activeIntersection != intersection)
                    {
                        if (activeIntersection != null)
                            activeIntersection.ResumeNormal();

                        activeIntersection = intersection;

                        string direction = GetTravelDirection();
                        activeIntersection.ActivateEmergency(direction);
                    }

                    return;
                }
            }
        }
    }

    string GetTravelDirection()
    {
        Vector3 velocity = agent.velocity;

        if (Mathf.Abs(velocity.z) > Mathf.Abs(velocity.x))
            return velocity.z > 0 ? "North" : "South";
        else
            return velocity.x > 0 ? "East" : "West";
    }
}