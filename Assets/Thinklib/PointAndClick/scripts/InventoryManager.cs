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

    [Header("Combination Settings")]
    public bool combinationsEnabled = true;
    public List<CombinationRecipe> availableRecipes;

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

    /// <summary>
    /// Attempts to combine two items based on the list of available recipes.
    /// </summary>
    /// <returns>True if the combination was successful.</returns>
    public bool TryCombineItems(Item itemA, Item itemB)
    {
        if (!combinationsEnabled) return false;

        foreach (CombinationRecipe recipe in availableRecipes)
        {
            bool matches = (recipe.item1 == itemA && recipe.item2 == itemB) ||
                           (recipe.item1 == itemB && recipe.item2 == itemA);

            if (matches)
            {
                Debug.Log($"Combination successful! Created {recipe.resultingItem.name}");
                
                // First, remove the ingredients from the inventory
                RemoveItem(itemA);
                RemoveItem(itemB);

                // --- NEW LOGIC FOR ADDING THE RESULTING ITEM ---

                // Check if we should sum the values for this specific recipe
                if (recipe.sumIngredientValues)
                {
                    // Create a new temporary instance of the resulting item in memory
                    Item itemInstance = ScriptableObject.CreateInstance<Item>();
                    
                    // Copy all properties from the recipe's template to our new instance
                    itemInstance.name = recipe.resultingItem.name; // Keep the same name
                    itemInstance.description = recipe.resultingItem.description;
                    itemInstance.icon = recipe.resultingItem.icon;
                    
                    // Calculate and set the new combined value
                    itemInstance.value = itemA.value + itemB.value;
                    
                    // Add the new INSTANCE to the inventory
                    AddItem(itemInstance);
                    Debug.Log($"New value for {itemInstance.name} is {itemInstance.value}");
                }
                else
                {
                    // If not summing, just add the predefined item from the recipe (old behavior)
                    AddItem(recipe.resultingItem);
                }

                DeselectItem();
                EndItemDrag();

                return true;
            }
        }

        Debug.Log("These items cannot be combined.");
        return false;
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