// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Platformer/Movement/Movement Controller", -97)]
[RequireComponent(typeof(Animator))]
public class PlatformerMovementController : MovementController
{
    [Header("Movement Settings")]
    public List<KeyCode> rightKeys = new List<KeyCode> { KeyCode.D, KeyCode.RightArrow };
    public List<KeyCode> leftKeys  = new List<KeyCode> { KeyCode.A, KeyCode.LeftArrow };
    public Joystick joystick;

    [Header("Speed Settings")]
    public float walkSpeed = 5f;
    public float runSpeed  = 8f;
    public KeyCode runKey  = KeyCode.LeftShift;

    [Header("Player State")]
    /// <summary>
    /// Updated externally by the jump controller script.
    /// </summary>
    public bool isJumping = false;
    /// <summary>
    /// Updated externally by the jump controller script.
    /// </summary>
    public bool isFalling = false;

    [Header("Attack Settings")]
    public PlatformerProjectileAttackController projectileAttackController;

    private InputHandler inputHandler;
    private Animator animator;
    private bool isFacingRight = true;

    private const string MechanicName = "Platformer/Movement";
    private bool _sentUsed = false;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>() ?? gameObject.AddComponent<InputHandler>();
        animator = GetComponent<Animator>();

        ThinklibTelemetry.Track(
            eventName: "mechanic_instantiated",
            mechanic:  MechanicName,
            className: nameof(PlatformerMovementController)
        );
    }

    private void Update()
    {
        Vector2 inputDirection = joystick != null
            ? inputHandler.GetJoystickInput(joystick)
            : inputHandler.GetKeyboardInput(rightKeys, leftKeys);

        // Sends mechanic_used only the first time there is real input.
        if (!_sentUsed && inputDirection.sqrMagnitude > 0.0001f)
        {
            _sentUsed = true;
            string inputType = joystick != null ? "joystick" : "keyboard";
            ThinklibTelemetry.Track(
                eventName: "mechanic_used",
                mechanic:  MechanicName,
                className: nameof(PlatformerMovementController),
                extra: new Dictionary<string, object> { { "input", inputType } }
            );
        }

        float speed = Input.GetKey(runKey) ? runSpeed : walkSpeed;

        Move(inputDirection, speed);

        UpdateAnimator(inputDirection, speed);

        FlipSprite(inputDirection.x);
    }

    private void UpdateAnimator(Vector2 inputDirection, float speed)
    {
        animator.SetBool("IsMoving", inputDirection.magnitude > 0);
        animator.SetFloat("MoveSpeed", Mathf.Abs(speed));

        isJumping = animator.GetBool("IsJumping");
        isFalling = animator.GetBool("IsFalling");
    }

    private void FlipSprite(float horizontalInput)
    {
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;

        if (projectileAttackController != null)
        {
            int newDirection = isFacingRight ? 1 : -1;
            projectileAttackController.SetDirection(newDirection);
        }
    }
}
