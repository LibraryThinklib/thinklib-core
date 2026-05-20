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

[AddComponentMenu("Thinklib/TowerDefense/Defense/Bullet", -100)]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 5f;
    public int damage = 1;

    private Transform target;

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/Defense/Bullet";
    private bool _sentUsed = false;    // envio do primeiro "uso" (quando ganha um alvo)
    private bool _sentHit  = false;    // opcional: marcar primeiro acerto

    private void Awake()
    {
        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(Bullet),
            new Dictionary<string, object> {
                { "speed", speed },
                { "damage", damage }
            }
        );
    }

    public void SetTarget(Transform newTarget)
    {
        try
        {
            target = newTarget;

            // mechanic_used: primeira vez que a bala recebe um alvo (momento em que o uso começa de fato)
            if (!_sentUsed && target != null)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(Bullet),
                    new Dictionary<string, object> {
                        { "action", "set_target" },
                        { "targetName", target.name }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(Bullet),
                new Dictionary<string, object> {
                    { "where", "SetTarget" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void Update()
    {
        try
        {
            if (target == null)
            {
                Destroy(gameObject); // Destrói a bala se o alvo não existir
                return;
            }

            // Direção do movimento (do centro da bala até o alvo)
            Vector3 direction = (target.position - transform.position).normalized;

            // Movimenta a bala em direção ao alvo
            transform.position += direction * speed * Time.deltaTime;

            // Rotaciona a bala para que a ponta esteja sempre voltada para o alvo
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            // Se a bala atingir o inimigo
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 0.1f)
            {
                EnemyHealth eh = target.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    eh.TakeDamage(damage);

                    // (Opcional) mechanic_used: primeiro hit
                    if (!_sentHit)
                    {
                        _sentHit = true;
                        ThinklibTelemetry.Track(
                            "mechanic_used",
                            MechanicName,
                            nameof(Bullet),
                            new Dictionary<string, object> {
                                { "action", "hit" },
                                { "targetName", target.name },
                                { "damage", damage }
                            }
                        );
                    }
                }

                Destroy(gameObject); // Destrói a bala
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(Bullet),
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
