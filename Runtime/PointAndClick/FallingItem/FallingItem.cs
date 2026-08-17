// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingItem : MonoBehaviour
{
    [Header("Movement Configuration")]
    public float fallSpeed = 5.0f;

    [Header("References")]
    [SerializeField] private SpriteRenderer itemSpriteRenderer;

    // Holds the item data (ScriptableObject)
    public Item itemData { get; private set; }
    
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Keeps the Rigidbody unaffected by default gravity (we control velocity manually)
        // and prevents physical collision with other items
        rb.isKinematic = true;

        // Keeps the Collider as a 'trigger', for detection only, not physical collision
        GetComponent<Collider2D>().isTrigger = true;

        if (itemSpriteRenderer == null)
        {
            itemSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Called by ItemSpawner to configure this item
    public void Initialize(Item item)
    {
        this.itemData = item;

        if (itemSpriteRenderer != null && item.icon != null)
        {
            itemSpriteRenderer.sprite = item.icon;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    // Requires a "DespawnZone"-tagged collider below the screen to destroy items that fall off
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DespawnZone"))
        {
            Destroy(gameObject);
        }
    }
}