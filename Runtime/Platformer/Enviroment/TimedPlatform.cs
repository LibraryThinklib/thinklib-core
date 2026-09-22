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

[AddComponentMenu("Thinklib/Platformer/Environment/Timed Platform", -99)]
[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class TimedPlatform : MonoBehaviour
{
    public enum PlatformBehavior
    {
        Fall,
        Disappear
    }

    public PlatformBehavior behavior = PlatformBehavior.Disappear;

    public float delayBeforeAction = 1f;
    public float fadeDuration = 1f;

    public bool enableRespawn = false;
    public float respawnDelay = 2f;

    public string activatorTag = "Player";

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer[] spriteRenderers;
    private Color[] originalColors;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isTriggered = false;

    // Telemetry
    private const string MechanicName = "Platformer/TimedPlatform";
    private bool _sentUsed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);

        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColors[i] = spriteRenderers[i].color;
        }

        originalPosition = transform.position;
        originalRotation = transform.rotation;

        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(TimedPlatform),
            new Dictionary<string, object> {
                { "behavior", behavior.ToString() },
                { "enableRespawn", enableRespawn },
                { "delayBeforeAction", delayBeforeAction },
                { "fadeDuration", fadeDuration },
                { "respawnDelay", respawnDelay }
            }
        );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isTriggered) return;

        if (collision.gameObject.CompareTag(activatorTag))
        {
            isTriggered = true;
            Invoke(nameof(TriggerAction), delayBeforeAction);
        }
    }

    private void TriggerAction()
    {
        try
        {
            if (behavior == PlatformBehavior.Fall)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;

                if (enableRespawn)
                    Invoke(nameof(ResetPlatform), respawnDelay);
            }
            else if (behavior == PlatformBehavior.Disappear)
            {
                StartCoroutine(FadeOutAndDisable());
            }

            // telemetry: first effective use (first activation)
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TimedPlatform),
                    new Dictionary<string, object> {
                        { "behavior", behavior.ToString() }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TimedPlatform),
                new Dictionary<string, object> {
                    { "where", "TriggerAction" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    // Iterators (yield) can't have try/catch — use helpers without yield to catch/log errors, and try/finally here.
    private IEnumerator FadeOutAndDisable()
    {
        float elapsed = 0f;

        try
        {
            while (elapsed < fadeDuration)
            {
                // Step without yield -> can have an internal try/catch
                SafeSetAlpha(Mathf.Lerp(1f, 0f, elapsed / fadeDuration));
                elapsed += Time.deltaTime;

                yield return null;
            }
        }
        finally
        {
            SafeFinalizeFadeAndScheduleRespawn();
        }
    }

    private void SafeSetAlpha(float t)
    {
        try
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color original = originalColors[i];
                spriteRenderers[i].color = new Color(original.r, original.g, original.b, original.a * t);
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TimedPlatform),
                new Dictionary<string, object> {
                    { "where", "SafeSetAlpha" },
                    { "t", t },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void SafeFinalizeFadeAndScheduleRespawn()
    {
        try
        {
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                Color original = originalColors[i];
                spriteRenderers[i].color = new Color(original.r, original.g, original.b, 0f);
                spriteRenderers[i].enabled = false;
            }

            col.enabled = false;

            if (enableRespawn)
                Invoke(nameof(ResetPlatform), respawnDelay);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TimedPlatform),
                new Dictionary<string, object> {
                    { "where", "SafeFinalizeFadeAndScheduleRespawn" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            // usually not re-thrown here, so it doesn't crash the game; adjust if you prefer otherwise.
        }
    }

    private void ResetPlatform()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].enabled = true;
            spriteRenderers[i].color = originalColors[i];
        }

        col.enabled = true;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.bodyType = RigidbodyType2D.Kinematic;

        isTriggered = false;
    }
}
