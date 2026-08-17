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

[AddComponentMenu("Thinklib/Platformer/Environment/Moving Platform", -100)]
public class MovingPlatform : MonoBehaviour
{
    [Header("Movement Configuration")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private bool startActive = false;

    [Header("Activation Configuration")]
    [SerializeField] private bool requirePlayerInput = false;
    [SerializeField] private KeyCode activationKey = KeyCode.E;

    private Transform targetPoint;
    private bool isWaiting = false;
    private bool isActive = false;
    private bool playerOnPlatform = false;

    // Telemetry
    private const string MechanicName = "Platformer/Environment/MovingPlatform";
    private bool _sentUsed = false;
    private bool _wasActive = false;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("PointA and PointB must be set.");
            enabled = false;
            return;
        }

        transform.position = pointA.position;
        targetPoint = pointB;
        isActive = startActive;

        // telemetry: componente instanciado
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(MovingPlatform),
            new Dictionary<string, object> {
                { "startActive", startActive },
                { "requirePlayerInput", requirePlayerInput },
                { "speed", speed },
                { "waitTime", waitTime }
            }
        );
    }

    private void Update()
    {
        if (requirePlayerInput && playerOnPlatform && Input.GetKeyDown(activationKey))
        {
            isActive = true;
            // telemetry: primeiro uso quando ativado por input
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(MovingPlatform),
                    new Dictionary<string, object> {
                        { "activation", "input" },
                        { "key", activationKey.ToString() }
                    }
                );
            }
        }

        // Covers the case where it became active without input (e.g. startActive = true), logging usage on that transition
        if (isActive && !_wasActive && !_sentUsed)
        {
            _sentUsed = true;
            ThinklibTelemetry.Track(
                "mechanic_used",
                MechanicName,
                nameof(MovingPlatform),
                new Dictionary<string, object> {
                    { "activation", requirePlayerInput ? "input" : "auto" }
                }
            );
        }
        _wasActive = isActive;

        if (!isActive || isWaiting) return;

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        try
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPoint.position) < 0.01f)
            {
                StartCoroutine(WaitAndSwitch());
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(MovingPlatform),
                new Dictionary<string, object> {
                    { "where", "MoveToTarget" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;

        try
        {
            targetPoint = (targetPoint == pointA) ? pointB : pointA;
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(MovingPlatform),
                new Dictionary<string, object> {
                    { "where", "WaitAndSwitch" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
            collision.transform.SetParent(transform); // Makes the player move together with the platform
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
            collision.transform.SetParent(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
