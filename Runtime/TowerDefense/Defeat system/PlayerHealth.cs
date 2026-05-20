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

[AddComponentMenu("Thinklib/TowerDefense/Defeat System/Player Health", -100)]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;       // Quantidade inicial de vidas
    private int currentHealth;

    [Header("UI")]
    public Text healthText;          // Exibe as vidas
    public Text gameOverText;        // Exibe "GAME OVER"

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/DefeatSystem/PlayerHealth";
    private bool _sentUsedFirstDamage = false;
    private bool _sentUsedGameOver    = false;

    private void Awake()
    {
        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(PlayerHealth),
            new Dictionary<string, object> {
                { "maxHealth", maxHealth },
                { "hasHealthText", healthText != null },
                { "hasGameOverText", gameOverText != null }
            }
        );
    }

    private void Start()
    {
        try
        {
            currentHealth = maxHealth;
            UpdateHealthUI();
            if (gameOverText != null)
                gameOverText.gameObject.SetActive(false);  // Garante que "GAME OVER" comece oculto
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerHealth),
                new Dictionary<string, object> {
                    { "where", "Start" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    public void TakeDamage(int amount)
    {
        try
        {
            currentHealth -= amount;
            UpdateHealthUI();

            // mechanic_used: primeira vez que recebe dano
            if (!_sentUsedFirstDamage)
            {
                _sentUsedFirstDamage = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlayerHealth),
                    new Dictionary<string, object> {
                        { "action", "take_damage_first_time" },
                        { "amount", amount },
                        { "currentHealth", currentHealth },
                        { "maxHealth", maxHealth }
                    }
                );
            }

            if (currentHealth <= 0)
            {
                GameOver();
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerHealth),
                new Dictionary<string, object> {
                    { "where", "TakeDamage" },
                    { "amount", amount },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void UpdateHealthUI()
    {
        try
        {
            if (healthText != null)
            {
                healthText.text = "Lifes: " + currentHealth.ToString();
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerHealth),
                new Dictionary<string, object> {
                    { "where", "UpdateHealthUI" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void GameOver()
    {
        try
        {
            if (gameOverText != null)
                gameOverText.gameObject.SetActive(true);

            // mechanic_used: primeira transição para game over
            if (!_sentUsedGameOver)
            {
                _sentUsedGameOver = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlayerHealth),
                    new Dictionary<string, object> {
                        { "action", "game_over" },
                        { "finalHealth", currentHealth },
                        { "maxHealth", maxHealth }
                    }
                );
            }

            Time.timeScale = 0f; // Pausa o jogo
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerHealth),
                new Dictionary<string, object> {
                    { "where", "GameOver" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
