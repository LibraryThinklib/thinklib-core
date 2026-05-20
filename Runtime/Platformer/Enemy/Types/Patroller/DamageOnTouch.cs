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

[AddComponentMenu("Thinklib/Platformer/Enemy/Patroller/Damage On Touch", -100)]
[RequireComponent(typeof(Collider2D))]
public class DamageOnTouch : MonoBehaviour
{
    [Header("Dano ao tocar no jogador")]
    public int damage = 1;

    [Header("Tag do jogador")]
    public string targetTag = "Player";

    // Telemetry
    private const string MechanicName = "Platformer/Enemy/DamageOnTouch";
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(DamageOnTouch),
            new Dictionary<string, object> {
                { "damage", damage },
                { "targetTag", targetTag }
            }
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(targetTag)) return;

        try
        {
            var life = collision.collider.GetComponent<LifeSystemController>();
            var hurt = collision.collider.GetComponent<PlayerHurtEffect>();

            if (life != null && (hurt == null || !hurt.IsInvulnerable))
            {
                life.TakeDamage(damage);

                if (hurt != null)
                    hurt.TriggerInvulnerability();

                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(DamageOnTouch),
                        new Dictionary<string, object> {
                            { "firstHit", true },
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
                nameof(DamageOnTouch),
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
