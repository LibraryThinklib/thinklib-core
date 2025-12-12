using System;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Common/Effects/Player Hurt Effect", -99)]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerHurtEffect : MonoBehaviour
{
    [Header("Invulnerability Time")]
    public float invulnerabilityDuration = 1.5f;
    public float blinkInterval = 0.1f;

    private SpriteRenderer spriteRenderer;
    private bool isInvulnerable = false;
    private float invulnerabilityTimer = 0f;
    private float blinkTimer = 0f;

    private int originalLayer;

    public bool IsInvulnerable => isInvulnerable;

    // Telemetry
    private const string MechanicName = "Common/Effects/PlayerHurtEffect";
    private bool _sentUsed = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalLayer = gameObject.layer; // store original layer

        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(PlayerHurtEffect),
            new Dictionary<string, object> {
                { "invulnerabilityDuration", invulnerabilityDuration },
                { "blinkInterval", blinkInterval },
                { "initialLayer", originalLayer }
            }
        );
    }

    private void Update()
    {
        if (!isInvulnerable) return;

        try
        {
            invulnerabilityTimer -= Time.deltaTime;
            blinkTimer -= Time.deltaTime;

            if (blinkTimer <= 0f)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                blinkTimer = blinkInterval;
            }

            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
                spriteRenderer.enabled = true;
                gameObject.layer = originalLayer; // restore original layer
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerHurtEffect),
                new Dictionary<string, object> {
                    { "where", "Update" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    public void TriggerInvulnerability()
    {
        try
        {
            isInvulnerable = true;
            invulnerabilityTimer = invulnerabilityDuration;
            blinkTimer = 0f;

            originalLayer = gameObject.layer;
            int invulnerableLayer = LayerMask.NameToLayer("PlayerInvulnerable");

            if (invulnerableLayer != -1)
            {
                gameObject.layer = invulnerableLayer;
            }
            else
            {
                Debug.LogWarning("⚠️ Layer 'PlayerInvulnerable' not found. Make sure it exists in the project.");
            }

            // mechanic_used (primeiro acionamento do efeito)
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlayerHurtEffect),
                    new Dictionary<string, object> {
                        { "invulnerabilityDuration", invulnerabilityDuration },
                        { "blinkInterval", blinkInterval },
                        { "invulnerableLayerFound", invulnerableLayer != -1 }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerHurtEffect),
                new Dictionary<string, object> {
                    { "where", "TriggerInvulnerability" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
