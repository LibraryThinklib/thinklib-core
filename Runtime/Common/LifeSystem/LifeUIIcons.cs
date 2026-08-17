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
using UnityEngine.UI;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Common/LifeSystem/Life UI Icons", -88)]
public class LifeUIIcons : MonoBehaviour
{
    [Header("Icon Configuration")]
    [SerializeField] private Image originalIcon; // Image already present in the hierarchy
    [SerializeField] private Sprite activeIcon;
    [SerializeField] private Sprite inactiveIcon;

    private List<Image> iconList = new List<Image>();

    private const string MechanicName = "Common/LifeSystem/LifeUIIcons";
    private bool _sentUsedUpdate = false;
    private bool _sentUsedShake = false;

    private void Awake()
    {
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(LifeUIIcons),
            new Dictionary<string, object> {
                { "hasOriginalIcon", originalIcon != null },
                { "hasActiveSprite",  activeIcon  != null },
                { "hasInactiveSprite", inactiveIcon != null }
            }
        );
    }

    public void UpdateIcons(int currentHealth, int maxHealth)
    {
        try
        {
            if (originalIcon == null)
            {
                Debug.LogWarning("No original icon assigned.");
                return;
            }

            if (iconList.Count != maxHealth)
            {
                GenerateIcons(maxHealth);
            }

            for (int i = 0; i < iconList.Count; i++)
            {
                iconList[i].sprite = i < currentHealth ? activeIcon : inactiveIcon;
                iconList[i].enabled = i < maxHealth;
            }

            // mechanic_used: first update
            if (!_sentUsedUpdate)
            {
                _sentUsedUpdate = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(LifeUIIcons),
                    new Dictionary<string, object> {
                        { "action", "update_icons" },
                        { "currentHealth", currentHealth },
                        { "maxHealth", maxHealth }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(LifeUIIcons),
                new Dictionary<string, object> {
                    { "where", "UpdateIcons" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void GenerateIcons(int amount)
    {
        try
        {
            // Hide the original icon
            originalIcon.gameObject.SetActive(false);

            // Remove all old icons (except the original)
            foreach (Transform child in transform)
            {
                if (child != originalIcon.transform)
                    Destroy(child.gameObject);
            }

            iconList.Clear();

            for (int i = 0; i < amount; i++)
            {
                Image newIcon = Instantiate(originalIcon, transform);
                newIcon.gameObject.SetActive(true);
                iconList.Add(newIcon);
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(LifeUIIcons),
                new Dictionary<string, object> {
                    { "where", "GenerateIcons" },
                    { "amount", amount },
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
                    nameof(LifeUIIcons),
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
                nameof(LifeUIIcons),
                new Dictionary<string, object> {
                    { "where", "Shake" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    // Coroutine: no try/catch (only try/finally). Risky operations go into yield-free helpers.
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
            target.localPosition = originalPos;
        }
    }

    // Yield-free helper -> can have try/catch + telemetry
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
                nameof(LifeUIIcons),
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
