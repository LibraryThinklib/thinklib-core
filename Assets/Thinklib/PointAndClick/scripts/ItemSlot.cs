using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // NEW: Add this line to use TextMeshPro

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [Header("Item Data")]
    private Item item;

    [Header("UI Components")]
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private Image slotBackground;
    [SerializeField]
    private TextMeshProUGUI valueText;

    [Header("Visual Selection Feedback")]
    [SerializeField]
    private Color normalColor = Color.white;
    [SerializeField]
    private Color selectedColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    // CORRECT: Only one static variable for the dragged item.
    public static Item draggedItem;
    
    void Update()
    {
        if (item == null || slotBackground == null) return;
        
        if (InventoryManager.instance.selectedItem == this.item)
        {
            slotBackground.color = selectedColor;
        }
        else
        {
            slotBackground.color = normalColor;
        }
    }

    public void Initialize(Item newItem)
    {
        item = newItem;
        if (item != null)
        {
            iconImage.sprite = item.icon; 
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        if (item != null && item.value > 0)
        {
            valueText.gameObject.SetActive(true); // Show the text object
            valueText.text = item.value.ToString(); // Set the text to the item's value
        }
        else
        {
            valueText.gameObject.SetActive(false); // Hide the text object if there's no item or value
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode != InteractionMode.ClickToSelect) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        Item currentlySelectedItem = InventoryManager.instance.selectedItem;

        if (currentlySelectedItem != null && currentlySelectedItem != this.item)
        {
            InventoryManager.instance.TryCombineItems(currentlySelectedItem, this.item);
        }
        else
        {
            if (currentlySelectedItem == this.item)
            {
                InventoryManager.instance.DeselectItem();
            }
            else
            {
                InventoryManager.instance.SelectItem(this.item);
            }
        }
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode != InteractionMode.DragAndDrop) return;
        
        // This will now correctly read from the variable set in OnBeginDrag
        Item itemToCombineWith = ItemSlot.draggedItem; 
        
        if(itemToCombineWith != null && itemToCombineWith != this.item)
        {
            InventoryManager.instance.TryCombineItems(itemToCombineWith, this.item);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode == InteractionMode.DragAndDrop)
        {
            if (item != null)
            {
                // CORRECTED: Use the single 'draggedItem' variable
                draggedItem = item; 
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
            Debug.Log($"Finished dragging item {item.name} at position {eventData.position}");
            
            // CORRECTED: Use the single 'draggedItem' variable
            draggedItem = null;
            iconImage.enabled = true;
        }
    }
}