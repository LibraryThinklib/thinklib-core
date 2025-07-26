using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Required for OrderBy and other list operations
using UnityEngine.Events;

public class DropZoneManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static DropZoneManager instance;
    void Awake()
    {
        if (instance != null && instance != this) 
        { 
            Destroy(this.gameObject); 
            return; 
        }
        instance = this;
    }

    // The different puzzle rules the developer can choose from.
    public enum PuzzleOrderRule
    {
        AscendingValue,
        DescendingValue,
        Custom
    }

    [Header("Puzzle Configuration")]
    [Tooltip("The rule that determines the correct order of items in the drop zones.")]
    public PuzzleOrderRule puzzleRule;
    [Tooltip("For CUSTOM mode only. The list must match the number of Drop Zones. Element 0 is the solution for Zone ID 0, etc.")]
    public List<Item> customSolution;
    
    [Header("Game Events")]
    [Tooltip("This event is triggered when all drop zones are filled correctly.")]
    public UnityEvent onPuzzleCompleted;

    private List<DropZone> orderedZones;

    void Start()
    {
        // Find all drop zones in the scene and order them by their ID to ensure they are in the correct sequence.
        orderedZones = new List<DropZone>(FindObjectsOfType<DropZone>());
        orderedZones = orderedZones.OrderBy(zone => zone.zoneID).ToList();
    }

    // The DropZone will call this method to ask for permission to place an item.
    public bool IsValidPlacement(Item item, int zoneID)
    {
        switch (puzzleRule)
        {
            case PuzzleOrderRule.AscendingValue:
                return IsValidAscending(item, zoneID);
                
            case PuzzleOrderRule.DescendingValue:
                return IsValidDescending(item, zoneID);
                
            case PuzzleOrderRule.Custom:
                return IsValidCustom(item, zoneID);
        }
        return false;
    }

    // New method to check if an item can be returned from a specific zone.
    public bool CanReturnItem(int zoneID)
    {
        // Find the zone with the highest ID that currently has an item.
        DropZone lastFilledZone = orderedZones.LastOrDefault(z => z.HasItem());

        // If no zones are filled, we can't return anything.
        if (lastFilledZone == null)
        {
            return false;
        }

        // We can only return an item from the last filled zone in the sequence.
        return lastFilledZone.zoneID == zoneID;
    }
    
    // Called by the DropZone after a successful placement.
    public void CheckForPuzzleCompletion()
    {
        // Check if all zones are now filled.
        if (orderedZones.All(z => z.HasItem()))
        {
            Debug.Log("<color=lime>PUZZLE COMPLETED!</color>");
            if (onPuzzleCompleted != null)
            {
                onPuzzleCompleted.Invoke();
            }
        }
    }

    // --- Private Validation Methods ---

    private bool IsValidAscending(Item item, int zoneID)
    {
        // Player must fill zones in order (0, 1, 2...). We find the first one that is empty.
        DropZone firstEmptyZone = orderedZones.FirstOrDefault(z => !z.HasItem());
        if (firstEmptyZone == null || firstEmptyZone.zoneID != zoneID)
        {
            Debug.Log("Invalid placement: must fill zones in order.");
            return false;
        }

        // If it's not the very first zone (ID 0), compare its value with the previous zone's item.
        if (zoneID > 0)
        {
            Item previousItem = orderedZones[zoneID - 1].GetStoredItem();
            if (item.value <= previousItem.value)
            {
                Debug.Log($"Invalid placement: Item value {item.value} is not greater than previous value {previousItem.value}.");
                return false;
            }
        }
        return true; // The first item placement is always valid.
    }

    private bool IsValidDescending(Item item, int zoneID)
    {
        DropZone firstEmptyZone = orderedZones.FirstOrDefault(z => !z.HasItem());
        if (firstEmptyZone == null || firstEmptyZone.zoneID != zoneID)
        {
            Debug.Log("Invalid placement: must fill zones in order.");
            return false;
        }

        if (zoneID > 0)
        {
            Item previousItem = orderedZones[zoneID - 1].GetStoredItem();
            if (item.value >= previousItem.value)
            {
                Debug.Log($"Invalid placement: Item value {item.value} is not less than previous value {previousItem.value}.");
                return false;
            }
        }
        return true;
    }

    private bool IsValidCustom(Item item, int zoneID)
    {
        // Check if the provided item matches the solution for this specific zone ID.
        if (zoneID < customSolution.Count && customSolution[zoneID] == item)
        {
            return true;
        }
        Debug.Log($"Invalid placement: Item '{item?.name}' is not the correct item for zone {zoneID}.");
        return false;
    }
}