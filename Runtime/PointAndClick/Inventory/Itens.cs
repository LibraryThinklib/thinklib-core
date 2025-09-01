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
}
