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

[AddComponentMenu("Thinklib/Topdown/Combat/Player Melee Attack Controller", -100)]
[RequireComponent(typeof(Animator))]
public class PlayerTopdownMeleeAttackController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode attackKey = KeyCode.F;
    public Button attackButton;

    [Header("Attack Settings")]
    public float timeBetweenAttacks = 0.8f;
    public int attackDamage = 1;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    [Header("Attack Points by Direction")]
    public Transform attackPointUp;
    public Transform attackPointDown;
    public Transform attackPointLeft;
    public Transform attackPointRight;
    public Transform attackPointUpLeft;
    public Transform attackPointUpRight;
    public Transform attackPointDownLeft;
    public Transform attackPointDownRight;

    [Header("Visual Effects")]
    public GameObject slashEffectPrefab;
    public float slashEffectDuration = 0.3f;

    private float attackCooldown = 0f;
    private Animator animator;
    private Vector2 lastMoveDirection = Vector2.down;
    private TopdownMovementController movementController;

    // Telemetry
    private const string MechanicName = "Topdown/Combat/MeleeAttack";
    private bool _sentUsed = false;
    private string _lastInput = "unknown";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movementController = GetComponent<TopdownMovementController>();
        ConfigureAttackButton();

        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(PlayerTopdownMeleeAttackController),
            new Dictionary<string, object> {
                { "timeBetweenAttacks", timeBetweenAttacks },
                { "attackDamage", attackDamage },
                { "attackRange", attackRange },
                { "hasButton", attackButton != null },
                { "hasUp", attackPointUp != null },
                { "hasDown", attackPointDown != null },
                { "hasLeft", attackPointLeft != null },
                { "hasRight", attackPointRight != null },
                { "hasUpLeft", attackPointUpLeft != null },
                { "hasUpRight", attackPointUpRight != null },
                { "hasDownLeft", attackPointDownLeft != null },
                { "hasDownRight", attackPointDownRight != null }
            }
        );
    }

    private void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (Input.GetKeyDown(attackKey) && attackCooldown <= 0f)
        {
            _lastInput = "keyboard";
            Attack();
        }

        UpdateLastDirection();
    }

    private void ConfigureAttackButton()
    {
        if (attackButton != null)
        {
            attackButton.onClick.RemoveAllListeners();
            attackButton.onClick.AddListener(() =>
            {
                if (attackCooldown <= 0f)
                {
                    _lastInput = "button";
                    Attack();
                }
            });
        }
    }

    private void UpdateLastDirection()
    {
        if (movementController != null)
        {
            Vector2 moveInput = movementController.GetLastMoveDirection();
            if (moveInput.sqrMagnitude > 0.01f)
                lastMoveDirection = moveInput.normalized;
        }
    }

    private Transform GetAttackPointFromDirection(Vector2 dir)
    {
        // Diagonais
        if (dir.x > 0.5f && dir.y > 0.5f) return attackPointUpRight;
        if (dir.x < -0.5f && dir.y > 0.5f) return attackPointUpLeft;
        if (dir.x > 0.5f && dir.y < -0.5f) return attackPointDownRight;
        if (dir.x < -0.5f && dir.y < -0.5f) return attackPointDownLeft;

        // Cardeais
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? attackPointRight : attackPointLeft;
        else
            return dir.y > 0 ? attackPointUp : attackPointDown;
    }

    private void Attack()
    {
        try
        {
            if (animator != null)
            {
                animator.SetTrigger("IsAttacking");
                animator.SetFloat("Horizontal", lastMoveDirection.x);
                animator.SetFloat("Vertical",  lastMoveDirection.y);
            }

            Transform attackPoint = GetAttackPointFromDirection(lastMoveDirection);
            if (attackPoint == null) return;

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                var life = enemy.GetComponent<LifeSystemController>();
                if (life != null) life.TakeDamage(attackDamage);
            }

            if (slashEffectPrefab != null)
            {
                GameObject effect = Instantiate(slashEffectPrefab, attackPoint.position, Quaternion.identity);
                float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
                effect.transform.rotation = Quaternion.Euler(0, 0, angle);
                Destroy(effect, slashEffectDuration);
            }

            // mechanic_used (first effective attack)
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlayerTopdownMeleeAttackController),
                    new Dictionary<string, object> {
                        { "input", _lastInput },
                        { "range", attackRange },
                        { "damage", attackDamage },
                        { "dirX", Mathf.Round(lastMoveDirection.x * 100f) / 100f },
                        { "dirY", Mathf.Round(lastMoveDirection.y * 100f) / 100f }
                    }
                );
            }

            attackCooldown = timeBetweenAttacks;
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerTopdownMeleeAttackController),
                new Dictionary<string, object> {
                    { "where", "Attack" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Transform[] points = {
            attackPointUp, attackPointDown, attackPointLeft, attackPointRight,
            attackPointUpLeft, attackPointUpRight, attackPointDownLeft, attackPointDownRight
        };

        Gizmos.color = Color.red;
        foreach (var point in points)
        {
            if (point != null)
                Gizmos.DrawWireSphere(point.position, attackRange);
        }
    }
}
