using UnityEngine;

[AddComponentMenu("Thinklib/Topdown/Enemy/Patroller/Patroller AI", -99)]
[RequireComponent(typeof(Animator))]
public class TopdownPatrollerAI : MonoBehaviour
{
    [Header("Patrol Points (placed outside the enemy!)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float tolerance = 0.2f;

    private Animator animator;
    private Transform currentTarget;
    private Vector2 lastDirection = Vector2.down;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentTarget = pointB;
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = currentTarget.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        // Mover inimigo
        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        // Atualizar par�metros do Animator
        bool isMoving = direction.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            lastDirection = direction;
        }
        else
        {
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical", lastDirection.y);
        }

        // Chegou no destino?
        if (Vector2.Distance(currentPosition, targetPosition) <= tolerance)
        {
            SwitchTarget();
        }
    }

    private void SwitchTarget()
    {
        currentTarget = currentTarget == pointA ? pointB : pointA;
    }
}
