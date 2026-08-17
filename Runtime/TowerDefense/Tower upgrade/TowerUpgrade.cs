// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;
using System.Collections.Generic;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/TowerDefense/Tower Upgrade/Tower Upgrade", -99)]
public class TowerUpgrade : MonoBehaviour
{
    public int currentLevel = 1;
    public int maxLevel = 3;

    public int damage = 1;
    public float fireRate = 1f;
    public float range = 3f;

    public int upgradeCost = 10;

    private PlayerScore playerScore;

    // Telemetry
    private const string MechanicName = "TowerDefense/TowerUpgrade/TowerUpgrade";
    private bool _sentInstantiated = false;
    private bool _sentFirstSuccess = false;

    void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();

        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            Thinklib.Telemetry.ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(TowerUpgrade),
                new Dictionary<string, object>
                {
                    { "currentLevel", currentLevel },
                    { "maxLevel", maxLevel },
                    { "damage", damage },
                    { "fireRate", fireRate },
                    { "range", range },
                    { "upgradeCost", upgradeCost },
                    { "hasPlayerScore", playerScore != null }
                }
            );
        }
    }

    public void UpgradeTower()
    {
        try
        {
            if (currentLevel >= maxLevel)
            {
                // Already at max level — still useful to log the usage
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerUpgrade),
                    new Dictionary<string, object>
                    {
                        { "action", "already_at_max_level" },
                        { "currentLevel", currentLevel },
                        { "maxLevel", maxLevel }
                    }
                );

                Debug.Log("Maximum level reached.");
                return;
            }

            if (playerScore == null)
            {
                ThinklibTelemetry.Track(
                    "mechanic_error",
                    MechanicName,
                    nameof(TowerUpgrade),
                    new Dictionary<string, object>
                    {
                        { "where", "UpgradeTower" },
                        { "message", "PlayerScore reference is null" }
                    }
                );
                Debug.LogWarning("PlayerScore not found.");
                return;
            }

            if (playerScore.currentScore < upgradeCost)
            {
                // Attempt with insufficient points (counted as usage to understand demand)
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerUpgrade),
                    new Dictionary<string, object>
                    {
                        { "action", "insufficient_funds" },
                        { "currentScore", playerScore.currentScore },
                        { "required", upgradeCost },
                        { "currentLevel", currentLevel },
                        { "maxLevel", maxLevel }
                    }
                );

                Debug.Log("Insufficient points.");
                return;
            }

            int fromLevel = currentLevel;
            int oldDamage = damage;
            float oldFire = fireRate;
            float oldRange = range;

            playerScore.AddScore(-upgradeCost);
            currentLevel++;

            switch (currentLevel)
            {
                case 2:
                    damage = 3;
                    fireRate = 1.5f;
                    range = 4f;
                    upgradeCost = 20;
                    break;
                case 3:
                    damage = 5;
                    fireRate = 2f;
                    range = 5f;
                    upgradeCost = 30;
                    break;
                default:
                    break;
            }

            if (!_sentFirstSuccess)
            {
                _sentFirstSuccess = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerUpgrade),
                    new Dictionary<string, object>
                    {
                        { "action", "first_upgrade_success" },
                        { "fromLevel", fromLevel },
                        { "toLevel", currentLevel },
                        { "oldDamage", oldDamage }, { "newDamage", damage },
                        { "oldFireRate", oldFire }, { "newFireRate", fireRate },
                        { "oldRange", oldRange },   { "newRange", range }
                    }
                );
            }
            else
            {
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerUpgrade),
                    new Dictionary<string, object>
                    {
                        { "action", "upgrade_success" },
                        { "fromLevel", fromLevel },
                        { "toLevel", currentLevel },
                        { "oldDamage", oldDamage }, { "newDamage", damage },
                        { "oldFireRate", oldFire }, { "newFireRate", fireRate },
                        { "oldRange", oldRange },   { "newRange", range }
                    }
                );
            }

            if (currentLevel >= maxLevel)
            {
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerUpgrade),
                    new Dictionary<string, object>
                    {
                        { "action", "reached_max_level" },
                        { "level", currentLevel }
                    }
                );
            }

            Debug.Log("Tower upgraded to level " + currentLevel);
        }
        catch (System.Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TowerUpgrade),
                new Dictionary<string, object>
                {
                    { "where", "UpgradeTower" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace },
                    { "currentLevel", currentLevel },
                    { "maxLevel", maxLevel }
                }
            );
            throw;
        }
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 220, 20), "Tower Level: " + currentLevel);
        GUI.Label(new Rect(10, 30, 220, 20), "Damage: " + damage);
        GUI.Label(new Rect(10, 50, 220, 20), "Range: " + range);
        GUI.Label(new Rect(10, 70, 220, 20), "Fire Rate: " + fireRate);
    }
}
