using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class LaneFollower : MonoBehaviour
{
    private NavMeshAgent agent;

    public float steeringSmoothness = 6f;

    private List<Vector3> pathPoints = new List<Vector3>();
    private int currentIndex = 0;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    public void SetPath(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(destination, path);

        pathPoints.Clear();
        pathPoints.AddRange(path.corners);

        currentIndex = 0;
    }

    void Update()
    {
        if (pathPoints.Count == 0)
            return;

        Vector3 target = pathPoints[currentIndex];
        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.magnitude < 2f)
        {
            if (currentIndex < pathPoints.Count - 1)
                currentIndex++;
        }

        direction.Normalize();

        // Smooth steering
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * steeringSmoothness
            );
        }

        agent.Move(transform.forward * agent.speed * Time.deltaTime);
    }
}