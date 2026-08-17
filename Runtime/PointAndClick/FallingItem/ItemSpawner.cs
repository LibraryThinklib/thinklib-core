// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Thinklib/Game/ItemSpawner")]
public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [Tooltip("The prefab for the object that 'falls'. Must have the FallingItem script and a Collider2D.")]
    public GameObject fallingItemPrefab;

    [Tooltip("The list of possible items (ScriptableObjects) that can be dropped.")]
    public List<Item> itemsToSpawn;

    [Tooltip("Interval, in seconds, between each dropped item.")]
    public float spawnInterval = 2.0f;

    [Tooltip("Width of the spawn area. The item will appear at a random X position within this width.")]
    public float spawnAreaWidth = 10f;

    [Tooltip("Initial delay before items start dropping.")]
    public float initialDelay = 1.0f;

    void Start()
    {
        if (fallingItemPrefab == null)
        {
            Debug.LogError("The 'fallingItemPrefab' prefab was not assigned on ItemSpawner!");
            return;
        }

        if (itemsToSpawn.Count == 0)
        {
            Debug.LogError("The 'itemsToSpawn' list is empty on ItemSpawner!");
            return;
        }
        
        InvokeRepeating("SpawnItem", initialDelay, spawnInterval);
    }

    void SpawnItem()
    {
        Item itemToSpawn = itemsToSpawn[Random.Range(0, itemsToSpawn.Count)];

        float randomX = Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2);
        Vector3 spawnPosition = transform.position + new Vector3(randomX, 0, 0);

        GameObject itemObject = Instantiate(fallingItemPrefab, spawnPosition, Quaternion.identity);

        FallingItem fallingItem = itemObject.GetComponent<FallingItem>();
        if (fallingItem != null)
        {
            fallingItem.Initialize(itemToSpawn);
        }
        else
        {
            Debug.LogError($"The prefab {fallingItemPrefab.name} does not contain the FallingItem.cs script!");
            Destroy(itemObject); // Destroy it if it's misconfigured
        }
    }

    // Visual aid for seeing the spawn area in the Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 leftPoint = transform.position - new Vector3(spawnAreaWidth / 2, 0, 0);
        Vector3 rightPoint = transform.position + new Vector3(spawnAreaWidth / 2, 0, 0);
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawSphere(leftPoint, 0.2f);
        Gizmos.DrawSphere(rightPoint, 0.2f);
    }
}