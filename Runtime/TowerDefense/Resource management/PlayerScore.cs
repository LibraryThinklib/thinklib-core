// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/TowerDefense/Resource Management/Player Score", -100)]
public class PlayerScore : MonoBehaviour
{
    public int currentScore = 0;
    public Text scoreText;

    // Telemetry
    private const string MechanicName = "TowerDefense/ResourceManagement/PlayerScore";
    private bool _sentInstantiated = false;
    private bool _sentUsed = false;

    private void Awake()
    {
        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(PlayerScore),
                new Dictionary<string, object>
                {
                    { "initialScore", currentScore },
                    { "hasScoreText", scoreText != null }
                }
            );
        }

        SafeUpdateScoreUI();
    }

    public void AddScore(int points)
    {
        try
        {
            currentScore += points;

            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlayerScore),
                    new Dictionary<string, object>
                    {
                        { "action", "add_score_first_time" },
                        { "added", points },
                        { "newScore", currentScore }
                    }
                );
            }
            else
            {
                // (Optional) If you want to log every increment:
                // ThinklibTelemetry.Track(
                //     "mechanic_used",
                //     MechanicName,
                //     nameof(PlayerScore),
                //     new Dictionary<string, object>
                //     {
                //         { "action", "add_score" },
                //         { "added", points },
                //         { "newScore", currentScore }
                //     }
                // );
            }

            SafeUpdateScoreUI();
        }
        catch (System.Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerScore),
                new Dictionary<string, object>
                {
                    { "where", "AddScore" },
                    { "points", points },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void SafeUpdateScoreUI()
    {
        try
        {
            if (scoreText != null)
            {
                scoreText.text = "Points: " + currentScore.ToString();
            }
            else
            {
                // If you want to log a missing UI in real usage (optional)
                // ThinklibTelemetry.Track(
                //     "mechanic_error",
                //     MechanicName,
                //     nameof(PlayerScore),
                //     new Dictionary<string, object>
                //     {
                //         { "where", "SafeUpdateScoreUI" },
                //         { "message", "scoreText is null" }
                //     }
                // );
            }
        }
        catch (System.Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerScore),
                new Dictionary<string, object>
                {
                    { "where", "SafeUpdateScoreUI" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
