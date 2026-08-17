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

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Thinklib/Common/Environment/Reward Chest", -99)]
public class RewardChest : MonoBehaviour
{
    [Header("Reward Configuration")]
    [Tooltip("Amount of health to heal.")]
    public int healthToGive = 1;

    [Header("Visuals")]
    [Tooltip("Optional sprite for the 'opened' chest, shown once it has been used.")]
    public Sprite openSprite;

    private const string MechanicName = "Common/Enviroment/RewardChest";

    private SpriteRenderer spriteRenderer;
    private bool isUsed = false;
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(RewardChest),
            new Dictionary<string, object>
            {
                { "healthToGive", healthToGive },
                { "hasOpenSprite", openSprite != null }
            });
    }

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed || !other.CompareTag("Player")) return;

        try
        {
            LifeSystemController lifeSystem = other.GetComponent<LifeSystemController>();

            if (lifeSystem != null)
            {
                lifeSystem.Heal(healthToGive);
                Debug.Log($"Jogador curou {healthToGive} de vida!");

                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(RewardChest),
                        new Dictionary<string, object>
                        {
                            { "action", "heal" },
                            { "healthToGive", healthToGive }
                        });
                }

                MarkAsUsed();
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track("mechanic_error", MechanicName, nameof(RewardChest),
                new Dictionary<string, object>
                {
                    { "where", "OnTriggerEnter2D" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                });
            throw;
        }
    }

    private void MarkAsUsed()
    {
        isUsed = true;
        GetComponent<Collider2D>().enabled = false;

        if (spriteRenderer != null && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
    }
}
