// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/TowerDefense/Enemy Progression/Enemy Path", -99)]
public class EnemyPath : MonoBehaviour
{
    public Transform[] waypoints;   // pontos do caminho
    public float speed = 2f;
    private int currentWaypointIndex = 0;

    public int damageAmount = 1;    // Dano causado ao jogador ao atingir o fim do caminho

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/EnemyProgression/EnemyPath";
    private bool _sentInstantiated = false;
    private bool _sentStartPath    = false;
    private bool _sentEndReached   = false;

    private void Start()
    {
        // mechanic_instantiated (1x)
        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(EnemyPath),
                new Dictionary<string, object> {
                    { "waypointsCount", waypoints != null ? waypoints.Length : 0 },
                    { "speed", speed },
                    { "damageAmount", damageAmount }
                }
            );
        }
    }

    private void Update()
    {
        try
        {
            if (waypoints == null || waypoints.Length == 0)
                return;

            if (currentWaypointIndex < waypoints.Length)
            {
                // mechanic_used: primeira movimentação no caminho
                if (!_sentStartPath)
                {
                    _sentStartPath = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(EnemyPath),
                        new Dictionary<string, object> {
                            { "action", "start_path" },
                            { "waypointsCount", waypoints.Length }
                        }
                    );
                }

                // Move o inimigo na direção do waypoint atual
                Transform target = waypoints[currentWaypointIndex];
                Vector3 direction = (target.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;

                // Verifica se chegou perto o suficiente para mudar para o próximo ponto
                float distance = Vector3.Distance(transform.position, target.position);
                if (distance < 0.1f)
                {
                    currentWaypointIndex++;
                }
            }
            else
            {
                // Chegou ao fim do caminho: causa dano ao jogador
                if (!_sentEndReached)
                {
                    _sentEndReached = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(EnemyPath),
                        new Dictionary<string, object> {
                            { "action", "end_reached" },
                            { "damageAmount", damageAmount }
                        }
                    );
                }

                PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageAmount);
                }

                Destroy(gameObject);
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(EnemyPath),
                new Dictionary<string, object> {
                    { "where", "Update" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
