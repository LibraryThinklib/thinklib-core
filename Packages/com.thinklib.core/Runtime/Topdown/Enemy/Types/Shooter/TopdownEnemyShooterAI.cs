using UnityEngine;
using Thinklib.Topdown.Player.Core;

namespace Thinklib.Topdown.Enemy
{
    [RequireComponent(typeof(ProjectileTopdownShooterBase))]
    public class TopdownEnemyShooterAI : MonoBehaviour
    {
        [Header("References")]
        public Transform player;

        [Header("Shooting Settings")]
        public float shootingRadius = 5f;
        public float timeBetweenShots = 1.5f;

        [Header("Shooting Mode")]
        public bool aimAtTarget = true;

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

        private ProjectileTopdownShooterBase shooter;
        private Animator animator;
        private Transform currentTarget;
        private float currentCooldown = 0f;
        private Vector2 lastDirection = Vector2.down;

        private void Awake()
        {
            shooter = GetComponent<ProjectileTopdownShooterBase>();
            animator = GetComponent<Animator>();
            currentTarget = pointB;
        }

        private void Update()
        {
            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= shootingRadius)
            {
                TryShoot();
                SetAnimatorDirection((player.position - transform.position).normalized);
                animator.SetBool("IsMoving", false);
            }
            else if (isPatroller)
            {
                Patrol();
            }

            currentCooldown -= Time.deltaTime;
        }

        private void TryShoot()
        {
            if (currentCooldown > 0f) return;

            Vector2 direction = aimAtTarget
                ? (player.position - shooter.launchPosition.position).normalized
                : (player.position.x > transform.position.x ? Vector2.right : Vector2.left);

            animator.SetBool("IsShooting", true);
            SetAnimatorDirection(direction);
            StartCoroutine(ResetShootAnimation());

            GameObject proj = shooter.ShootProjectile(direction);
            if (proj != null)
            {
                var damageDealer = proj.GetComponent<ProjectileDamageDealer>();
                if (damageDealer != null)
                    damageDealer.damage = projectileDamage;
            }

            currentCooldown = timeBetweenShots;
        }

        private void Patrol()
        {
            if (pointA == null || pointB == null) return;

            Vector2 targetPos = currentTarget.position;
            Vector2 currentPos = transform.position;
            Vector2 moveDir = (targetPos - currentPos).normalized;

            animator.SetBool("IsMoving", true);
            SetAnimatorDirection(moveDir);

            transform.position = Vector2.MoveTowards(currentPos, targetPos, patrolSpeed * Time.deltaTime);

            if (Vector2.Distance(currentPos, targetPos) <= patrolTolerance)
            {
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
            }
        }

        private void SetAnimatorDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                animator.SetFloat("Horizontal", direction.x);
                animator.SetFloat("Vertical", direction.y);
                lastDirection = direction;
            }
            else
            {
                animator.SetFloat("Horizontal", lastDirection.x);
                animator.SetFloat("Vertical", lastDirection.y);
            }
        }

        private System.Collections.IEnumerator ResetShootAnimation()
        {
            yield return null;
            AnimatorStateInfo shootState = animator.GetCurrentAnimatorStateInfo(0);
            float duration = shootState.length > 0f ? shootState.length : 0.5f;
            yield return new WaitForSeconds(duration);
            animator.SetBool("IsShooting", false);
        }
    }
}
