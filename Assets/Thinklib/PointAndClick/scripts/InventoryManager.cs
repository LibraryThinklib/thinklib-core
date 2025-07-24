using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    
    private GameObject activePawnObject;
    private bool isPawnActive = false; // The inventory lock flag
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
                
                RemoveItem(itemA);
                RemoveItem(itemB);

                if (recipe.sumIngredientValues)
                {
                    Item itemInstance = ScriptableObject.CreateInstance<Item>();
                    itemInstance.name = recipe.resultingItem.name;
                    itemInstance.description = recipe.resultingItem.description;
                    itemInstance.icon = recipe.resultingItem.icon;
                    itemInstance.value = itemA.value + itemB.value;
                    AddItem(itemInstance);
                }
                else
                {
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
            // Do not call DeselectItem here as it can cause issues with the pawn lock
            if (selectedItem == item)
            {
                selectedItem = null;
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
                slotScript.Initialize(item); 
            }
        }
    }

    // MODIFIED: SelectItem handles the new rules and inventory lock.
    public void SelectItem(Item item)
    {
        // If a pawn is already active in the world, block selecting new items.
        if (isPawnActive)
        {
            Debug.Log("Cannot select a new item while a pawn is active on the graph.");
            return;
        }

        // If the item doesn't have a pawn, it's a regular selection.
        if (item.pathFollowerPrefab == null)
        {
            DeselectItem(); // Deselect previous regular item if any
            selectedItem = item;
            Debug.Log($"Item '{item.name}' selected (no pawn).");
            return;
        }
        
        // --- Logic for placing a pawn on the graph ---
        
        selectedItem = item;
        Debug.Log($"Placing item '{item.name}' on the graph.");

        if (GraphManager.instance != null && GraphManager.instance.nodes.Count > 0)
        {
            // Spawn the pawn.
            Node startNode = GraphManager.instance.nodes[0];
            activePawnObject = Instantiate(item.pathFollowerPrefab, startNode.position, Quaternion.identity);

            // Setup the pawn with its starting values.
            PathFollower follower = activePawnObject.GetComponent<PathFollower>();
            if (follower != null)
            {
                follower.Initialize(0, item.value);
            }
            
            // Setup the pawn's sprite.
            SpriteRenderer pawnRenderer = activePawnObject.GetComponent<SpriteRenderer>();
            if (pawnRenderer != null && item.icon != null)
            {
                pawnRenderer.sprite = item.icon;
            }

            // Lock the inventory and remove the item now that it's in play.
            isPawnActive = true;
            RemoveItem(item);
        }
    }

    // MODIFIED: DeselectItem now also considers the inventory lock.
    public void DeselectItem()
    {
        // Don't do anything if a pawn is active. The lock is only released via PawnDepleted().
        if (isPawnActive) return; 
        
        if (activePawnObject != null)
        {
            Destroy(activePawnObject);
            activePawnObject = null;
        }
        
        selectedItem = null;
        Debug.Log("No item selected.");
    }
    
    // NEW: A public method that the PathFollower can call when its value is depleted.
    public void PawnDepleted()
    {
        isPawnActive = false; // Unlock the inventory.
        activePawnObject = null;
        selectedItem = null; // Clear the selected item reference.
        Debug.Log("Pawn depleted. Inventory unlocked.");
    }

    public void StartItemDrag(Item item)
    {
        if (draggedItemIcon != null)
        {
            // We don't call DeselectItem() here anymore to avoid conflicts with the pawn lock
            selectedItem = null; 
            isDragging = true;
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