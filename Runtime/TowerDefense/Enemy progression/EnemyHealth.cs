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

[AddComponentMenu("Thinklib/TowerDefense/Enemy Progression/Enemy Health", -100)]
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public int pointsOnDeath = 5;

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/EnemyProgression/EnemyHealth";
    private bool _sentInstantiated = false;
    private bool _sentFirstHit     = false;
    private bool _sentDeath        = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(EnemyHealth),
                new Dictionary<string, object> {
                    { "maxHealth", maxHealth },
                    { "pointsOnDeath", pointsOnDeath }
                }
            );
        }
    }

    public void TakeDamage(int amount)
    {
        try
        {
            currentHealth -= amount;

            // mechanic_used: first damage received
            if (!_sentFirstHit)
            {
                _sentFirstHit = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(EnemyHealth),
                    new Dictionary<string, object> {
                        { "action", "take_damage_first" },
                        { "amount", amount },
                        { "remainingHealth", currentHealth }
                    }
                );
            }

            if (currentHealth <= 0)
            {
                // mechanic_used: enemy death
                if (!_sentDeath)
                {
                    _sentDeath = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(EnemyHealth),
                        new Dictionary<string, object> {
                            { "action", "death" },
                            { "pointsOnDeath", pointsOnDeath }
                        }
                    );
                }

                PlayerScore playerScore = FindObjectOfType<PlayerScore>();
                if (playerScore != null)
                {
                    playerScore.AddScore(pointsOnDeath);
                }

                Destroy(gameObject);
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(EnemyHealth),
                new Dictionary<string, object> {
                    { "where", "TakeDamage" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
