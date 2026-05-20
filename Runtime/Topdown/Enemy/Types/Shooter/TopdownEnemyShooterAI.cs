// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;
using Thinklib.Topdown.Player.Core;

namespace Thinklib.Topdown.Enemy
{
    [AddComponentMenu("Thinklib/Topdown/Enemy/Shooter/Enemy Shooter AI", -90)]
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

        // Telemetry
        private const string MechanicName = "Topdown/Enemy/Shooter/EnemyShooterAI";
        private bool _sentUsedShoot   = false;
        private bool _sentUsedPatrol  = false;

        private void Awake()
        {
            shooter = GetComponent<ProjectileTopdownShooterBase>();
            animator = GetComponent<Animator>();
            currentTarget = pointB;

            // mechanic_instantiated
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(TopdownEnemyShooterAI),
                new Dictionary<string, object> {
                    { "shootingRadius", shootingRadius },
                    { "timeBetweenShots", timeBetweenShots },
                    { "aimAtTarget", aimAtTarget },
                    { "projectileDamage", projectileDamage },
                    { "isStatic", isStatic },
                    { "isPatroller", isPatroller },
                    { "hasPointA", pointA != null },
                    { "hasPointB", pointB != null }
                }
            );
        }

        private void Update()
        {
            try
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
            catch (Exception ex)
            {
                ThinklibTelemetry.Track(
                    "mechanic_error",
                    MechanicName,
                    nameof(TopdownEnemyShooterAI),
                    new Dictionary<string, object> {
                        { "where", "Update" },
                        { "message", ex.Message },
                        { "stack", ex.StackTrace }
                    }
                );
                throw;
            }
        }

        private void TryShoot()
        {
            try
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

                // mechanic_used: primeiro disparo com sucesso
                if (!_sentUsedShoot)
                {
                    _sentUsedShoot = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(TopdownEnemyShooterAI),
                        new Dictionary<string, object> {
                            { "action", "shoot" },
                            { "dirX", direction.x },
                            { "dirY", direction.y },
                            { "projectileSpawned", proj != null }
                        }
                    );
                }

                currentCooldown = timeBetweenShots;
            }
            catch (Exception ex)
            {
                ThinklibTelemetry.Track(
                    "mechanic_error",
                    MechanicName,
                    nameof(TopdownEnemyShooterAI),
                    new Dictionary<string, object> {
                        { "where", "TryShoot" },
                        { "message", ex.Message },
                        { "stack", ex.StackTrace }
                    }
                );
                throw;
            }
        }

        private void Patrol()
        {
            try
            {
                if (pointA == null || pointB == null) return;

                Vector2 targetPos  = currentTarget.position;
                Vector2 currentPos = transform.position;
                Vector2 moveDir    = (targetPos - currentPos).normalized;

                animator.SetBool("IsMoving", true);
                SetAnimatorDirection(moveDir);

                transform.position = Vector2.MoveTowards(currentPos, targetPos, patrolSpeed * Time.deltaTime);

                // mechanic_used: primeira vez que inicia patrulha/movimento
                if (!_sentUsedPatrol && moveDir.sqrMagnitude > 0.0001f)
                {
                    _sentUsedPatrol = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(TopdownEnemyShooterAI),
                        new Dictionary<string, object> {
                            { "action", "start_patrol" },
                            { "speed", patrolSpeed }
                        }
                    );
                }

                if (Vector2.Distance(currentPos, targetPos) <= patrolTolerance)
                {
                    currentTarget = (currentTarget == pointA) ? pointB : pointA;
                }
            }
            catch (Exception ex)
            {
                ThinklibTelemetry.Track(
                    "mechanic_error",
                    MechanicName,
                    nameof(TopdownEnemyShooterAI),
                    new Dictionary<string, object> {
                        { "where", "Patrol" },
                        { "message", ex.Message },
                        { "stack", ex.StackTrace }
                    }
                );
                throw;
            }
        }

        private void SetAnimatorDirection(Vector2 direction)
        {
            if (direction.sqrMagnitude > 0.01f)
            {
                animator.SetFloat("Horizontal", direction.x);
                animator.SetFloat("Vertical",   direction.y);
                lastDirection = direction;
            }
            else
            {
                animator.SetFloat("Horizontal", lastDirection.x);
                animator.SetFloat("Vertical",   lastDirection.y);
            }
        }

        // Corrotina sem try/catch (safe)
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
