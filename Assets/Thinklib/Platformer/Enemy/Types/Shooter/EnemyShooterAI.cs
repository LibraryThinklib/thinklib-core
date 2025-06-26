using UnityEngine;
using Thinklib.Platformer.Enemy.Core;

namespace Thinklib.Platformer.Enemy.Types
{
    [RequireComponent(typeof(ProjectileShooterBase))]
    public class EnemyShooterAI : MonoBehaviour
    {
        [Header("References")]
        public Transform player;

        [Header("Shooting Settings")]
        public float shootingRadius = 5f;
        public float timeBetweenShots = 1.5f;

        [Header("Shooting Mode")]
        public bool aimAtTarget = false;

        [Header("Damage Settings")]
        public int projectileDamage = 1;

        [Header("Behavior Mode")]
        public bool isStatic = true;
        public bool isPatroller = false;

        [Header("Patrol Points (if patroller)")]
        public Transform pointA;
        public Transform pointB;
        public float patrolSpeed = 2f;
        public float patrolTolerance = 0.1f;

        private ProjectileShooterBase shooter;
        private Animator animator;
        private Transform currentTarget;
        private float currentCooldown;

        private void Awake()
        {
            shooter = GetComponent<ProjectileShooterBase>();
            animator = GetComponent<Animator>();
            currentTarget = pointB;
        }

        private void Update()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= shootingRadius)
            {
                if (currentCooldown <= 0f)
                {
                    Vector2 direction;

                    if (aimAtTarget)
                        direction = (player.position - shooter.launchPosition.position).normalized;
                    else
                        direction = new Vector2(player.position.x > transform.position.x ? 1 : -1, 0);

                    // Dispara e configura o dano do projétil
                    GameObject proj = shooter.ShootProjectile(direction);
                    if (proj != null)
                    {
                        var damageDealer = proj.GetComponent<ProjectileDamageDealer>();
                        if (damageDealer != null)
                            damageDealer.damage = projectileDamage;
                    }

                    currentCooldown = timeBetweenShots;
                }

                animator.SetBool("IsWalking", false);
            }
            else if (isPatroller)
            {
                Patrol();
            }

            currentCooldown -= Time.deltaTime;
        }

        private void Patrol()
        {
            if (pointA == null || pointB == null) return;

            animator.SetBool("IsWalking", true);
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, patrolSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, currentTarget.position) <= patrolTolerance)
            {
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
                Flip();
            }
        }

        private void Flip()
        {
            Vector3 scale = transform.localScale;
            float direction = currentTarget.position.x - transform.position.x;

            if (direction > 0f)
                scale.x = Mathf.Abs(scale.x);
            else if (direction < 0f)
                scale.x = -Mathf.Abs(scale.x);

            transform.localScale = scale;
        }
    }
}
