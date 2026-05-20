// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Common/LifeSystem/Life UI Bar", -89)]
public class LifeUIBar : MonoBehaviour
{
    [SerializeField] private Image healthFill;

    [Header("Color Settings")]
    [SerializeField] private Color highHealthColor = new Color(0f, 1f, 0f);     // green
    [SerializeField] private Color mediumHealthColor = new Color(1f, 0.64f, 0f);  // orange
    [SerializeField] private Color lowHealthColor = new Color(1f, 0f, 0f);     // red

    // Telemetry
    private const string MechanicName = "Common/LifeSystem/LifeUIBar";
    private bool _sentUsedUpdate = false;
    private bool _sentUsedShake = false;

    private void Awake()
    {
        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(LifeUIBar),
            new Dictionary<string, object> {
                { "hasHealthFill", healthFill != null },
                { "highColor",   $"{highHealthColor.r:F2},{highHealthColor.g:F2},{highHealthColor.b:F2}" },
                { "mediumColor", $"{mediumHealthColor.r:F2},{mediumHealthColor.g:F2},{mediumHealthColor.b:F2}" },
                { "lowColor",    $"{lowHealthColor.r:F2},{lowHealthColor.g:F2},{lowHealthColor.b:F2}" }
            }
        );
    }

    public void UpdateBar(int currentHealth, int maxHealth)
    {
        try
        {
            if (healthFill != null && maxHealth > 0)
            {
                float percent = (float)currentHealth / maxHealth;
                healthFill.fillAmount = percent;

                // Change color based on remaining health
                if (percent >= 0.7f) healthFill.color = highHealthColor;
                else if (percent >= 0.3f) healthFill.color = mediumHealthColor;
                else healthFill.color = lowHealthColor;

                // mechanic_used: first time UI bar is updated
                if (!_sentUsedUpdate)
                {
                    _sentUsedUpdate = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(LifeUIBar),
                        new Dictionary<string, object> {
                            { "action", "update_bar" },
                            { "percent", percent }
                        }
                    );
                }
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(LifeUIBar),
                new Dictionary<string, object> {
                    { "where", "UpdateBar" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    public void Shake(float intensity)
    {
        try
        {
            StartCoroutine(ShakeUI(transform, intensity));

            if (!_sentUsedShake)
            {
                _sentUsedShake = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(LifeUIBar),
                    new Dictionary<string, object> {
                        { "action", "shake" },
                        { "intensity", intensity }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(LifeUIBar),
                new Dictionary<string, object> {
                    { "where", "Shake" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    // Em corrotinas: sem try/catch (apenas try/finally). Erros são telemetrados nos helpers sem yield.
    private IEnumerator ShakeUI(Transform target, float intensity)
    {
        Vector3 originalPos = target.localPosition;

        try
        {
            for (int i = 0; i < 5; i++)
            {
                ShakeStepSafe(target, originalPos, intensity);
                yield return new WaitForSeconds(0.02f);
            }
        }
        finally
        {
            // restaura posição mesmo se algo der errado
            target.localPosition = originalPos;
        }
    }

    // Helper sem yield → pode ter try/catch + telemetria
    private void ShakeStepSafe(Transform target, Vector3 basePos, float intensity)
    {
        try
        {
            target.localPosition = basePos + (Vector3)UnityEngine.Random.insideUnitCircle * intensity;
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(LifeUIBar),
                new Dictionary<string, object> {
                    { "where", "ShakeStepSafe" },
                    { "intensity", intensity },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
