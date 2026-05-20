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

[AddComponentMenu("Thinklib/Platformer/Collectibles/Collectible Item", -100)]
[RequireComponent(typeof(Collider2D))]
public class CollectibleItem : MonoBehaviour
{
    public CollectibleType type;
    public int value = 1;

    [Header("Feedbacks")]
    public AudioClip collectSound;
    public ParticleSystem collectEffect;
    public bool destroyOnCollect = true;

    private bool alreadyCollected = false;

    // Telemetry
    private const string MechanicName = "Platformer/Collectibles/CollectibleItem";
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(CollectibleItem),
            new Dictionary<string, object> {
                { "type", type.ToString() },
                { "value", value },
                { "hasSound", collectSound != null },
                { "hasEffect", collectEffect != null },
                { "destroyOnCollect", destroyOnCollect }
            }
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (alreadyCollected) return;

        try
        {
            if (other.CompareTag("Player"))
            {
                alreadyCollected = true;

                if (GameManager.Instance != null)
                    GameManager.Instance.AddCollectible(type, value);

                if (collectSound != null)
                    AudioSource.PlayClipAtPoint(collectSound, transform.position);

                if (collectEffect != null)
                {
                    ParticleSystem effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
                }

                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(CollectibleItem),
                        new Dictionary<string, object> {
                            { "type", type.ToString() },
                            { "value", value }
                        }
                    );
                }

                if (destroyOnCollect)
                    Destroy(gameObject);
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(CollectibleItem),
                new Dictionary<string, object> {
                    { "where", "OnTriggerEnter2D" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
