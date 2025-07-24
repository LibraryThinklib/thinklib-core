// PathFollower.cs
using UnityEngine;
using TMPro; // Add this to control TextMeshPro

public class PathFollower : MonoBehaviour
{
    public float speed = 5.0f;
    
    [Header("Runtime State")]
    public int currentNodeIndex = 0;
    public int currentValue;

    [Header("Visuals")]
    [SerializeField] private TextMeshPro valueText; // Reference to the text component on the pawn

    private Node targetNode = null;
    private bool isMoving = false;
    private bool isBlocked = false;

    public void Initialize(int startNodeIndex, int initialValue)
    {
        if (GraphManager.instance == null) return;
        
        currentNodeIndex = startNodeIndex;
        currentValue = initialValue;
        
        transform.position = GraphManager.instance.nodes[currentNodeIndex].position;
        
        // Update the text when initialized
        UpdateValueText();
    }

    public void MoveToNode(int targetNodeIndex)
    {
        if (isBlocked || isMoving) return;

        Node startNode = GraphManager.instance.nodes[currentNodeIndex];
        
        Edge edgeToTarget = null;
        foreach (Edge edge in startNode.edges)
        {
            if (edge.targetNodeIndex == targetNodeIndex) { edgeToTarget = edge; break; }
        }

        if (edgeToTarget != null)
        {
            int moveCost = (int)edgeToTarget.weight;

            if (moveCost > currentValue)
            {
                isBlocked = true;
                Debug.LogError($"GAME OVER: Move cost ({moveCost}) > current value ({currentValue}).");
                // Optionally hide text on game over
                if (valueText != null) valueText.gameObject.SetActive(false);
                return;
            }
            
            currentValue -= moveCost;
            UpdateValueText(); // Update the text after value changes
            
            targetNode = GraphManager.instance.nodes[targetNodeIndex];
            isMoving = true;
        }
    }
    
    // NEW Helper Method
    void UpdateValueText()
    {
        // Only try to update text if the component is assigned
        if (valueText != null)
        {
            valueText.text = currentValue.ToString();
        }
    }

    void Update()
    {
        if (isMoving && targetNode != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime);

            // When the pawn arrives at the destination...
            if (Vector3.Distance(transform.position, targetNode.position) < 0.001f)
            {
                // Update its current node index.
                currentNodeIndex = GraphManager.instance.nodes.IndexOf(targetNode);
                isMoving = false;
                targetNode = null;
                Debug.Log($"Arrived at node {currentNodeIndex}.");

                // --- NEW LOGIC: Check if the destination is a final node ---
                Node arrivedNode = GraphManager.instance.nodes[currentNodeIndex];
                if (arrivedNode.isFinalNode)
                {
                    Debug.Log($"<color=lime>SUCCESS! Reached final node {currentNodeIndex}.</color>");
                    
                    // Trigger the global success event from the GraphManager.
                    if (GraphManager.instance.onFinalNodeReached != null)
                    {
                        GraphManager.instance.onFinalNodeReached.Invoke();
                    }
                    
                    // The puzzle is solved, so we can clean up.
                    InventoryManager.instance.PawnDepleted(); // Unlock inventory
                    Destroy(this.gameObject); // Destroy the pawn
                    return; // Stop further checks in this frame.
                }
                // --- END OF NEW LOGIC ---

                // Check if the item's value is depleted (only if it wasn't a final node).
                if (currentValue <= 0)
                {
                    Debug.Log("Item value depleted. Destroying pawn and unlocking inventory.");
                    InventoryManager.instance.PawnDepleted();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}