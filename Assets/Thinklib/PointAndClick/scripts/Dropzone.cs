using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Zone Configuration")]
    [Tooltip("The unique, ordered ID for this zone (0 for the first, 1 for the second, etc.)")]
    public int zoneID;

    [Header("Visuals")]
    [Tooltip("The SpriteRenderer of the CHILD OBJECT used to display the item placed here.")]
    [SerializeField] private SpriteRenderer displaySprite;

    private Item storedItem = null;

    // Helper methods for the manager to check the state of this zone
    public bool HasItem() 
    { 
        return storedItem != null; 
    }
    
    public Item GetStoredItem() 
    { 
        return storedItem; 
    }

    private void Start()
    {
        // Ensure the item display is hidden at the beginning
        if (displaySprite != null) 
        { 
            displaySprite.enabled = false; 
        }
    }
    
    // This is the main logic function to place an item in the zone
    public void PlaceItem(Item itemToPlace)
    {
        // Ask the manager for permission.
        if (itemToPlace == null || !DropZoneManager.instance.IsValidPlacement(itemToPlace, this.zoneID))
        {
            Debug.Log("Placement denied by DropZoneManager.");
            return;
        }

        // If permission is granted, proceed.
        storedItem = itemToPlace;
        
        if (displaySprite != null)
        {
            displaySprite.sprite = storedItem.icon;
            displaySprite.enabled = true;
        }
        
        // Remove the item from the player's inventory
        InventoryManager.instance.RemoveItem(itemToPlace);
        
        // --- THIS IS THE FIX ---
        // Manually end the drag operation to hide the mouse icon immediately.
        InventoryManager.instance.EndItemDrag();
        
        // If this was a drag-and-drop action, set the success flag
        if (ItemSlot.draggedItem != null)
        {
            ItemSlot.dragWasSuccessful = true;
        }
        
        Debug.Log($"Item '{storedItem.name}' placed in Zone {zoneID}.");

        // Tell the manager to check if the puzzle is complete.
        DropZoneManager.instance.CheckForPuzzleCompletion();
    }

    // --- Interaction Handlers ---

    // This handles the CLICK-TO-SELECT interaction mode
    private void OnMouseDown()
    {
        // Case 1: The player has an item selected from the inventory, trying to PLACE it
        if (InventoryManager.instance.selectedItem != null)
        {
            PlaceItem(InventoryManager.instance.selectedItem);
        }
        // Case 2: The player has NO item selected and this zone is full, trying to RETURN it
        else if (HasItem())
        {
            ReturnItemToInventory();
        }
    }

    // This handles the DRAG-AND-DROP interaction mode
    public void OnDrop(PointerEventData eventData) 
    { 
        PlaceItem(ItemSlot.draggedItem); 
    }

    // --- Private Helper Methods ---

    // New method to handle returning the item to the inventory
    private void ReturnItemToInventory()
    {
        // Ask the manager for permission to return the item from this specific zone
        if (DropZoneManager.instance.CanReturnItem(this.zoneID))
        {
            Debug.Log($"Returning item '{storedItem.name}' from Zone {zoneID} to inventory.");
            InventoryManager.instance.AddItem(storedItem);
            ClearZone();
        }
        else
        {
            Debug.Log($"Cannot return item from Zone {zoneID}. It is not the last item in the sequence.");
        }
    }

    // New helper method to clear the zone's state and visuals
    private void ClearZone()
    {
        storedItem = null;
        if (displaySprite != null)
        {
            displaySprite.sprite = null;
            displaySprite.enabled = false;
        }
    }
}