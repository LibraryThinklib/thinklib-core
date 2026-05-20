// Copyright (c) 2026 Thinkned Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Platformer/Collectibles/Collectibles Manager", -99)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Coins")]
    public int coins = 0;
    public Text coinsText;

    // Telemetry
    private const string MechanicName = "Platformer/Collectibles/Manager";
    private bool _sentUsed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(GameManager),
                new Dictionary<string, object> {
                    { "startCoins", coins },
                    { "hasCoinsText", coinsText != null }
                }
            );
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddCollectible(CollectibleType type, int value)
    {
        try
        {
            switch (type)
            {
                case CollectibleType.Coin:
                    coins += value;
                    UpdateUI();
                    break;

                case CollectibleType.Life:
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        LifeSystemController lifeSystem = player.GetComponent<LifeSystemController>();
                        if (lifeSystem != null)
                            lifeSystem.Heal(value);
                    }
                    break;
            }

            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(GameManager),
                    new Dictionary<string, object> {
                        { "type", type.ToString() },
                        { "value", value },
                        { "coinsAfter", coins }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(GameManager),
                new Dictionary<string, object> {
                    { "where", "AddCollectible" },
                    { "type", type.ToString() },
                    { "value", value },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void UpdateUI()
    {
        if (coinsText != null)
            coinsText.text = "" + coins;
    }
}
