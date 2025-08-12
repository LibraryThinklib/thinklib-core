using UnityEngine;
using UnityEngine.UI;
using Thinklib.Platformer.Enemy.Core;
using Thinklib.Topdown.Player.Core;
using System.Collections;

[AddComponentMenu("Thinklib/Topdown/Combat/Player Shooter Controller", -99)]
[RequireComponent(typeof(ProjectileTopdownShooterBase))]
public class PlayerTopdownShooterController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode shootKey = KeyCode.Space;
    public Button shootButton;

    [Header("Cooldown")]
    public float timeBetweenShots = 1.0f;
    private float shotCooldownTimer = 0f;

    [Header("Damage Settings")]
    public int projectileDamage = 1;

    [Header("References")]
    public TopdownMovementController movementController;

    private Animator animator;
    private ProjectileTopdownShooterBase shooter;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        shooter = GetComponent<ProjectileTopdownShooterBase>();

        if (shootButton != null)
        {
            shootButton.onClick.RemoveAllListeners();
            shootButton.onClick.AddListener(() =>
            {
                if (shotCooldownTimer <= 0f)
                    Shoot();
            });
        }

        if (movementController == null)
            movementController = GetComponent<TopdownMovementController>();
    }

    private void Update()
    {
        shotCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(shootKey) && shotCooldownTimer <= 0f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Vector2 direction = movementController != null
            ? movementController.GetLastMoveDirection()
            : Vector2.down;

        if (direction == Vector2.zero)
            direction = Vector2.down;

        if (animator != null)
        {
            animator.SetBool("IsShooting", true);
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
            StartCoroutine(ResetIsShootingAfterAnimation());
        }

        GameObject proj = shooter.ShootProjectile(direction);

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

    private IEnumerator ResetIsShootingAfterAnimation()
    {
        yield return null; // garante que a transi��o de estado ocorra
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float duration = state.length > 0 ? state.length : 0.5f;
        yield return new WaitForSeconds(duration);
        animator.SetBool("IsShooting", false);
    }
}
