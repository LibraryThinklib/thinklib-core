using UnityEngine;

namespace Thinklib.Topdown.Player.Core
{
    public class ProjectileTopdownShooterBase : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public GameObject projectilePrefab;
        public Transform launchPosition;
        public float projectileSpeed = 10f;
        public float maxProjectileLifetime = 5f;

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
                Debug.LogWarning("ProjectileTopdownShooterBase: Projectile prefab or launch position not assigned.");
                return null;
            }

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
            return proj;
        }
    }
}
