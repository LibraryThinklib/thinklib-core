// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;

[CreateAssetMenu(menuName = "Thinklib/Point and Click/Inventory/CombinationRecipe", order = -100)]
public class CombinationRecipe : ScriptableObject
{
    [Header("Recipe Ingredients")]
    public Item item1;
    public Item item2;

    [Header("Result")]
    public Item resultingItem;

    [Header("Value Calculation")]
    [Tooltip("If checked, the resulting item's value will be the sum of the ingredients' values. If unchecked, it will use the value from the 'Resulting Item' asset.")]
    public bool sumIngredientValues = false;
}
