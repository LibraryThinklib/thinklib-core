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

[AddComponentMenu("Thinklib/Platformer/Combat/Player Melee Attack Controller", -99)]
public class PlayerMeleeAttackController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode attackKey = KeyCode.F;
    public Button attackButton;

    [Header("Attack Settings")]
    public float timeBetweenAttacks = 0.8f;
    public int attackDamage = 1;
    public float attackRange = 0.5f;
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Visual Effects")]
    public GameObject slashEffectPrefab;
    public float slashEffectDuration = 0.3f;

    private float attackCooldown = 0f;
    private Animator animator;
    private int direction = 1;

    // Telemetry
    private const string MechanicName = "Platformer/Combat/MeleeAttack";
    private bool _sentUsed = false;
    private string _lastInput = "unknown";

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ConfigureAttackButton();

        // telemetry: componente instanciado
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(PlayerMeleeAttackController),
            new Dictionary<string, object> {
                { "timeBetweenAttacks", timeBetweenAttacks },
                { "attackDamage", attackDamage },
                { "attackRange", attackRange },
                { "hasButton", attackButton != null },
                { "enemyLayersMask", enemyLayers.value }
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

        UpdateDirectionFromScale();
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

    private void UpdateDirectionFromScale()
    {
        direction = transform.localScale.x >= 0 ? 1 : -1;
    }

    private void Attack()
    {
        try
        {
            if (animator != null)
                animator.SetTrigger("IsAttacking");

            Vector2 attackCenter = (Vector2)attackPoint.position + Vector2.right * direction * attackRange * 0.5f;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackCenter, attackRange, enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                var life = enemy.GetComponent<LifeSystemController>();
                if (life != null)
                    life.TakeDamage(attackDamage);
            }

            // Efeito visual
            if (slashEffectPrefab != null)
            {
                Vector3 effectPosition = attackPoint.position;
                GameObject effect = Instantiate(slashEffectPrefab, effectPosition, Quaternion.identity);

                // Flips the effect when facing left
                Vector3 scale = effect.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * direction;
                effect.transform.localScale = scale;

                Destroy(effect, slashEffectDuration);
            }

            // telemetry: first effective use (first attack)
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlayerMeleeAttackController),
                    new Dictionary<string, object> {
                        { "input", _lastInput },
                        { "attackRange", attackRange },
                        { "damage", attackDamage }
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
                nameof(PlayerMeleeAttackController),
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
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Vector2 attackCenter = (Vector2)attackPoint.position + Vector2.right * direction * attackRange * 0.5f;
        Gizmos.DrawWireSphere(attackCenter, attackRange);
    }
}
