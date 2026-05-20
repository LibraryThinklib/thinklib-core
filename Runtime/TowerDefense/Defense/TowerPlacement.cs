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

[AddComponentMenu("Thinklib/TowerDefense/Defense/Tower Placement", -99)]
public class TowerPlacement : MonoBehaviour
{
    public GameObject towerPrefab;
    private TowerShop towerShop;

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/Defense/TowerPlacement";
    private bool _sentInstantiated = false;

    private void Start()
    {
        towerShop = FindObjectOfType<TowerShop>();

        // mechanic_instantiated (apenas 1x)
        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(TowerPlacement),
                new Dictionary<string, object> {
                    { "hasTowerPrefab", towerPrefab != null }
                }
            );
        }
    }

    private void Update()
    {
        try
        {
            if (towerShop == null || !towerShop.IsPlacingTower()) return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;

                RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

                if (hit.collider != null && hit.collider.CompareTag("Blocked"))
                {
                    // tentativa bloqueada
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(TowerPlacement),
                        new Dictionary<string, object> {
                            { "action", "attempt_place_blocked" },
                            { "x", mousePosition.x },
                            { "y", mousePosition.y },
                            { "prefab", towerPrefab ? towerPrefab.name : "null" },
                            { "blockedCollider", hit.collider.name }
                        }
                    );

                    Debug.Log("Área bloqueada! Não é possível colocar a torre aqui.");
                    return;
                }

                // posição válida ⇒ instanciar torre
                if (towerPrefab != null)
                {
                    Instantiate(towerPrefab, mousePosition, Quaternion.identity);
                }
                Debug.Log("Torre colocada!");

                // mechanic_used: primeira colocação efetiva
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerPlacement),
                    new Dictionary<string, object> {
                        { "action", "placed" },
                        { "x", mousePosition.x },
                        { "y", mousePosition.y },
                        { "prefab", towerPrefab ? towerPrefab.name : "null" }
                    }
                );

                towerShop.StopPlacingTower();
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TowerPlacement),
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
