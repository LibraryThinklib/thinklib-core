using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [Header("Item Data")]
    private Item item;
    
    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image slotBackground;
    [SerializeField] private TextMeshProUGUI valueText;

    [Header("Visual Selection Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    
    // Static variables to manage the drag-and-drop state across all slots
    public static Item draggedItem;
    public static bool dragWasSuccessful; // The "flag" to signal a successful drop
    
    // Update logic for selection color and live value display
    void Update()
    {
        if (item == null || slotBackground == null) return;
        
        // --- Visual Feedback for Selection ---
        if (InventoryManager.instance.selectedItem == this.item)
        {
            slotBackground.color = selectedColor;
        }
        else
        {
            slotBackground.color = normalColor;
        }

        // --- Live value update logic for the active pawn ---
        if (InventoryManager.instance.pawnValueDisplayMode == PawnValueDisplayMode.UpdateValueInInventory)
        {
            if (InventoryManager.instance.isPawnActive && InventoryManager.instance.selectedItem == this.item)
            {
                PathFollower activePawn = InventoryManager.instance.GetActivePawnFollower();
                if (activePawn != null)
                {
                    // Update the text with the pawn's CURRENT value in real-time
                    if (valueText != null)
                    {
                        valueText.text = activePawn.currentValue.ToString();
                        valueText.gameObject.SetActive(true);
                    }
                    return; // Skip the default value display logic
                }
            }
        }
        
        // Default value display logic (for all other cases)
        if (valueText != null)
        {
            if (item != null && item.value > 0)
            {
                valueText.text = item.value.ToString();
                valueText.gameObject.SetActive(true);
            }
            else
            {
                valueText.gameObject.SetActive(false);
            }
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
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode != InteractionMode.DragAndDrop || item == null) return;
        
        draggedItem = item;
        dragWasSuccessful = false; // Reset the flag at the start of every drag.
        
        InventoryManager.instance.StartItemDrag(item);
        iconImage.enabled = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode != InteractionMode.DragAndDrop) return;

        InventoryManager.instance.EndItemDrag();
        
        // Check the flag that a successful DropZone might have set.
        if (!dragWasSuccessful)
        {
            // If the drop was not successful, make the icon reappear.
            iconImage.enabled = true;
        }

        draggedItem = null;
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
        Item itemToCombineWith = ItemSlot.draggedItem;
        if (itemToCombineWith != null && itemToCombineWith != this.item)
        {
            InventoryManager.instance.TryCombineItems(itemToCombineWith, this.item);
        }
    }

    public void OnDrag(PointerEventData eventData) { }
}