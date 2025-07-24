// DropZone.cs - SIMPLIFIED VERSION
using UnityEngine;
// We no longer need UnityEngine.EventSystems
// We removed IDropHandler from the class declaration

public class DropZone : MonoBehaviour 
{
    [Header("Zone Configuration")]
    [Tooltip("The unique, ordered ID for this zone (0 for the first, 1 for the second, etc.)")]
    public int zoneID;

    [Header("Visuals")]
    [Tooltip("The SpriteRenderer of the CHILD OBJECT used to display the item placed here.")]
    [SerializeField] private SpriteRenderer displaySprite;

    [Header("State")]
    private Item storedItem = null;
    
    private void Start()
    {
        if (displaySprite != null)
        {
            displaySprite.enabled = false;
        }
    }
    
    public bool PlaceItem(Item itemToPlace)
    {
        if (storedItem != null || itemToPlace == null)
        {
            return false;
        }

        storedItem = itemToPlace;
        
        if (displaySprite != null)
        {
            displaySprite.sprite = storedItem.icon;
            displaySprite.enabled = true;
        }
        
        InventoryManager.instance.RemoveItem(itemToPlace);
        
        Debug.Log($"Item '{storedItem.name}' placed in Zone {zoneID}.");
        return true;
    }

    // This method for the click-to-select mode is still needed.
    private void OnMouseDown()
    {
        Item itemToPlace = InventoryManager.instance.selectedItem;
        if (itemToPlace != null)
        {
            PlaceItem(itemToPlace);
        }
    }
}