// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Common/Effects/Death Effect", -100)]
[RequireComponent(typeof(SpriteRenderer))]
public class DeathEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float blinkDuration = 1.2f;
    public float blinkInterval = 0.1f;

    [Header("UI to Hide on Death (Optional)")]
    public GameObject[] uiElementsToHide;

    private SpriteRenderer spriteRenderer;

    // Telemetry
    private const string MechanicName = "Common/Effects/DeathEffect";
    private bool _sentUsed = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(DeathEffect),
            new Dictionary<string, object> {
                { "blinkDuration", blinkDuration },
                { "blinkInterval", blinkInterval },
                { "uiElementsCount", uiElementsToHide != null ? uiElementsToHide.Length : 0 }
            }
        );
    }

    /// <summary>
    /// Starts the death effect with blinking and destroys the object.
    /// </summary>
    public void PlayDeathEffect()
    {
        try
        {
            HideUIElements();

            // mechanic_used (first time the effect runs)
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(DeathEffect),
                    new Dictionary<string, object> {
                        { "blinkDuration", blinkDuration },
                        { "blinkInterval", blinkInterval }
                    }
                );
            }

            StartCoroutine(BlinkAndDestroy());
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(DeathEffect),
                new Dictionary<string, object> {
                    { "where", "PlayDeathEffect" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void HideUIElements()
    {
        if (uiElementsToHide == null) return;

        foreach (var ui in uiElementsToHide)
        {
            if (ui != null)
                ui.SetActive(false);
        }
    }

    // >>> Don't use try/catch in iterators. Use yield-free helpers for error telemetry, and try/finally here.
    private IEnumerator BlinkAndDestroy()
    {
        float timer = 0f;

        try
        {
            while (timer < blinkDuration)
            {
                // Yield-free operation -> can have internal try/catch with telemetry
                BlinkStepSafe();

                yield return new WaitForSeconds(blinkInterval);
                timer += blinkInterval;
            }
        }
        finally
        {
            SafeDestroy();
        }
    }

    // Yield-free helper: exceptions can be caught and reported via telemetry here
    private void BlinkStepSafe()
    {
        try
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(DeathEffect),
                new Dictionary<string, object> {
                    { "where", "BlinkStepSafe" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void SafeDestroy()
    {
        try
        {
            Destroy(gameObject);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(DeathEffect),
                new Dictionary<string, object> {
                    { "where", "SafeDestroy" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
        }
    }
}
