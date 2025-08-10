using UnityEngine;
using UnityEngine.UI;

public class PlatformerProjectileAttackController : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public Transform launchPosition;
    public float projectileSpeed = 10f;
    public float maxProjectileLifetime = 5f;
    public Button shootButton;

    [Header("Key Bindings and Animation")]
    public KeyCode shootKey = KeyCode.E;
    private Animator animator;

    [Header("Debug Direction (Read Only)")]
    public bool isFacingRight = true;
    public bool isFacingLeft = false;

    private int direction = 1;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ConfigureShootButton();
    }

    private void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            Debug.Log("ShootProjectile method will be called.");
            ShootProjectile();
        }

        UpdateDirectionDebug();
    }

    private void ConfigureShootButton()
    {
        if (shootButton != null)
        {
            shootButton.onClick.RemoveAllListeners();
            shootButton.onClick.AddListener(ShootProjectile);
        }
    }

    public void SetDirection(int newDirection)
    {
        direction = Mathf.Clamp(newDirection, -1, 1);
        Debug.Log($"Projectile direction set to: {(direction == 1 ? "Right" : "Left")} ({direction})");

        UpdateDirectionDebug();
    }

    private void UpdateDirectionDebug()
    {
        isFacingRight = direction == 1;
        isFacingLeft = direction == -1;
    }

    public void ShootProjectile()
    {
        if (animator != null)
        {
            Debug.Log($"Before: IsShooting = {animator.GetBool("IsShooting")}");
            animator.SetBool("IsShooting", true);
            Debug.Log($"After: IsShooting = {animator.GetBool("IsShooting")}");
            StartCoroutine(ResetShootingState());
        }

        if (projectilePrefab == null || launchPosition == null)
        {
            Debug.LogWarning("Projectile prefab or launch position not assigned!");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, launchPosition.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 velocity = new Vector2(projectileSpeed * direction, 0);
            rb.velocity = velocity;
            Debug.Log($"Projectile velocity: {velocity}");
        }

        Vector3 scale = projectile.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        projectile.transform.localScale = scale;

        Destroy(projectile, maxProjectileLifetime);
    }

    private System.Collections.IEnumerator ResetShootingState()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsShooting", false);
        Debug.Log("Parameter 'IsShooting' set to false.");
    }
}
