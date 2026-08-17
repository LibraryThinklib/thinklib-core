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
    private bool _sentUsed = false;    // fires the first "used" event (when the bullet gets a target)
    private bool _sentHit  = false;    // optional: marks the first hit

    private void Awake()
    {
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

            // mechanic_used: first time the bullet receives a target (this is when actual use begins)
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
                Destroy(gameObject);
                return;
            }

            Vector3 direction = (target.position - transform.position).normalized;

            transform.position += direction * speed * Time.deltaTime;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < 0.1f)
            {
                EnemyHealth eh = target.GetComponent<EnemyHealth>();
                if (eh != null)
                {
                    eh.TakeDamage(damage);

                    // (Optional) mechanic_used: first hit
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

                Destroy(gameObject);
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
