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
    public int currentLevel = 1;   // Nível atual da torre
    public int maxLevel = 3;       // Máximo de níveis que a torre pode alcançar

    // Atributos da torre
    public int damage = 1;
    public float fireRate = 1f;
    public float range = 3f;

    public int upgradeCost = 10;   // Custo para evoluir a torre

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

    // Método para melhorar a torre
    public void UpgradeTower()
    {
        try
        {
            if (currentLevel >= maxLevel)
            {
                // Já está no máximo — ainda é útil registrar o uso
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

                Debug.Log("Nível máximo atingido.");
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
                Debug.LogWarning("PlayerScore não encontrado.");
                return;
            }

            if (playerScore.currentScore < upgradeCost)
            {
                // Tentativa com pontos insuficientes (conta como uso para entender demanda)
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

                Debug.Log("Pontos insuficientes.");
                return;
            }

            // --- Upgrade efetivo ---
            int fromLevel = currentLevel;
            int oldDamage = damage;
            float oldFire = fireRate;
            float oldRange = range;

            playerScore.AddScore(-upgradeCost);
            currentLevel++;

            // Atualiza os atributos conforme o novo nível
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

            // Registra o primeiro sucesso de upgrade
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
                // Demais upgrades (se quiser rastrear todos)
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

            // Se acabou de atingir o máximo, registra
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

            Debug.Log("Torre evoluída para o nível " + currentLevel);
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

    // Exibindo as informações da torre (opcional)
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 220, 20), "Nível da Torre: " + currentLevel);
        GUI.Label(new Rect(10, 30, 220, 20), "Dano: " + damage);
        GUI.Label(new Rect(10, 50, 220, 20), "Alcance: " + range);
        GUI.Label(new Rect(10, 70, 220, 20), "Taxa de Disparo: " + fireRate);
    }
}
