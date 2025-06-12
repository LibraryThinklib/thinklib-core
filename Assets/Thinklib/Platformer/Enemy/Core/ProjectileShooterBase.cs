using UnityEngine;
using System.Collections;

namespace Thinklib.Platformer.Enemy.Core
{
    public class ProjectileShooterBase : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public GameObject projectilePrefab;
        public Transform launchPosition;
        public float projectileSpeed = 10f;
        public float maxProjectileLifetime = 5f;

        [Header("Animator Reference (optional)")]
        public Animator animator;

        /// <summary>
        /// Shoots a projectile in horizontal direction (1 = right, -1 = left)
        /// </summary>
        public void ShootProjectile(int direction)
        {
            ShootProjectile(new Vector2(direction, 0));
        }

        /// <summary>
        /// Shoots a projectile in the given direction (normalized or not).
        /// </summary>
        public void ShootProjectile(Vector2 direction)
        {
            if (projectilePrefab == null || launchPosition == null)
            {
                Debug.LogWarning("ProjectileShooterBase: Projectile prefab or launch position not assigned.");
                return;
            }

            if (animator != null)
                animator.SetBool("IsShooting", true);

            GameObject proj = Instantiate(projectilePrefab, launchPosition.position, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                direction.Normalize();
                rb.velocity = direction * projectileSpeed;
            }

            // Adjust the scale to match the horizontal direction
            Vector3 scale = proj.transform.localScale;
            if (Mathf.Abs(direction.x) > 0.01f)
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
            proj.transform.localScale = scale;

            Destroy(proj, maxProjectileLifetime);
            StartCoroutine(ResetShootAnimation());
        }

        private IEnumerator ResetShootAnimation()
        {
            yield return new WaitForSeconds(0.1f);
            if (animator != null)
                animator.SetBool("IsShooting", false);
        }
    }
}
