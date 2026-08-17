// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/TowerDefense/Enemy Progression/Enemy Spawner", -98)]
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] waypoints;
    public int enemiesToSpawn = 5;
    public float spawnInterval = 2f;

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/EnemyProgression/EnemySpawner";
    private bool _sentInstantiated = false;
    private bool _sentUsed = false;

    private void Start()
    {
        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(EnemySpawner),
                new Dictionary<string, object> {
                    { "enemiesToSpawn", enemiesToSpawn },
                    { "spawnInterval", spawnInterval },
                    { "waypointsCount", waypoints != null ? waypoints.Length : 0 },
                    { "hasPrefab", enemyPrefab != null }
                }
            );
        }

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        // Important note about try/catch + yield:
        // we don't put yield inside a try-catch, to avoid the C# compiler error.
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            bool ok = SpawnOne(i);

            // mechanic_used (first real use: 1st successful spawn)
            if (!_sentUsed && ok)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(EnemySpawner),
                    new Dictionary<string, object> {
                        { "action", "spawn_started" },
                        { "firstIndex", i },
                        { "plannedTotal", enemiesToSpawn }
                    }
                );
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Instantiates an enemy and injects the waypoints. No yield here, so we can use try/catch.
    /// </summary>
    private bool SpawnOne(int index)
    {
        try
        {
            if (enemyPrefab == null)
            {
                ThinklibTelemetry.Track(
                    "mechanic_error",
                    MechanicName,
                    nameof(EnemySpawner),
                    new Dictionary<string, object> {
                        { "where", "SpawnOne" },
                        { "index", index },
                        { "message", "enemyPrefab is null" }
                    }
                );
                return false;
            }

            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            var enemyPath = enemy.GetComponent<EnemyPath>();
            if (enemyPath != null)
            {
                enemyPath.waypoints = waypoints;
            }

            return true;
        }
        catch (System.Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(EnemySpawner),
                new Dictionary<string, object> {
                    { "where", "SpawnOne" },
                    { "index", index },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            return false;
        }
    }
}
