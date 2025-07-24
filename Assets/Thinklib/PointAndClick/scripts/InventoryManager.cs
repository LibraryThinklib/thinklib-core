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
                    Debug.Log($"New value for {itemInstance.name} is {itemInstance.value}");
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
                slotScript.Initialize(item); 
            }
        }
    }

    public void SelectItem(Item item)
    {
        // First, deselect any previous item and destroy its pawn.
        DeselectItem();

        selectedItem = item;
        Debug.Log($"Item selected: {item.name}");

        // If the selected item has an associated pawn prefab...
        if (item.pathFollowerPrefab != null)
        {
            // ...and we have a graph...
            if (GraphManager.instance != null && GraphManager.instance.nodes.Count > 0)
            {
                // ...spawn the pawn at the starting node's position (e.g., node 0).
                Node startNode = GraphManager.instance.nodes[0];
                activePawnObject = Instantiate(item.pathFollowerPrefab, startNode.position, Quaternion.identity);

                // --- THIS IS THE NEW LOGIC ---
                // Get the SpriteRenderer component of the instantiated pawn.
                SpriteRenderer pawnRenderer = activePawnObject.GetComponent<SpriteRenderer>();

                // Check if the pawn has a SpriteRenderer and the item has an icon.
                if (pawnRenderer != null && item.icon != null)
                {
                    // Set the sprite of the pawn to be the icon of the selected item.
                    pawnRenderer.sprite = item.icon;
                }
                else if (pawnRenderer == null)
                {
                    Debug.LogWarning($"The PathFollower prefab '{item.pathFollowerPrefab.name}' does not have a SpriteRenderer component!");
                }
                // --- END OF NEW LOGIC ---

                // Initialize the pawn's state.
                PathFollower follower = activePawnObject.GetComponent<PathFollower>();
                if (follower != null)
                {
                    follower.SetCurrentNode(0); // Set its starting node index
                }
            }
        }
    }

    public void DeselectItem()
    {
        // If there is an active pawn in the world, destroy it.
        if (activePawnObject != null)
        {
            Destroy(activePawnObject);
            activePawnObject = null;
        }
        
        selectedItem = null;
        Debug.Log("No item selected.");
    }

    public void StartItemDrag(Item item)
    {
        if (draggedItemIcon != null)
        {
            DeselectItem();
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