using UnityEngine;
using System.Collections;

namespace Thinklib.Platformer.Enemy.Core
{
    public class ProjectileShooterBase : MonoBehaviour
    {
        [Header("Configura��es de Proj�til")]
        public GameObject projectilePrefab;
        public Transform launchPosition;
        public float projectileSpeed = 10f;
        public float maxProjectileLifetime = 5f;

        [Header("Refer�ncia ao Animator (opcional)")]
        public Animator animator;

        /// <summary>
        /// Dispara um proj�til em dire��o horizontal (1 = direita, -1 = esquerda)
        /// </summary>
        public void ShootProjectile(int direction)
        {
            ShootProjectile(new Vector2(direction, 0));
        }

        /// <summary>
        /// Dispara um proj�til na dire��o definida (normalizada ou n�o).
        /// </summary>
        public void ShootProjectile(Vector2 direction)
        {
            if (projectilePrefab == null || launchPosition == null)
            {
                Debug.LogWarning("ProjectileShooterBase: Proj�til ou ponto de lan�amento n�o atribu�dos.");
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

            // Ajusta a escala para corresponder � dire��o horizontal
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
