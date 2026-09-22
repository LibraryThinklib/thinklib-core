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

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Thinklib/Common/Environment/Reward Chest", -99)]
public class RewardChest : MonoBehaviour
{
    public enum OpenMode
    {
        OnTouch,
        OnKeyPress
    }

    [Header("Opening")]
    [Tooltip("OnTouch opens as soon as the player enters the trigger. OnKeyPress requires the player to press Open Key while inside the trigger.")]
    public OpenMode openMode = OpenMode.OnTouch;

    [Tooltip("Key used to open the chest when Open Mode is OnKeyPress.")]
    public KeyCode openKey = KeyCode.E;

    [Header("Reward")]
    [Tooltip("Type of reward granted through GameManager.AddCollectible.")]
    public CollectibleType rewardType = CollectibleType.Life;

    [Tooltip("Amount granted to GameManager.AddCollectible.")]
    public int rewardAmount = 1;

    [Header("Reward Animation")]
    [Tooltip("Where the reward visual spawns. If empty, uses this object's position.")]
    public Transform launchPoint;

    [Tooltip("Optional visual (e.g. a coin or heart) that pops up when the chest opens. If empty, no animation plays.")]
    public GameObject rewardVisualPrefab;

    [Tooltip("How high the reward visual rises while appearing.")]
    public float riseHeight = 1f;

    [Tooltip("Duration of the fade-in/rise.")]
    public float appearDuration = 0.4f;

    [Tooltip("How long the reward visual stays fully visible before fading out.")]
    public float holdDuration = 0.3f;

    [Tooltip("Duration of the fade-out.")]
    public float fadeOutDuration = 0.8f;

    [Header("Visuals")]
    [Tooltip("Optional sprite for the 'opened' chest, shown once it has been used.")]
    public Sprite openSprite;

    private const string MechanicName = "Common/Enviroment/RewardChest";

    private SpriteRenderer spriteRenderer;
    private bool isUsed = false;
    private bool _sentUsed = false;
    private bool playerInRange = false;
    private GameObject playerInRangeObject;

    private void Awake()
    {
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(RewardChest),
            new Dictionary<string, object>
            {
                { "openMode", openMode.ToString() },
                { "rewardType", rewardType.ToString() },
                { "rewardAmount", rewardAmount },
                { "hasOpenSprite", openSprite != null },
                { "hasRewardVisualPrefab", rewardVisualPrefab != null }
            });
    }

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isUsed || openMode != OpenMode.OnKeyPress || !playerInRange) return;

        if (Input.GetKeyDown(openKey))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed || !other.CompareTag("Player")) return;

        try
        {
            playerInRange = true;
            playerInRangeObject = other.gameObject;

            if (openMode == OpenMode.OnTouch)
            {
                OpenChest();
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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player") || playerInRangeObject != other.gameObject) return;

        playerInRange = false;
        playerInRangeObject = null;
    }

    private void OpenChest()
    {
        if (isUsed) return;

        try
        {
            isUsed = true;

            if (GameManager.Instance != null)
                GameManager.Instance.AddCollectible(rewardType, rewardAmount);

            MarkAsUsed();

            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(RewardChest),
                    new Dictionary<string, object>
                    {
                        { "action", "open" },
                        { "openMode", openMode.ToString() },
                        { "rewardType", rewardType.ToString() },
                        { "rewardAmount", rewardAmount }
                    });
            }

            PlayRewardVisual();
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track("mechanic_error", MechanicName, nameof(RewardChest),
                new Dictionary<string, object>
                {
                    { "where", "OpenChest" },
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

    private void PlayRewardVisual()
    {
        if (rewardVisualPrefab == null) return;

        Vector3 spawnPosition = launchPoint != null ? launchPoint.position : transform.position;
        GameObject visual = Instantiate(rewardVisualPrefab, spawnPosition, Quaternion.identity);

        StartCoroutine(AnimateRewardVisual(visual));
    }

    private IEnumerator AnimateRewardVisual(GameObject visual)
    {
        SpriteRenderer visualRenderer = visual.GetComponent<SpriteRenderer>();
        Vector3 startPosition = visual.transform.position;
        Vector3 risenPosition = startPosition + Vector3.up * riseHeight;

        float elapsed = 0f;
        while (elapsed < appearDuration)
        {
            float t = elapsed / appearDuration;
            visual.transform.position = Vector3.Lerp(startPosition, risenPosition, t);
            SetVisualAlpha(visualRenderer, Mathf.Lerp(0f, 1f, t));
            elapsed += Time.deltaTime;
            yield return null;
        }
        visual.transform.position = risenPosition;
        SetVisualAlpha(visualRenderer, 1f);

        yield return new WaitForSeconds(holdDuration);

        elapsed = 0f;
        while (elapsed < fadeOutDuration)
        {
            SetVisualAlpha(visualRenderer, Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }
        SetVisualAlpha(visualRenderer, 0f);

        Destroy(visual);
    }

    private void SetVisualAlpha(SpriteRenderer renderer, float alpha)
    {
        if (renderer == null) return;

        Color c = renderer.color;
        renderer.color = new Color(c.r, c.g, c.b, alpha);
    }
}
