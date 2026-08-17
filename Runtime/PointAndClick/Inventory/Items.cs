// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;

[CreateAssetMenu(menuName = "Thinklib/Point and Click/Inventory/Item", order = -98)]
public class Item : ScriptableObject
{
    [Header("Item Information")]
    public int value;
    [TextArea(3, 5)]
    public string description;

    [Header("Game World Representation")]
    [Tooltip("If this item can be placed on the graph, assign its 3D prefab here.")]
    public GameObject pathFollowerPrefab;

    [Header("Inventory Visuals")]
    public Sprite icon;

    [Header("Stacking")]
    [Tooltip("Can this item be stacked in the inventory?")]
    public bool isStackable = false;
    [Tooltip("What is the maximum quantity of this item per slot?")]
    public int maxStackSize = 99;

    [Header("Timer Settings")]
    [Tooltip("Does this item start a timer when placed in a DropZone?")]
    public bool hasTimer = false;
    [Tooltip("Time in seconds the item can stay in the DropZone before disappearing.")]
    public float itemLifetime = 10.0f;
}