// CombinationRecipe.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewCombination", menuName = "Point and Click/Combination Recipe")]
public class CombinationRecipe : ScriptableObject
{
    [Header("Recipe Ingredients")]
    public Item item1;
    public Item item2;

    [Header("Result")]
    public Item resultingItem;

    // NEW: Optional setting for value calculation
    [Header("Value Calculation")]
    [Tooltip("If checked, the resulting item's value will be the sum of the ingredients' values. If unchecked, it will use the value from the 'Resulting Item' asset.")]
    public bool sumIngredientValues = false;
}