using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// NEW: Defining the possible interaction modes.
// Placing this outside the class allows other scripts to access it easily.
public enum InteractionMode
{
    ClickToSelect,
    DragAndDrop
}

public class InventoryManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static InventoryManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }
    // --- End of Singleton ---

    [Header("Interaction Settings")]
    // NEW: Public variable to choose the mode in the Unity Inspector.
    public InteractionMode currentMode = InteractionMode.ClickToSelect;


    [Header("Initial Setup")]
    public List<Item> initialItems;

    [Header("UI Setup")]
    public Transform itemsPanel;
    public GameObject itemSlotPrefab;

    [Header("Item Drag and Drop Logic")]
    public Canvas mainCanvas;
    public Image draggedItemIcon;

    [Header("Inventory Status")]
    private List<Item> inventory = new List<Item>();
    public Item selectedItem { get; private set; }
    
    private bool isDragging = false;

    void Start()
    {
        if (draggedItemIcon != null)
        {
            draggedItemIcon.gameObject.SetActive(false);
        }

        foreach (Item item in initialItems)
        {
            AddItem(item);
        }
    }

    void Update()
    {
        if (isDragging && draggedItemIcon != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform,
                Input.mousePosition,
                mainCanvas.worldCamera,
                out Vector2 localPoint);
            
            draggedItemIcon.rectTransform.localPosition = localPoint;
        }
    }
    
    public void AddItem(Item item)
    {
        if (item != null && !inventory.Contains(item))
        {
            inventory.Add(item);
            UpdateUI();
        }
    }

    public void RemoveItem(Item item)
    {
        if (item != null && inventory.Contains(item))
        {
            if (selectedItem == item)
            {
                DeselectItem();
            }
            inventory.Remove(item);
            UpdateUI();
        }
    }
    
    private void UpdateUI()
    {
        foreach (Transform child in itemsPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Item item in inventory)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemsPanel);
            var slotScript = slotObj.GetComponent<ItemSlot>();
            if (slotScript != null)
            {
                // Assuming ItemSlot has an "Initialize" method.
                slotScript.Initialize(item); 
            }
        }
    }

    public void SelectItem(Item item)
    {
        selectedItem = item;
        // Assuming your 'Item' class has a 'name' property.
        Debug.Log($"Item selected: {item.name}"); 
    }

    public void DeselectItem()
    {
        selectedItem = null;
        Debug.Log("No item selected.");
    }

    public void StartItemDrag(Item item)
    {
        if (draggedItemIcon != null)
        {
            DeselectItem();
            isDragging = true;
            // Assuming your 'Item' class has an 'icon' property.
            draggedItemIcon.sprite = item.icon; 
            draggedItemIcon.gameObject.SetActive(true);
        }
    }

    public void EndItemDrag()
    {
        if (draggedItemIcon != null)
        {
            isDragging = false;
            draggedItemIcon.gameObject.SetActive(false);
        }
    }
}