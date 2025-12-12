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
    [Tooltip("Este item pode ser acumulado no inventário?")]
    public bool isStackable = false;
    [Tooltip("Qual a quantidade máxima deste item por slot?")]
    public int maxStackSize = 99;

    [Header("Timer Settings")]
    [Tooltip("Este item ativa um cronômetro quando colocado em uma DropZone?")]
    public bool hasTimer = false;
    [Tooltip("Tempo em segundos que o item pode ficar na DropZone antes de sumir.")]
    public float itemLifetime = 10.0f;
}