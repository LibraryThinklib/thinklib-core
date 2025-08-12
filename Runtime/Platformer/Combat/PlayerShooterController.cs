using UnityEngine;
using UnityEngine.UI;
using Thinklib.Platformer.Enemy.Core;

[AddComponentMenu("Thinklib/Platformer/Combat/Player Shooter Controller", -98)]

[RequireComponent(typeof(ProjectileShooterBase))]
public class PlayerShooterController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode shootKey = KeyCode.E;
    public Button shootButton;

    [Header("Shooting Settings")]
    public float timeBetweenShots = 1.5f;
    private float shotCooldownTimer = 0f;

    [Header("Damage Settings")]
    public int projectileDamage = 1;

    [Header("Debug Direction (Read Only)")]
    public bool isFacingRight = true;
    public bool isFacingLeft = false;

    private int direction = 1;
    private ProjectileShooterBase shooter;
    private Animator animator;

    private void Awake()
    {
        shooter = GetComponent<ProjectileShooterBase>();
        animator = GetComponent<Animator>();
        ConfigureShootButton();
    }

    private void Update()
    {
        shotCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(shootKey) && shotCooldownTimer <= 0f)
        {
            Shoot();
        }

        UpdateDirectionFromScale();
    }

    private void ConfigureShootButton()
    {
        if (shootButton != null)
        {
            shootButton.onClick.RemoveAllListeners();
            shootButton.onClick.AddListener(() =>
            {
                if (shotCooldownTimer <= 0f)
                    Shoot();
            });
        }
    }

    private void Shoot()
    {
        if (animator != null)
            animator.SetBool("IsShooting", true);

        GameObject proj = shooter.ShootProjectile(new Vector2(direction, 0));

        if (proj != null)
        {
            var damageDealer = proj.GetComponent<ProjectileDamageDealer>();
            if (damageDealer != null)
            {
                damageDealer.damage = projectileDamage;
            }
        }

        shotCooldownTimer = timeBetweenShots;
    }


    private void UpdateDirectionFromScale()
    {
        direction = transform.localScale.x >= 0 ? 1 : -1;
        isFacingRight = direction == 1;
        isFacingLeft = direction == -1;
    }
}
