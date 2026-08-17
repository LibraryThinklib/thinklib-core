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

[AddComponentMenu("Thinklib/Topdown/Movement/Movement Controller", -100)]
[RequireComponent(typeof(Animator))]
public class TopdownMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Joystick joystick;

    [Header("Input Settings")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    private Animator animator;
    private InputHandler inputHandler;

    /// <summary>
    /// Stores the last valid direction (used for directional idle).
    /// </summary>
    private Vector2 lastMoveDirection = Vector2.down;

    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }

    private const string MechanicName = "Topdown/Movement/TopdownMovementController";
    private bool _sentUsedMove = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<InputHandler>() ?? gameObject.AddComponent<InputHandler>();

        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(TopdownMovementController),
            new Dictionary<string, object> {
                { "moveSpeed", moveSpeed },
                { "hasJoystick", joystick != null },
                { "keys", new Dictionary<string, object> {
                    { "up",   upKey.ToString() },
                    { "down", downKey.ToString() },
                    { "left", leftKey.ToString() },
                    { "right", rightKey.ToString() }
                }}
            }
        );
    }

    private void Update()
    {
        try
        {
            Vector2 input = GetInput();

            Move(input);

            UpdateAnimator(input);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TopdownMovementController),
                new Dictionary<string, object> {
                    { "where", "Update" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private Vector2 GetInput()
    {
        if (joystick != null)
        {
            return new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            float h = 0f;
            float v = 0f;

            if (Input.GetKey(leftKey))  h -= 1f;
            if (Input.GetKey(rightKey)) h += 1f;
            if (Input.GetKey(downKey))  v -= 1f;
            if (Input.GetKey(upKey))    v += 1f;

            return new Vector2(h, v).normalized;
        }
    }

    private void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            // First time the object actually moved.
            if (!_sentUsedMove)
            {
                _sentUsedMove = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TopdownMovementController),
                    new Dictionary<string, object> {
                        { "action", "start_move" },
                        { "dirX", direction.x },
                        { "dirY", direction.y },
                        { "speed", moveSpeed }
                    }
                );
            }

            lastMoveDirection = direction;
        }
    }

    private void UpdateAnimator(Vector2 direction)
    {
        bool isMoving = direction.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
        }
        else
        {
            // Keeps using the last saved direction for the Idle state.
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical",   lastMoveDirection.y);
        }
    }
}
