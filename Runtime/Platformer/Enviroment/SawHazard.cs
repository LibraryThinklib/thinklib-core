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

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Thinklib/Platformer/Environment/Saw Hazard", -98)]
public class SawHazard : MonoBehaviour
{
    [Header("Damage Configuration")]
    [Tooltip("How much damage this saw deals when touching the player.")]
    public int damageAmount = 1;

    [Header("Movement (Patrol)")]
    [Tooltip("Check if this saw should move.")]
    public bool shouldMove = false;

    [Tooltip("Patrol point A (starting position).")]
    public Vector2 pointA;

    [Tooltip("Patrol point B (ending position).")]
    public Vector2 pointB;

    [Tooltip("Patrol movement speed.")]
    public float moveSpeed = 3.0f;

    private const string MechanicName = "Platformer/Enviroment/SawHazard";

    private Vector2 targetPosition;
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(SawHazard),
            new Dictionary<string, object>
            {
                { "damageAmount", damageAmount },
                { "shouldMove", shouldMove },
                { "moveSpeed", moveSpeed }
            });
    }

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (shouldMove)
        {
            transform.position = pointA;
            targetPosition = pointB;
        }
    }

    void Update()
    {
        if (shouldMove) Move();
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance((Vector2)transform.position, targetPosition) < 0.1f)
        {
            targetPosition = (targetPosition == pointA) ? pointB : pointA;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        try
        {
            LifeSystemController lifeSystem = other.GetComponent<LifeSystemController>();

            if (lifeSystem != null)
            {
                lifeSystem.TakeDamage(damageAmount);

                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(SawHazard),
                        new Dictionary<string, object>
                        {
                            { "action", "damage" },
                            { "damageAmount", damageAmount }
                        });
                }
            }
            else
            {
                Debug.LogWarning("Saw touched the 'Player', but couldn't find the 'LifeSystemController' script.");
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track("mechanic_error", MechanicName, nameof(SawHazard),
                new Dictionary<string, object>
                {
                    { "where", "OnTriggerEnter2D" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                });
            throw;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (shouldMove)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointA, 0.3f);
            Gizmos.DrawWireSphere(pointB, 0.3f);
            Gizmos.DrawLine(pointA, pointB);
        }
    }
}
