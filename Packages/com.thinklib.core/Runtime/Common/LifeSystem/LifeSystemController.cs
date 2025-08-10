using UnityEngine;
using UnityEngine.Events;

public class LifeSystemController : MonoBehaviour
{
    [Header("General Settings")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Events")]
    public UnityEvent onDeath;

    [Header("UI Reference")]
    public LifeUIBar healthBar;
    public LifeUIIcons healthIcons;

    [Header("Visual Feedback")]
    [SerializeField] private bool enableShake = false;

    [Tooltip("Shake intensity when losing health")]
    [SerializeField, Range(0f, 10f)] private float shakeIntensity = 0.05f;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();

        if (enableShake)
        {
            if (healthBar != null)
                healthBar.Shake(shakeIntensity);

            if (healthIcons != null)
                healthIcons.Shake(shakeIntensity);
        }

        Animator animator = GetComponent<Animator>();

        if (currentHealth > 0)
        {
            // Ativa a animação de dano (Hurt) se ainda estiver vivo
            if (animator != null)
                animator.SetTrigger("IsHurt");
        }
        else
        {
            // Ativa a animação de morte (Dead)
            if (animator != null)
                animator.SetBool("IsDead", true);

            // Executa o efeito de morte
            DeathEffect deathEffect = GetComponent<DeathEffect>();
            if (deathEffect != null)
            {
                deathEffect.PlayDeathEffect();
            }
            else
            {
                Destroy(gameObject);
            }

            onDeath?.Invoke();
        }
    }



    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthBar != null)
            healthBar.UpdateBar(currentHealth, maxHealth);

        if (healthIcons != null)
            healthIcons.UpdateIcons(currentHealth, maxHealth);
    }
}
