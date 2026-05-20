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

[AddComponentMenu("Thinklib/Platformer/Enemy/Patroller/Patrulheiro AI", -99)]
[RequireComponent(typeof(Animator))]
public class PatrollerAI : MonoBehaviour
{
    [Header("Patrol Points (placed outside the enemy!)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float tolerance = 0.5f;

    private Animator animator;
    private Transform currentTarget;

    // Telemetry
    private const string MechanicName = "Platformer/Enemy/Patroller";
    private bool _sentUsed = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentTarget = pointB;
        Flip();

        // telemetry: componente instanciado
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(PatrollerAI),
            new Dictionary<string, object> {
                { "hasPointA", pointA != null },
                { "hasPointB", pointB != null },
                { "speed", speed },
                { "tolerance", tolerance }
            }
        );
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        try
        {
            if (pointA == null || pointB == null) return;

            animator.SetBool("IsWalking", true);

            // Move toward the current target
            Vector3 before = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, speed * Time.deltaTime);

            // telemetry: primeiro uso real (quando de fato se move pela primeira vez)
            if (!_sentUsed && (transform.position - before).sqrMagnitude > 0.000001f)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PatrollerAI),
                    new Dictionary<string, object> {
                        { "trigger", "first_move" }
                    }
                );
            }

            // Reached destination?
            if (Vector2.Distance(transform.position, currentTarget.position) <= tolerance)
            {
                SwitchTarget();
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PatrollerAI),
                new Dictionary<string, object> {
                    { "where", "Patrol" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void SwitchTarget()
    {
        try
        {
            // Toggle between point A and B
            currentTarget = currentTarget == pointA ? pointB : pointA;
            Flip();
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PatrollerAI),
                new Dictionary<string, object> {
                    { "where", "SwitchTarget" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        float direction = currentTarget.position.x - transform.position.x;

        if (direction > 0f)
            scale.x = Mathf.Abs(scale.x); // facing right
        else if (direction < 0f)
            scale.x = -Mathf.Abs(scale.x); // facing left

        transform.localScale = scale;
    }
}
