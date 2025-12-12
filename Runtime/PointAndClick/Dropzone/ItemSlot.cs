using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[AddComponentMenu("Thinklib/Point and Click/Dropzone/ItemSlot", -98)]
public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    [Header("Item Data")]
    private Item item;
    private InventorySlot mySlotRef;

    [Header("UI Components")]
    [SerializeField] private Image iconImage;
    [SerializeField] private Image slotBackground;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI quantityText;

    [Header("Visual Selection Feedback")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color timerColor = Color.red;

    public static Item draggedItem;
    public static bool dragWasSuccessful;

    public void Initialize(InventorySlot slot)
    {
        mySlotRef = slot;
        item = slot.item;

        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }

        UpdateInfoText();
    }

    void Update()
    {
        if (InventoryManager.instance == null) return;
        
        if (mySlotRef != null && item != null && item.hasTimer)
        {
            if (quantityText != null)
            {
                quantityText.gameObject.SetActive(true);
                quantityText.color = timerColor;
                quantityText.text = Mathf.CeilToInt(mySlotRef.currentLifeTime).ToString() + "s";
            }
        }

        if (item == null || slotBackground == null) return;

        slotBackground.color = (InventoryManager.instance.selectedItem == this.item)
            ? selectedColor : normalColor;

        if (InventoryManager.instance.pawnValueDisplayMode == PawnValueDisplayMode.UpdateValueInInventory)
        {
            if (InventoryManager.instance.isPawnActive && InventoryManager.instance.selectedItem == this.item)
            {
                PathFollower activePawn = InventoryManager.instance.GetActivePawnFollower();
                if (activePawn != null && valueText != null)
                {
                    valueText.text = activePawn.currentValue.ToString();
                    valueText.gameObject.SetActive(true);
                    return;
                }
            }
        }

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

    private void UpdateInfoText()
    {
        if (quantityText != null)
        {
            if (item != null && item.hasTimer)
            {
                quantityText.gameObject.SetActive(true);
                quantityText.color = timerColor;
            }
            else if (item != null && item.isStackable && mySlotRef.quantity > 1)
            {
                quantityText.text = mySlotRef.quantity.ToString();
                quantityText.color = Color.white;
                quantityText.gameObject.SetActive(true);
            }
            else
            {
                quantityText.gameObject.SetActive(false);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode != InteractionMode.DragAndDrop || item == null) return;
        draggedItem = item;
        dragWasSuccessful = false;
        InventoryManager.instance.StartItemDrag(item);
        iconImage.enabled = false;
        if (quantityText != null) quantityText.gameObject.SetActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (InventoryManager.instance.currentMode != InteractionMode.DragAndDrop) return;
        InventoryManager.instance.EndItemDrag();
        if (!dragWasSuccessful)
        {
            iconImage.enabled = true;
            UpdateInfoText();
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
            if (currentlySelectedItem == this.item) InventoryManager.instance.DeselectItem();
            else InventoryManager.instance.SelectItem(this.item);
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