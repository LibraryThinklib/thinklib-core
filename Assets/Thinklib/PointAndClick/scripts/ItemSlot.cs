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
    
    public static Item draggedItem;
    
    // MODIFIED Update method in ItemSlot.cs
    void Update()
    {
        if (item == null || slotBackground == null) return;

        // --- Visual Feedback for Selection (same as before) ---
        if (InventoryManager.instance.selectedItem == this.item)
        {
            slotBackground.color = selectedColor;
        }
        else
        {
            slotBackground.color = normalColor;
        }

        // --- NEW LOGIC: Live value update for the active pawn ---
        // Check if the chosen mechanic is "UpdateValueInInventory"
        if (InventoryManager.instance.pawnValueDisplayMode == PawnValueDisplayMode.UpdateValueInInventory)
        {
            // Check if this slot represents the item whose pawn is currently active
            if (InventoryManager.instance.isPawnActive && InventoryManager.instance.selectedItem == this.item)
            {
                // Get the active pawn
                PathFollower activePawn = InventoryManager.instance.GetActivePawnFollower();
                if (activePawn != null)
                {
                    // Update the text with the pawn's CURRENT value in real-time
                    valueText.text = activePawn.currentValue.ToString();
                    valueText.gameObject.SetActive(true);
                    return; // Skip the default value display logic
                }
            }
        }
        
        // Default value display logic (for all other cases)
        if (item.value > 0)
        {
            valueText.text = item.value.ToString();
            valueText.gameObject.SetActive(true);
        }
        else
        {
            valueText.gameObject.SetActive(false);
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
            valueText.gameObject.SetActive(true);
            valueText.text = item.value.ToString();
        }
        else
        {
            valueText.gameObject.SetActive(false);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode != InteractionMode.DragAndDrop || item == null) return;
        
        draggedItem = item;
        InventoryManager.instance.StartItemDrag(item);
        iconImage.enabled = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode != InteractionMode.DragAndDrop) return;

        InventoryManager.instance.EndItemDrag();

        // THIS IS THE FIX: Declare the variable before using it.
        bool dropSuccessful = false;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            DropZone dropZone = hit.collider.GetComponent<DropZone>();
            if (dropZone != null)
            {
                dropSuccessful = dropZone.PlaceItem(draggedItem);
            }
        }
        
        if (!dropSuccessful)
        {
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