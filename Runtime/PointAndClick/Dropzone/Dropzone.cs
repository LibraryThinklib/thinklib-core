using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[AddComponentMenu("Thinklib/Point and Click/Dropzone/DropZone", -100)]
public class DropZone : MonoBehaviour, IDropHandler
{
    [Header("Zone Configuration")]
    [Tooltip("The unique, ordered ID for this zone (0 for the first, 1 for the second, etc.)")]
    public int zoneID;

    [Header("Visuals")]
    [Tooltip("The SpriteRenderer of the CHILD OBJECT used to display the item placed here.")]
    [SerializeField] private SpriteRenderer displaySprite;

    private Item storedItem = null;

    private Coroutine itemTimerCoroutine = null;

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

        InventoryManager.instance.RemoveItem(newItem, 1); 

        if (ItemSlot.draggedItem == newItem) { ItemSlot.dragWasSuccessful = true; }
        InventoryManager.instance.EndItemDrag();

        Debug.Log($"Item '{storedItem.name}' placed in Zone {zoneID}.");

        if (storedItem.hasTimer)
        {
            StopTimer();
            itemTimerCoroutine = StartCoroutine(StartItemTimer(storedItem));
        }

        DropZoneManager.instance.CheckForPuzzleCompletion();
    }

    private IEnumerator StartItemTimer(Item itemToTrack)
    {
        Debug.Log($"Timer started for {itemToTrack.name} in Zone {zoneID}.");
        yield return new WaitForSeconds(itemToTrack.itemLifetime);

        if (storedItem == itemToTrack)
        {
            Debug.LogWarning($"Timer expired for {itemToTrack.name}! Removing from Zone {zoneID}.");
            ClearZone();
        }
    }

    public void StopTimer()
    {
        if (itemTimerCoroutine != null)
        {
            StopCoroutine(itemTimerCoroutine);
            itemTimerCoroutine = null;
            Debug.Log($"Timer stopped for item in Zone {zoneID}.");
        }
    }


    private IEnumerator RejectItem(Item itemToReturn, float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);

        if (storedItem == itemToReturn)
        {
            InventoryManager.instance.AddItem(itemToReturn, 1);
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
            
            InventoryManager.instance.AddItem(storedItem, 1);
            ClearZone();
        }
        else
        {
            Debug.Log($"Cannot return item from Zone {zoneID}. It is not the last item in the sequence.");
        }
    }

    private void ClearZone()
    {
        StopTimer(); 
        
        storedItem = null;
        if (displaySprite != null)
        {
            displaySprite.sprite = null;
            displaySprite.enabled = false;
        }
    }
}