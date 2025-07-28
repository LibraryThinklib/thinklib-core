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

public enum PawnSpawnMode
{
    SpawnPawnFromItem,
    UseFixedPawnInScene
}

public enum PawnStartPositionMode
{
    AlwaysStartAtHomeNode,
    ContinueFromLastNode
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

    public PawnStartPositionMode startPositionMode = PawnStartPositionMode.AlwaysStartAtHomeNode;
    public PawnValueDisplayMode pawnValueDisplayMode = PawnValueDisplayMode.UpdateValueInInventory;

    private GameObject activePawnObject;
    public bool isPawnActive { get; private set; } = false;
    private bool isDragging = false;

    [Tooltip("Determines how the pawn is created and used in the scene.")]
    public PawnSpawnMode pawnMode = PawnSpawnMode.SpawnPawnFromItem;
    [Tooltip("Assign the single, fixed pawn from the scene here. Used only in 'UseFixedPawnInScene' mode.")]
    public PathFollower fixedPawn;


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
        if (!combinationsEnabled || itemA == null || itemB == null) return false;

        foreach (CombinationRecipe recipe in availableRecipes)
        {
            bool matches = (recipe.item1.name == itemA.name && recipe.item2.name == itemB.name) ||
                           (recipe.item1.name == itemB.name && recipe.item2.name == itemA.name);

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
                    itemInstance.pathFollowerPrefab = recipe.resultingItem.pathFollowerPrefab;
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
        if (isPawnActive) { Debug.Log("Cannot select a new item while a pawn is active."); return; }

        int startNodeIndex = 0;
        if (startPositionMode == PawnStartPositionMode.ContinueFromLastNode)
        {
            startNodeIndex = GraphManager.instance.lastKnownPawnNodeIndex;
        }

        if (pawnMode == PawnSpawnMode.SpawnPawnFromItem)
        {
            if (item.pathFollowerPrefab == null) { DeselectItem(); selectedItem = item; UpdateUI(); return; }
            selectedItem = item;

            if (GraphManager.instance != null && GraphManager.instance.nodes.Count > startNodeIndex)
            {
                Node startNode = GraphManager.instance.nodes[startNodeIndex];
                activePawnObject = Instantiate(item.pathFollowerPrefab, startNode.position, Quaternion.identity);
                PathFollower follower = activePawnObject.GetComponent<PathFollower>();
                if (follower != null) follower.Initialize(startNodeIndex, item.value);
                SpriteRenderer pawnRenderer = activePawnObject.GetComponent<SpriteRenderer>();
                if (pawnRenderer != null && item.icon != null) pawnRenderer.sprite = item.icon;
            }
        }
        else
        {
            if (fixedPawn == null) { Debug.LogError("Fixed Pawn mode is selected, but no pawn is assigned!"); return; }
            if (item.value <= 0) { Debug.Log($"Item '{item.name}' has no value and cannot be used as fuel."); return; }
            selectedItem = item;
            activePawnObject = fixedPawn.gameObject;
            activePawnObject.SetActive(true);
            fixedPawn.Initialize(startNodeIndex, item.value);
        }

        isPawnActive = true;

        if (pawnValueDisplayMode == PawnValueDisplayMode.ShowValueOnPawn)
        {
            RemoveItem(item);
        }
        UpdateUI();
    }

    public void DeselectItem() {
        if (isPawnActive) return;
        if (activePawnObject != null) { Destroy(activePawnObject);
            activePawnObject = null; } selectedItem = null;
        UpdateUI(); }


    public void PawnDepleted()
    {
        if (pawnValueDisplayMode == PawnValueDisplayMode.UpdateValueInInventory && selectedItem != null)
        {
            RemoveItem(selectedItem);
        }
        
        if (pawnMode == PawnSpawnMode.UseFixedPawnInScene && activePawnObject != null)
        {
            activePawnObject.SetActive(false);
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