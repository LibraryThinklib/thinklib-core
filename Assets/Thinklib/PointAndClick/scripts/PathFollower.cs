// PathFollower.cs
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float speed = 5.0f;
    
    [Header("Runtime State")]
    public int currentNodeIndex = 0;
    public int currentValue; // The item's current value, which decreases.

    private Node targetNode = null;
    private bool isMoving = false;
    private bool isBlocked = false; // "Game Over" state flag.

    // This method is now called by the InventoryManager to setup the pawn.
    public void Initialize(int startNodeIndex, int initialValue)
    {
        if (GraphManager.instance == null) return;
        
        currentNodeIndex = startNodeIndex;
        currentValue = initialValue;
        
        // Place the pawn at the start node position.
        transform.position = GraphManager.instance.nodes[currentNodeIndex].position;
        Debug.Log($"Pawn initialized at node {currentNodeIndex} with value {currentValue}.");
    }

    // This method has the new core logic.
    public void MoveToNode(int targetNodeIndex)
    {
        // If pawn is blocked or already moving, do nothing.
        if (isBlocked || isMoving) return;

        Node startNode = GraphManager.instance.nodes[currentNodeIndex];
        
        // Find the edge connecting the current node to the target node.
        Edge edgeToTarget = null;
        foreach (Edge edge in startNode.edges)
        {
            if (edge.targetNodeIndex == targetNodeIndex)
            {
                edgeToTarget = edge;
                break;
            }
        }

        if (edgeToTarget != null) // If a path exists...
        {
            int moveCost = (int)edgeToTarget.weight; // Get the cost of the move.

            // RULE 4: Check if the move is too expensive.
            if (moveCost > currentValue)
            {
                isBlocked = true; // Block the pawn.
                Debug.LogError($"GAME OVER: Move cost ({moveCost}) is greater than current value ({currentValue}). Pawn is blocked.");
                return;
            }
            
            // RULE 1: Subtract the value BEFORE moving.
            currentValue -= moveCost;
            Debug.Log($"Pawn moving. Cost: {moveCost}. New value: {currentValue}.");
            
            // Set the target and start moving.
            targetNode = GraphManager.instance.nodes[targetNodeIndex];
            isMoving = true;
        }
        else
        {
            Debug.Log($"No direct path from node {currentNodeIndex} to {targetNodeIndex}!");
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

                // RULE 2: Check if the item's value is depleted.
                if (currentValue <= 0)
                {
                    Debug.Log("Item value depleted. Destroying pawn and unlocking inventory.");
                    // Notify the manager that the inventory can be unlocked.
                    InventoryManager.instance.PawnDepleted();
                    // Destroy this pawn object.
                    Destroy(this.gameObject);
                }
            }
        }
    }
}