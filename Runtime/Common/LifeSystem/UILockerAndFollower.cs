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

[AddComponentMenu("Thinklib/Common/LifeSystem/UI Locker And Follower", -87)]
public class UILockerAndFollower : MonoBehaviour
{
    [Tooltip("Target's transform (the object to follow)")]
    public Transform target;

    [Tooltip("UI offset relative to the target")]
    public Vector3 offset = new Vector3(0, 1.5f, 0);

    private Vector3 initialScale;
    private Quaternion initialRotation;

    // Telemetry
    private const string MechanicName = "Common/LifeSystem/UILockerAndFollower";
    private bool _sentUsed = false;

    private void Awake()
    {
        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(UILockerAndFollower),
            new Dictionary<string, object> {
                { "hasTarget", target != null },
                { "offsetX", offset.x },
                { "offsetY", offset.y },
                { "offsetZ", offset.z }
            }
        );
    }

    private void Start()
    {
        initialScale = transform.localScale;
        initialRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        try
        {
            // Posicionar a UI no local desejado
            transform.position = target.position + offset;

            // Keep the original rotation and scale (stops the UI from flipping/rotating with the target)
            transform.rotation = initialRotation;

            // Correct the scale so it doesn't flip along with the target
            Vector3 correctedScale = initialScale;
            correctedScale.x = Mathf.Abs(initialScale.x);
            transform.localScale = correctedScale;

            // mechanic_used: first time it actually follows the target
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(UILockerAndFollower),
                    new Dictionary<string, object> {
                        { "trigger", "first_follow" }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(UILockerAndFollower),
                new Dictionary<string, object> {
                    { "where", "LateUpdate" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
