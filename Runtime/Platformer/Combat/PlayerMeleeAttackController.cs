using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Thinklib/Platformer/Combat/Player Melee Attack Controller", -99)]
public class PlayerMeleeAttackController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode attackKey = KeyCode.F;
    public Button attackButton;

    [Header("Attack Settings")]
    public float timeBetweenAttacks = 0.8f;
    public int attackDamage = 1;
    public float attackRange = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Visual Effects")]
    public GameObject slashEffectPrefab;
    public float slashEffectDuration = 0.3f;

    private float attackCooldown = 0f;
    private Animator animator;
    private int direction = 1;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ConfigureAttackButton();
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(attackKey) && attackCooldown <= 0f)
        {
            Attack();
        }

        UpdateDirectionFromScale();
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

    private void UpdateDirectionFromScale()
    {
        direction = transform.localScale.x >= 0 ? 1 : -1;
    }

    private void Attack()
    {
        if (animator != null)
            animator.SetTrigger("IsAttacking");

        // Detecta inimigos na �rea de ataque
        Vector2 attackCenter = (Vector2)attackPoint.position + Vector2.right * direction * attackRange * 0.5f;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            LifeSystemController life = enemy.GetComponent<LifeSystemController>();
            if (life != null)
                life.TakeDamage(attackDamage);
        }

        // Instancia o efeito visual do ataque
        if (slashEffectPrefab != null)
        {
            Vector3 effectPosition = attackPoint.position;
            GameObject effect = Instantiate(slashEffectPrefab, effectPosition, Quaternion.identity);

            // Inverte o efeito se estiver olhando para a esquerda
            Vector3 scale = effect.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            effect.transform.localScale = scale;

            Destroy(effect, slashEffectDuration);
        }

        attackCooldown = timeBetweenAttacks;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Vector2 attackCenter = (Vector2)attackPoint.position + Vector2.right * direction * attackRange * 0.5f;
        Gizmos.DrawWireSphere(attackCenter, attackRange);
    }
}
