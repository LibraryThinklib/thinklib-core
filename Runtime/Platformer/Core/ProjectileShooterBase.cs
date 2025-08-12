using UnityEngine;
using System.Collections;

[AddComponentMenu("Thinklib/Core/Projectile Shooter Base", -100)]
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
        public GameObject ShootProjectile(int direction)
        {
            return ShootProjectile(new Vector2(direction, 0));
        }

        /// <summary>
        /// Shoots a projectile in the given direction and returns the instance.
        /// </summary>
        public GameObject ShootProjectile(Vector2 direction)
        {
            if (projectilePrefab == null || launchPosition == null)
            {
                Debug.LogWarning("ProjectileShooterBase: Projectile prefab or launch position not assigned.");
                return null;
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

            // Ajusta a escala do projétil de acordo com a direção
            Vector3 scale = proj.transform.localScale;
            if (Mathf.Abs(direction.x) > 0.01f)
                scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction.x);
            proj.transform.localScale = scale;

            Destroy(proj, maxProjectileLifetime);
            StartCoroutine(ResetShootAnimation());

            return proj;
        }

        private IEnumerator ResetShootAnimation()
        {
            if (animator != null)
            {
                AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
                float duration = state.length;
                yield return new WaitForSeconds(Mathf.Max(0.1f, duration));
                animator.SetBool("IsShooting", false);
            }
        }
    }
}
