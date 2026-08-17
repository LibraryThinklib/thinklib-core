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

[AddComponentMenu("Thinklib/Platformer/Enemy/Shooter/Projectile Damage Dealer", -89)]
[RequireComponent(typeof(Collider2D))]
public class ProjectileDamageDealer : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;

    [Header("Collision Configuration")]
    public string targetTag = "Player";

    // Telemetry
    private const string MechanicName = "Platformer/Enemy/Projectile";
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(ProjectileDamageDealer),
            new Dictionary<string, object> {
                { "damage", damage },
                { "targetTag", targetTag }
            }
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        try
        {
            if (!other.CompareTag(targetTag)) return;

            var life = other.GetComponent<LifeSystemController>();
            var hurtEffect = other.GetComponent<PlayerHurtEffect>();

            if (life != null && (hurtEffect == null || !hurtEffect.IsInvulnerable))
            {
                life.TakeDamage(damage);

                if (hurtEffect != null)
                    hurtEffect.TriggerInvulnerability();

                // First effective use of the projectile (a hit)
                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(ProjectileDamageDealer),
                        new Dictionary<string, object> {
                            { "hitTag", targetTag },
                            { "damage", damage }
                        }
                    );
                }
            }

            Destroy(gameObject);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(ProjectileDamageDealer),
                new Dictionary<string, object> {
                    { "where", "OnTriggerEnter2D" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
