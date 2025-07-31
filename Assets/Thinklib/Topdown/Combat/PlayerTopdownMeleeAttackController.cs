using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class PlayerTopdownMeleeAttackController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode attackKey = KeyCode.F;
    public Button attackButton;

    [Header("Attack Settings")]
    public float timeBetweenAttacks = 0.8f;
    public int attackDamage = 1;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [Header("Attack Points por Direção")]
    public Transform attackPointUp;
    public Transform attackPointDown;
    public Transform attackPointLeft;
    public Transform attackPointRight;
    public Transform attackPointUpLeft;
    public Transform attackPointUpRight;
    public Transform attackPointDownLeft;
    public Transform attackPointDownRight;

    [Header("Visual Effects")]
    public GameObject slashEffectPrefab;
    public float slashEffectDuration = 0.3f;

    private float attackCooldown = 0f;
    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down;
    private TopdownMovementController movementController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movementController = GetComponent<TopdownMovementController>();
        ConfigureAttackButton();
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(attackKey) && attackCooldown <= 0f)
        {
            Attack();
        }

        UpdateLastDirection();
    }

    private void ConfigureAttackButton()
    {
        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(() =>
            {
                if (attackCooldown <= 0f)
                    Attack();
            });
        }
    }

    private void UpdateLastDirection()
    {
        if (movementController != null)
        {
            Vector2 moveInput = movementController.GetLastMoveDirection();
            if (moveInput.sqrMagnitude > 0.01f)
                lastMoveDirection = moveInput.normalized;
        }
    }

    private Transform GetAttackPointFromDirection(Vector2 dir)
    {
        // Diagonais primeiro
        if (dir.x > 0.5f && dir.y > 0.5f) return attackPointUpRight;
        if (dir.x < -0.5f && dir.y > 0.5f) return attackPointUpLeft;
        if (dir.x > 0.5f && dir.y < -0.5f) return attackPointDownRight;
        if (dir.x < -0.5f && dir.y < -0.5f) return attackPointDownLeft;

        // Cardeais
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? attackPointRight : attackPointLeft;
        else
            return dir.y > 0 ? attackPointUp : attackPointDown;
    }

    private void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("IsAttacking");
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
        }

        Transform attackPoint = GetAttackPointFromDirection(lastMoveDirection);
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            LifeSystemController life = enemy.GetComponent<LifeSystemController>();
            if (life != null)
                life.TakeDamage(attackDamage);
        }

        if (slashEffectPrefab != null)
        {
            GameObject effect = Instantiate(slashEffectPrefab, attackPoint.position, Quaternion.identity);

            float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
            effect.transform.rotation = Quaternion.Euler(0, 0, angle);

            Destroy(effect, slashEffectDuration);
        }

        attackCooldown = timeBetweenAttacks;
    }

    private void OnDrawGizmosSelected()
    {
        Transform[] points = {
            attackPointUp, attackPointDown, attackPointLeft, attackPointRight,
            attackPointUpLeft, attackPointUpRight, attackPointDownLeft, attackPointDownRight
        };

        Gizmos.color = Color.red;
        foreach (var point in points)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, attackRange);
        }
    }
}
