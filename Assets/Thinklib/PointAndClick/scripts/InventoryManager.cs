using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InteractionMode
{
    ClickToSelect,
    DragAndDrop
}

public enum PawnValueDisplayMode
{
    UpdateValueInInventory,
    ShowValueOnPawn
}


public class InventoryManager : MonoBehaviour
{
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

    [Header("Pawn Logic Settings")]
    public PawnValueDisplayMode pawnValueDisplayMode = PawnValueDisplayMode.UpdateValueInInventory;

    private GameObject activePawnObject;
    public bool isPawnActive { get; private set; } = false;
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

    public PathFollower GetActivePawnFollower()
    {
        if (activePawnObject != null)
        {
            return activePawnObject.GetComponent<PathFollower>();
        }
        return null;
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

    public void SelectItem(Item item)
    {
        if (isPawnActive)
        {
            Debug.Log("Cannot select a new item while a pawn is active.");
            return;
        }

        if (item.pathFollowerPrefab == null)
        {
            DeselectItem();
            selectedItem = item;
            Debug.Log($"Item '{item.name}' selected (no pawn).");
            return;
        }
        
        selectedItem = item;
        Debug.Log($"Placing item '{item.name}' on the graph.");

        if (GraphManager.instance != null && GraphManager.instance.nodes.Count > 0)
        {
            Node startNode = GraphManager.instance.nodes[0];
            activePawnObject = Instantiate(item.pathFollowerPrefab, startNode.position, Quaternion.identity);

            PathFollower follower = activePawnObject.GetComponent<PathFollower>();
            if (follower != null) { follower.Initialize(0, item.value); }
            
            SpriteRenderer pawnRenderer = activePawnObject.GetComponent<SpriteRenderer>();
            if (pawnRenderer != null && item.icon != null) { pawnRenderer.sprite = item.icon; }

            isPawnActive = true;
            
            if (pawnValueDisplayMode == PawnValueDisplayMode.ShowValueOnPawn)
            {
                RemoveItem(item);
            }
        }
    }

    public void DeselectItem()
    {
        if (isPawnActive) return; 
        
        if (activePawnObject != null) { Destroy(activePawnObject); activePawnObject = null; }
        
        selectedItem = null;
    }

    public void PawnDepleted()
    {
        if (pawnValueDisplayMode == PawnValueDisplayMode.UpdateValueInInventory && selectedItem != null)
        {
            RemoveItem(selectedItem);
        }

        isPawnActive = false;
        activePawnObject = null;
        selectedItem = null;
        Debug.Log("Pawn depleted. Inventory unlocked.");
    }

    public void StartItemDrag(Item item)
    {
        if (draggedItemIcon != null)
        {
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