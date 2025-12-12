using System;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Platformer.Enemy.Core;
using Thinklib.Telemetry;

namespace Thinklib.Platformer.Enemy.Types
{
    [AddComponentMenu("Thinklib/Platformer/Enemy/Shooter/Enemy Shooter AI", -90)]
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

        // Telemetry
        private const string MechanicName = "Platformer/Enemy/Shooter";
        private bool _sentUsed = false;

        private void Awake()
        {
            shooter = GetComponent<ProjectileShooterBase>();
            animator = GetComponent<Animator>();
            currentTarget = pointB;

            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(EnemyShooterAI),
                new Dictionary<string, object> {
                    { "shootingRadius", shootingRadius },
                    { "timeBetweenShots", timeBetweenShots },
                    { "aimAtTarget", aimAtTarget },
                    { "projectileDamage", projectileDamage },
                    { "isStatic", isStatic },
                    { "isPatroller", isPatroller },
                    { "hasPointA", pointA != null },
                    { "hasPointB", pointB != null },
                    { "patrolSpeed", patrolSpeed },
                    { "patrolTolerance", patrolTolerance }
                }
            );
        }

        private void Update()
        {
            if (player == null) return;

            try
            {
                float distance = Vector2.Distance(transform.position, player.position);

                if (distance <= shootingRadius)
                {
                    if (currentCooldown <= 0f)
                    {
                        Vector2 direction = aimAtTarget
                            ? (player.position - shooter.launchPosition.position).normalized
                            : new Vector2(player.position.x > transform.position.x ? 1 : -1, 0);

                        GameObject proj = shooter.ShootProjectile(direction);
                        if (proj != null)
                        {
                            var damageDealer = proj.GetComponent<ProjectileDamageDealer>();
                            if (damageDealer != null)
                                damageDealer.damage = projectileDamage;

                            if (!_sentUsed)
                            {
                                _sentUsed = true;
                                ThinklibTelemetry.Track(
                                    "mechanic_used",
                                    MechanicName,
                                    nameof(EnemyShooterAI),
                                    new Dictionary<string, object> {
                                        { "trigger", "first_shot" },
                                        { "aimAtTarget", aimAtTarget }
                                    }
                                );
                            }
                        }

                        currentCooldown = timeBetweenShots;
                    }

                    if (animator != null) animator.SetBool("IsWalking", false);
                }
                else if (isPatroller)
                {
                    Patrol();
                }

                currentCooldown -= Time.deltaTime;
            }
            catch (Exception ex)
            {
                ThinklibTelemetry.Track(
                    "mechanic_error",
                    MechanicName,
                    nameof(EnemyShooterAI),
                    new Dictionary<string, object> {
                        { "where", "Update" },
                        { "message", ex.Message },
                        { "stack", ex.StackTrace }
                    }
                );
                throw;
            }
        }

        private void Patrol()
        {
            if (pointA == null || pointB == null) return;

            try
            {
                if (animator != null) animator.SetBool("IsWalking", true);
                transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, patrolSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, currentTarget.position) <= patrolTolerance)
                {
                    currentTarget = (currentTarget == pointA) ? pointB : pointA;
                    Flip();
                }
            }
            catch (Exception ex)
            {
                ThinklibTelemetry.Track(
                    "mechanic_error",
                    MechanicName,
                    nameof(EnemyShooterAI),
                    new Dictionary<string, object> {
                        { "where", "Patrol" },
                        { "message", ex.Message },
                        { "stack", ex.StackTrace }
                    }
                );
                throw;
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
