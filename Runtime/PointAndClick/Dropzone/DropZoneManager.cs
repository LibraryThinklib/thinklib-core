using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

[AddComponentMenu("Thinklib/Point and Click/Dropzone/DropZoneManager", -99)]
public class DropZoneManager : MonoBehaviour
{
    public static DropZoneManager instance;
    void Awake() { if (instance != null && instance != this) { Destroy(this.gameObject); return; } instance = this; }

    public enum PuzzleOrderRule { AscendingValue, DescendingValue, Custom }

    [Header("Puzzle Configuration")]
    public PuzzleOrderRule puzzleRule;
    public List<Item> customSolution;

    [Header("Game Events")]
    public UnityEvent onPuzzleCompleted;

    private List<DropZone> orderedZones;

    void Start()
    {
        orderedZones = new List<DropZone>(FindObjectsOfType<DropZone>());
        orderedZones = orderedZones.OrderBy(zone => zone.zoneID).ToList();
    }

    public void CheckForPuzzleCompletion()
    {
        if (!orderedZones.All(z => z.HasItem()))
        {
            return;
        }

        if (IsSolutionCorrect())
        {
            Debug.Log("<color=lime>PUZZLE COMPLETED! The solution is correct.</color>");
            if (onPuzzleCompleted != null)
            {
                onPuzzleCompleted.Invoke();
            }
        }
        else
        {
            Debug.Log("<color=orange>Puzzle is full, but the solution is incorrect. Try again!</color>");
        }
    }

    private bool IsSolutionCorrect()
    {
        switch (puzzleRule)
        {
            case PuzzleOrderRule.AscendingValue:
                for (int i = 1; i < orderedZones.Count; i++)
                {
                    if (orderedZones[i].GetStoredItem().value <= orderedZones[i - 1].GetStoredItem().value)
                        return false;
                }
                return true;

            case PuzzleOrderRule.DescendingValue:
                for (int i = 1; i < orderedZones.Count; i++)
                {
                    if (orderedZones[i].GetStoredItem().value >= orderedZones[i - 1].GetStoredItem().value)
                        return false;
                }
                return true;

            case PuzzleOrderRule.Custom:
                for (int i = 0; i < orderedZones.Count; i++)
                {
                    if (i >= customSolution.Count || orderedZones[i].GetStoredItem() != customSolution[i])
                        return false;
                }
                return true;
        }
        return false;
    }

    public bool CanReturnItem(int zoneID)
    {
        return true;
    }
}