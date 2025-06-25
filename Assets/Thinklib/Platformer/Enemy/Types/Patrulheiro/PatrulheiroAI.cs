using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PatrollerAI : MonoBehaviour
{
    [Header("Patrol Points (placed outside the enemy!)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float tolerance = 0.5f;

    private Animator animator;
    private Transform currentTarget;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentTarget = pointB;
        Flip();
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        animator.SetBool("IsWalking", true);

        // Move toward the current target
        transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

        // Reached destination?
        if (Vector2.Distance(transform.position, currentTarget.position) <= tolerance)
        {
            SwitchTarget();
        }
    }

    private void SwitchTarget()
    {
        // Toggle between point A and B
        currentTarget = currentTarget == pointA ? pointB : pointA;
        Flip();
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        float direction = currentTarget.position.x - transform.position.x;

        if (direction > 0f)
            scale.x = Mathf.Abs(scale.x); // facing right
        else if (direction < 0f)
            scale.x = -Mathf.Abs(scale.x); // facing left

        transform.localScale = scale;
    }
}
