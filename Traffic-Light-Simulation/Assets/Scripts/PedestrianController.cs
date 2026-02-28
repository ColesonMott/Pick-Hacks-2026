using UnityEngine;

public class PedestrianController : MonoBehaviour
{
    [Header("Movement")]
    public Transform targetPoint;     // Where the pedestrian walks to
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 5f;
    public float stopDistance = 0.1f;

    [Header("Animation")]
    public string speedParameter = "Speed";

    private Animator animator;
    private bool canWalk = true;
    private bool hasReachedDestination = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (targetPoint == null)
        {
            Debug.LogError("No targetPoint assigned on " + gameObject.name);
        }
    }

    void Update()
    {
        if (targetPoint == null || hasReachedDestination)
        {
            animator.SetFloat(speedParameter, 0f);
            return;
        }

        if (!canWalk)
        {
            animator.SetFloat(speedParameter, 0f);
            return;
        }

        MoveTowardsTarget();
    }

    void MoveTowardsTarget()
    {
        Vector3 direction = (targetPoint.position - transform.position);
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance <= stopDistance)
        {
            hasReachedDestination = true;
            animator.SetFloat(speedParameter, 0f);
            return;
        }

        direction.Normalize();

        // Rotate smoothly toward direction
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            rotationSpeed * Time.deltaTime
        );

        // Move forward
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Tell animator we are walking
        animator.SetFloat(speedParameter, moveSpeed);
    }

    // 🚦 Called by traffic light system
    public void StopWalking()
    {
        canWalk = false;
        animator.SetFloat(speedParameter, 0f);
    }

    // 🚦 Called when WALK signal turns on
    public void ResumeWalking()
    {
        if (!hasReachedDestination)
            canWalk = true;
    }
}