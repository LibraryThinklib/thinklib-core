// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemCollector : MonoBehaviour
{
    [Header("Movement Configuration")]
    [Tooltip("Should the dropzone follow the mouse?")]
    public bool followMouse = true;

    [Tooltip("Horizontal movement limits (world coordinates)")]
    public float minX = -8f;
    public float maxX = 8f;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        GetComponent<Collider2D>().isTrigger = true;
    }

    void Update()
    {
        if (followMouse)
        {
            MoveWithMouse();
        }
    }

    private void MoveWithMouse()
    {
        if (mainCamera == null) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        float clampedX = Mathf.Clamp(mouseWorldPos.x, minX, maxX);

        // Keeps the collector's original Y and Z untouched
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        FallingItem fallingItem = other.GetComponent<FallingItem>();

        if (fallingItem != null)
        {
            Item collectedItem = fallingItem.itemData;

            if (collectedItem != null)
            {
                InventoryManager.instance.AddItem(collectedItem, 1);

                Debug.Log($"Item collected: {collectedItem.name}");

                // (Optional) Add a collect sound here
                // AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            Destroy(other.gameObject);
        }
    }
}