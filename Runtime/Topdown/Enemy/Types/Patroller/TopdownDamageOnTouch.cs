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

[AddComponentMenu("Thinklib/Topdown/Enemy/Patroller/Damage On Touch", -100)]
[RequireComponent(typeof(Collider2D))]
public class TopdownDamageOnTouch : MonoBehaviour
{
    [Header("Dano ao tocar no jogador")]
    public int damage = 1;

    [Header("Tag do jogador")]
    public string targetTag = "Player";

    // Telemetry
    private const string MechanicName = "Topdown/Enemy/Patroller/DamageOnTouch";
    private bool _sentUsed = false;

    private void Awake()
    {
        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(TopdownDamageOnTouch),
            new Dictionary<string, object> {
                { "damage", damage },
                { "targetTag", targetTag }
            }
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        try
        {
            if (!collision.collider.CompareTag(targetTag)) return;

            var life = collision.collider.GetComponent<LifeSystemController>();
            var hurt = collision.collider.GetComponent<PlayerHurtEffect>();

            if (life != null && (hurt == null || !hurt.IsInvulnerable))
            {
                life.TakeDamage(damage);

                if (hurt != null)
                    hurt.TriggerInvulnerability();

                // mechanic_used: primeiro hit
                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(TopdownDamageOnTouch),
                        new Dictionary<string, object> {
                            { "action", "touch_hit" },
                            { "damage", damage }
                        }
                    );
                }
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TopdownDamageOnTouch),
                new Dictionary<string, object> {
                    { "where", "OnCollisionEnter2D" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
