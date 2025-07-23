// Item.cs
using UnityEngine;

// The annotation below allows creating instances of this object in the Unity menu
// Assets > Create > Point and Click > Item
[CreateAssetMenu(fileName = "NewItem", menuName = "Point and Click/Item")]
public class Item : ScriptableObject
{
    [Header("Item Information")]
    public string name;
    public int value; // NEW: Field for the item's value
    [TextArea(3, 5)] // To give the description field more space in the Inspector
    public string description;

    [Header("Inventory Visuals")]
    public Sprite icon;
}