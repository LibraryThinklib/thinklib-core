// ItemSlot.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [Header("Item Data")]
    private Item item;

    [Header("UI Components")]
    [SerializeField]
    private Image iconImage;
    
    // NEW: Reference to the slot's background image
    [SerializeField]
    private Image slotBackground;

    [Header("Visual Selection Feedback")]
    // NEW: Colors we can customize in the Inspector
    [SerializeField]
    private Color normalColor = Color.white;
    [SerializeField]
    private Color selectedColor = new Color(0.8f, 0.8f, 0.8f, 1f); // A light gray

    public static Item itemBeingDragged;
    
    // NEW: Update method to check the selection state
    void Update()
    {
        // Ensures we only run the logic if we have an item and a background defined
        if (item == null || slotBackground == null) return;
        
        // Checks if the item in this slot is the same as the one selected in the InventoryManager
        if (InventoryManager.instance.selectedItem == this.item)
        {
            // If so, applies the selected color
            slotBackground.color = selectedColor;
        }
        else
        {
            // If not, applies the normal color
            slotBackground.color = normalColor;
        }
    }

    public void Initialize(Item newItem)
    {
        item = newItem;
        if (item != null)
        {
            // Assuming your 'Item' class has an 'icon' property
            iconImage.sprite = item.icon; 
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Using the translated enum and property from the previous script
        if (InventoryManager.instance.currentMode == InteractionMode.ClickToSelect)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (itemBeingDragged == null)
                {
                    // If the item is already selected, clicking it again deselects it
                    if(InventoryManager.instance.selectedItem == item)
                    {
                        InventoryManager.instance.DeselectItem();
                    }
                    else
                    {
                        InventoryManager.instance.SelectItem(item);
                    }
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode == InteractionMode.DragAndDrop)
        {
            if (item != null)
            {
                itemBeingDragged = item;
                InventoryManager.instance.StartItemDrag(item);
                iconImage.enabled = false;
            }
        }
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // No changes needed here
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode == InteractionMode.DragAndDrop)
        {
            InventoryManager.instance.EndItemDrag();
            // Assuming your 'Item' class has a 'name' property
            Debug.Log($"Finished dragging item {item.name} at position {eventData.position}");
            itemBeingDragged = null;
            iconImage.enabled = true;
        }
    }
}