using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public enum InteractionMode { ClickToSelect, DragAndDrop }
public enum PawnValueDisplayMode { UpdateValueInInventory, ShowValueOnPawn }
public enum PawnSpawnMode { SpawnPawnFromItem, UseFixedPawnInScene }
public enum PawnStartPositionMode { AlwaysStartAtHomeNode, ContinueFromLastNode }

[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int quantity;
    public float currentLifeTime;

    public InventorySlot(Item item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
        
        if (item.hasTimer)
        {
            this.currentLifeTime = item.itemLifetime;
        }
    }

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        quantity -= amount;
    }
}

[AddComponentMenu("Thinklib/Point and Click/Inventory/InventoryManager", -99)]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); return; }
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
    public List<InventorySlot> inventory = new List<InventorySlot>();
    public Item selectedItem { get; private set; }

    [Header("Pawn Logic Settings")]
    public PawnStartPositionMode startPositionMode = PawnStartPositionMode.AlwaysStartAtHomeNode;
    public PawnValueDisplayMode pawnValueDisplayMode = PawnValueDisplayMode.UpdateValueInInventory;
    private GameObject activePawnObject;
    public bool isPawnActive { get; private set; } = false;
    private bool isDragging = false;
    public PawnSpawnMode pawnMode = PawnSpawnMode.SpawnPawnFromItem;
    public PathFollower fixedPawn;

    void Start()
    {
        if (draggedItemIcon != null) draggedItemIcon.gameObject.SetActive(false);
        foreach (Item item in initialItems) AddItem(item, 1);
    }

    void Update()
    {
        if (isDragging && draggedItemIcon != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform, Input.mousePosition, mainCanvas.worldCamera, out Vector2 localPoint);
            draggedItemIcon.rectTransform.localPosition = localPoint;
        }

        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            InventorySlot slot = inventory[i];

            if (slot.item.hasTimer)
            {
                slot.currentLifeTime -= Time.deltaTime;

                if (slot.currentLifeTime <= 0)
                {
                    Debug.Log($"Tempo acabou para {slot.item.name}! Removendo do inventário.");
                    
                    if (selectedItem == slot.item) DeselectItem();
                    
                    inventory.RemoveAt(i);
                    UpdateUI();
            }
        }
    }
    }
    
    public PathFollower GetActivePawnFollower()
    {
        if (activePawnObject != null) return activePawnObject.GetComponent<PathFollower>();
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
                RemoveItem(itemA, 1);
                RemoveItem(itemB, 1);
                if (recipe.sumIngredientValues)
                {
                    Item itemInstance = ScriptableObject.CreateInstance<Item>();
                    itemInstance.name = recipe.resultingItem.name;
                    itemInstance.description = recipe.resultingItem.description;
                    itemInstance.icon = recipe.resultingItem.icon;
                    itemInstance.pathFollowerPrefab = recipe.resultingItem.pathFollowerPrefab;
                    itemInstance.value = itemA.value + itemB.value;
                    AddItem(itemInstance, 1);
                }
                else { AddItem(recipe.resultingItem, 1); }
                DeselectItem();
                EndItemDrag();
                return true;
            }
        }
        return false;
    }

    public void AddItem(Item item, int amount = 1)
    {
        if (item == null || amount <= 0) return;
        bool wasAdded = false;

        if (item.isStackable)
        {
            foreach (InventorySlot slot in inventory)
            {
                if (amount <= 0) break;
                if (slot.item == item && slot.quantity < item.maxStackSize)
                {
                    int amountCanAdd = item.maxStackSize - slot.quantity;
                    int amountToAdd = Mathf.Min(amount, amountCanAdd);
                    slot.AddQuantity(amountToAdd);
                    amount -= amountToAdd;
                    wasAdded = true;
                }
            }
            while (amount > 0)
            {
                int amountToAdd = Mathf.Min(amount, item.maxStackSize);
                inventory.Add(new InventorySlot(item, amountToAdd));
                amount -= amountToAdd;
                wasAdded = true;
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                inventory.Add(new InventorySlot(item, 1));
                wasAdded = true;
            }
        }

        if (wasAdded) UpdateUI();
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        if (item == null || amount <= 0) return;
        InventorySlot slotToRemoveFrom = GetInventorySlot(item);

        if (slotToRemoveFrom != null)
        {
            if (selectedItem == item && slotToRemoveFrom.quantity - amount <= 0)
            {
                if (GetTotalItemQuantity(item) - amount <= 0) selectedItem = null;
            }
            
            slotToRemoveFrom.RemoveQuantity(amount);
            if (slotToRemoveFrom.quantity <= 0) inventory.Remove(slotToRemoveFrom);
            UpdateUI();
        }
    }

    private InventorySlot GetInventorySlot(Item item) { return inventory.FirstOrDefault(slot => slot.item == item); }
    public int GetTotalItemQuantity(Item item)
    {
        int total = 0;
        foreach (InventorySlot slot in inventory) if (slot.item == item) total += slot.quantity;
        return total;
    }

    private void UpdateUI()
    {
        foreach (Transform child in itemsPanel) Destroy(child.gameObject);
        foreach (InventorySlot slot in inventory)
        {
            GameObject slotObj = Instantiate(itemSlotPrefab, itemsPanel);
            var slotScript = slotObj.GetComponent<ItemSlot>();
            if (slotScript != null) slotScript.Initialize(slot);
        }
    }

    public void SelectItem(Item item)
    {
        if (isPawnActive) return;
        int startNodeIndex = (startPositionMode == PawnStartPositionMode.ContinueFromLastNode) ? GraphManager.instance.lastKnownPawnNodeIndex : 0;

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
            if (fixedPawn == null || item.value <= 0) return;
            selectedItem = item;
            activePawnObject = fixedPawn.gameObject;
            activePawnObject.SetActive(true);
            fixedPawn.Initialize(startNodeIndex, item.value);
        }
        isPawnActive = true;
        if (pawnValueDisplayMode == PawnValueDisplayMode.ShowValueOnPawn) RemoveItem(item, 1);
        UpdateUI();
    }

    public void DeselectItem()
    {
        if (isPawnActive) return;
        if (activePawnObject != null) { Destroy(activePawnObject); activePawnObject = null; }
        selectedItem = null;
        UpdateUI();
    }

    public void PawnDepleted()
    {
        if (pawnValueDisplayMode == PawnValueDisplayMode.UpdateValueInInventory && selectedItem != null) RemoveItem(selectedItem, 1);
        if (pawnMode == PawnSpawnMode.UseFixedPawnInScene && activePawnObject != null) activePawnObject.SetActive(false);
        isPawnActive = false; activePawnObject = null; selectedItem = null;
    }

    public void StartItemDrag(Item item)
    {
        if (draggedItemIcon != null)
        {
            selectedItem = null; isDragging = true;
            draggedItemIcon.sprite = item.icon; draggedItemIcon.gameObject.SetActive(true);
        }
    }

    public void EndItemDrag()
    {
        if (draggedItemIcon != null) { isDragging = false; draggedItemIcon.gameObject.SetActive(false); }
    }
}