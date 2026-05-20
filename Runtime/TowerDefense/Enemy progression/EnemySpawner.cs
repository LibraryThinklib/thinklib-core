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
    public GameObject enemyPrefab;     // Prefab do inimigo a ser instanciado
    public Transform[] waypoints;      // Caminho que será passado para o inimigo
    public int enemiesToSpawn = 5;     // Quantos inimigos criar
    public float spawnInterval = 2f;   // Intervalo entre cada inimigo

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/EnemyProgression/EnemySpawner";
    private bool _sentInstantiated = false;
    private bool _sentUsed = false;

    private void Start()
    {
        // mechanic_instantiated (1x)
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
        // Observação importante sobre try/catch + yield:
        // não colocamos yield dentro de um try-catch para evitar o erro do C#.
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            bool ok = SpawnOne(i);

            // mechanic_used (primeiro uso efetivo: 1º spawn com sucesso)
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

            // espera entre spawns
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    /// <summary>
    /// Instancia um inimigo e injeta os waypoints. Sem yield aqui, então podemos usar try/catch.
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
