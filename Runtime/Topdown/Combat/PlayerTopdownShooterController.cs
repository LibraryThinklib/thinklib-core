// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thinklib.Platformer.Enemy.Core;
using Thinklib.Topdown.Player.Core;
using Thinklib.Telemetry;

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

    // Telemetry
    private const string MechanicName = "Topdown/Combat/PlayerTopdownShooter";
    private bool _sentUsedShoot = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        shooter  = GetComponent<ProjectileTopdownShooterBase>();

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

        // mechanic_instantiated
        Thinklib.Telemetry.ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(PlayerTopdownShooterController),
            new Dictionary<string, object> {
                { "hasShootButton", shootButton != null },
                { "timeBetweenShots", timeBetweenShots },
                { "projectileDamage", projectileDamage },
                { "hasMovementController", movementController != null }
            }
        );
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
        try
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

            // mechanic_used: primeiro disparo bem-sucedido
            if (!_sentUsedShoot)
            {
                _sentUsedShoot = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlayerTopdownShooterController),
                    new Dictionary<string, object> {
                        { "action", "shoot" },
                        { "dirX", direction.x },
                        { "dirY", direction.y },
                        { "cooldown", timeBetweenShots },
                        { "damage", projectileDamage },
                        { "projectileSpawned", proj != null }
                    }
                );
            }

            shotCooldownTimer = timeBetweenShots;
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerTopdownShooterController),
                new Dictionary<string, object> {
                    { "where", "Shoot" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    // Corrotina: não usar try/catch (apenas try/finally se necessário).
    private IEnumerator ResetIsShootingAfterAnimation()
    {
        yield return null; // garante que a transição de estado ocorra
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float duration = state.length > 0 ? state.length : 0.5f;
        yield return new WaitForSeconds(duration);
        if (animator != null) animator.SetBool("IsShooting", false);
    }
}
