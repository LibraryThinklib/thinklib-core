using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class DropZoneManager : MonoBehaviour
{
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
        orderedZones = new List<DropZone>(FindObjectsOfType<DropZone>());
        orderedZones = orderedZones.OrderBy(zone => zone.zoneID).ToList();
    }

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

    public bool CanReturnItem(int zoneID)
    {
        DropZone lastFilledZone = orderedZones.LastOrDefault(z => z.HasItem());

        if (lastFilledZone == null)
        {
            return false;
        }

        return lastFilledZone.zoneID == zoneID;
    }

    public void CheckForPuzzleCompletion()
    {
        if (orderedZones.All(z => z.HasItem()))
        {
            Debug.Log("<color=lime>PUZZLE COMPLETED!</color>");
            if (onPuzzleCompleted != null)
            {
                onPuzzleCompleted.Invoke();
            }
        }
    }

    private bool IsValidAscending(Item item, int zoneID)
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
            if (item.value <= previousItem.value)
            {
                Debug.Log($"Invalid placement: Item value {item.value} is not greater than previous value {previousItem.value}.");
                return false;
            }
        }
        return true;
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
        if (zoneID < customSolution.Count && customSolution[zoneID] == item)
        {
            return true;
        }
        Debug.Log($"Invalid placement: Item '{item?.name}' is not the correct item for zone {zoneID}.");
        return false;
    }
}