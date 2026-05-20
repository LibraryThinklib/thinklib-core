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
        Fall,       // A plataforma cai
        Disappear   // A plataforma desaparece gradualmente
    }

    public PlatformBehavior behavior = PlatformBehavior.Disappear;

    public float delayBeforeAction = 1f;   // Tempo antes da ação ocorrer
    public float fadeDuration = 1f;        // Tempo para desaparecer (fade out)

    public bool enableRespawn = false;     // Define se a plataforma reaparece
    public float respawnDelay = 2f;        // Tempo até reaparecer

    public string activatorTag = "Player"; // Quem pode ativar a plataforma

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private Color originalColor;
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
        sr = GetComponent<SpriteRenderer>();

        originalColor = sr.color;
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // telemetry: componente instanciado
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

            // telemetry: primeiro uso efetivo (primeira ativação)
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

    // Iteradores (yield) não podem ter try/catch — use helpers sem yield para capturar/telemetrar erros e try/finally aqui.
    private IEnumerator FadeOutAndDisable()
    {
        float elapsed = 0f;

        try
        {
            while (elapsed < fadeDuration)
            {
                // Passo sem yield → pode ter try/catch interno
                SafeSetAlpha(Mathf.Lerp(originalColor.a, 0f, elapsed / fadeDuration));
                elapsed += Time.deltaTime;

                // yield fica “limpo”
                yield return null;
            }
        }
        finally
        {
            SafeFinalizeFadeAndScheduleRespawn();
        }
    }

    private void SafeSetAlpha(float alpha)
    {
        try
        {
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, alpha);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TimedPlatform),
                new Dictionary<string, object> {
                    { "where", "SafeSetAlpha" },
                    { "alpha", alpha },
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
            // força alpha 0 e desabilita
            var c = sr.color;
            sr.color = new Color(c.r, c.g, c.b, 0f);
            sr.enabled = false;
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
            // normalmente não relança aqui para não derrubar o jogo; ajuste se preferir.
        }
    }

    private void ResetPlatform()
    {
        // Restaura a plataforma ao estado original
        sr.enabled = true;
        col.enabled = true;
        sr.color = originalColor;

        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        rb.bodyType = RigidbodyType2D.Kinematic;

        isTriggered = false;
    }
}
