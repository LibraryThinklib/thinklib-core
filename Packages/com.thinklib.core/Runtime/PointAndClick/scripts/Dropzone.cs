using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Zone Configuration")]
    [Tooltip("The unique, ordered ID for this zone (0 for the first, 1 for the second, etc.)")]
    public int zoneID;

    [Header("Visuals")]
    [Tooltip("The SpriteRenderer of the CHILD OBJECT used to display the item placed here.")]
    [SerializeField] private SpriteRenderer displaySprite;

    private Item storedItem = null;

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
        if (displaySprite != null)
        {
            displaySprite.enabled = false;
        }
    }

    public void PlaceItem(Item newItem)
    {
        if (newItem == null || HasItem())
        {
            return;
        }

        storedItem = newItem;
        if (displaySprite != null) { displaySprite.sprite = storedItem.icon; displaySprite.enabled = true; }
        
        InventoryManager.instance.RemoveItem(newItem);
        if (ItemSlot.draggedItem == newItem) { ItemSlot.dragWasSuccessful = true; }
        InventoryManager.instance.EndItemDrag();
        
        Debug.Log($"Item '{storedItem.name}' placed in Zone {zoneID}.");

        DropZoneManager.instance.CheckForPuzzleCompletion();
    }

    private IEnumerator RejectItem(Item itemToReturn, float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);
        
        if (storedItem == itemToReturn)
        {
            InventoryManager.instance.AddItem(itemToReturn);
            ClearZone();
        }
    }

    private void OnMouseDown()
    {
        if (InventoryManager.instance.selectedItem != null)
        {
            PlaceItem(InventoryManager.instance.selectedItem);
        }
        else if (HasItem())
        {
            ReturnItemToInventory();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        PlaceItem(ItemSlot.draggedItem);
    }

    private void ReturnItemToInventory()
    {
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