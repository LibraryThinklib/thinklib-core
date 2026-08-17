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

[AddComponentMenu("Thinklib/Topdown/Enemy/Patroller/Patroller AI", -99)]
[RequireComponent(typeof(Animator))]
public class TopdownPatrollerAI : MonoBehaviour
{
    [Header("Patrol Points (placed outside the enemy!)")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    [Header("Settings")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float tolerance = 0.2f;

    private Animator animator;
    private Transform currentTarget;
    private Vector2 lastDirection = Vector2.down;

    private const string MechanicName = "Topdown/Enemy/Patroller/PatrollerAI";
    private bool _sentUsedMove = false;
    private bool _sentUsedSwitch = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentTarget = pointB;

        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(TopdownPatrollerAI),
            new Dictionary<string, object> {
                { "speed", speed },
                { "tolerance", tolerance },
                { "hasPointA", pointA != null },
                { "hasPointB", pointB != null }
            }
        );
    }

    private void Update()
    {
        try
        {
            Patrol();
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TopdownPatrollerAI),
                new Dictionary<string, object> {
                    { "where", "Update/Patrol" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void Patrol()
    {
        if (pointA == null || pointB == null) return;

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition  = currentTarget.position;
        Vector2 direction       = (targetPosition - currentPosition).normalized;

        transform.position = Vector2.MoveTowards(currentPosition, targetPosition, speed * Time.deltaTime);

        bool isMoving = direction.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical",   direction.y);
            lastDirection = direction;

            // mechanic_used: first time it starts moving
            if (!_sentUsedMove)
            {
                _sentUsedMove = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TopdownPatrollerAI),
                    new Dictionary<string, object> {
                        { "action", "start_move" },
                        { "dirX", direction.x },
                        { "dirY", direction.y },
                        { "speed", speed }
                    }
                );
            }
        }
        else
        {
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical",   lastDirection.y);
        }

        if (Vector2.Distance(currentPosition, targetPosition) <= tolerance)
        {
            SwitchTarget();
        }
    }

    private void SwitchTarget()
    {
        currentTarget = currentTarget == pointA ? pointB : pointA;

        // optional: log the first point switch
        if (!_sentUsedSwitch)
        {
            _sentUsedSwitch = true;
            ThinklibTelemetry.Track(
                "mechanic_used",
                MechanicName,
                nameof(TopdownPatrollerAI),
                new Dictionary<string, object> {
                    { "action", "switch_target" },
                    { "next", currentTarget == pointA ? "A" : "B" }
                }
            );
        }
    }
}
