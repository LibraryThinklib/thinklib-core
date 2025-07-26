using UnityEngine;
using TMPro;

public class PathFollower : MonoBehaviour
{
    public float speed = 5.0f;
    
    [Header("Runtime State")]
    public int currentNodeIndex = 0;
    public int currentValue;

    [Header("Visuals")]
    [SerializeField] private TextMeshPro valueText;

    private Node targetNode = null;
    private bool isMoving = false;
    private bool isBlocked = false;

    public void Initialize(int startNodeIndex, int initialValue)
    {
        if (GraphManager.instance == null) return;
        
        currentNodeIndex = startNodeIndex;
        currentValue = initialValue;
        
        transform.position = GraphManager.instance.nodes[currentNodeIndex].position;
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
                if (valueText != null) valueText.gameObject.SetActive(false);
                return;
            }
            
            currentValue -= moveCost;
            UpdateValueText();
            
            targetNode = GraphManager.instance.nodes[targetNodeIndex];
            isMoving = true;
        }
    }
    
    void UpdateValueText()
    {
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

            if (Vector3.Distance(transform.position, targetNode.position) < 0.001f)
            {
                currentNodeIndex = GraphManager.instance.nodes.IndexOf(targetNode);
                isMoving = false;
                targetNode = null;
                Debug.Log($"Arrived at node {currentNodeIndex}.");

                Node arrivedNode = GraphManager.instance.nodes[currentNodeIndex];
                
                if (arrivedNode.isFinalNode)
                {
                    Debug.Log($"<color=lime>SUCCESS! Reached final node {currentNodeIndex}.</color>");
                    
                    // Deactivate this node as a final node for subsequent pawns
                    arrivedNode.isFinalNode = false;
                    Debug.Log($"Node {currentNodeIndex} is no longer a final node.");

                    if (GraphManager.instance.onFinalNodeReached != null)
                    {
                        GraphManager.instance.onFinalNodeReached.Invoke();
                    }
                    
                    InventoryManager.instance.PawnDepleted();
                    Destroy(this.gameObject);
                    return; 
                }

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