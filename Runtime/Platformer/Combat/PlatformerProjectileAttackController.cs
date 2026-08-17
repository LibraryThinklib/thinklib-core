// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Platformer/Combat/Projectile Attack Controller", -100)]
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

    // Telemetry
    private const string MechanicName = "Platformer/Combat/ProjectileAttack";
    private bool _sentUsed = false;
    private string _lastInput = "unknown";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ConfigureShootButton();

        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(PlatformerProjectileAttackController),
            new Dictionary<string, object> {
                { "projectileSpeed", projectileSpeed },
                { "maxProjectileLifetime", maxProjectileLifetime },
                { "hasButton", shootButton != null }
            }
        );
    }

    private void Update()
    {
        if (Input.GetKeyDown(shootKey))
        {
            _lastInput = "keyboard";
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
            shootButton.onClick.AddListener(() =>
            {
                _lastInput = "button";
                ShootProjectile();
            });
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
        isFacingLeft  = direction == -1;
    }

    public void ShootProjectile()
    {
        try
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

            // mechanic_used (first shot)
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlatformerProjectileAttackController),
                    new Dictionary<string, object> {
                        { "input", _lastInput },
                        { "direction", direction },
                        { "projectileSpeed", projectileSpeed }
                    }
                );
            }

            Destroy(projectile, maxProjectileLifetime);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlatformerProjectileAttackController),
                new Dictionary<string, object> {
                    { "where", "ShootProjectile" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private System.Collections.IEnumerator ResetShootingState()
    {
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("IsShooting", false);
        Debug.Log("Parameter 'IsShooting' set to false.");
    }
}
