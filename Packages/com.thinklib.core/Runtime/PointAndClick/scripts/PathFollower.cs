using UnityEngine;
using TMPro;

public class PathFollower : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5.0f;

    [Header("Rotation Settings")]
    [Tooltip("Should the pawn rotate to face its movement direction?")]
    public bool shouldRotateToDirection = true;
    [Tooltip("How quickly the pawn turns to face the new direction.")]
    public float rotationSpeed = 10f;

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
        isBlocked = false;
        isMoving = false;
    }

    public void MoveToNode(int targetNodeIndex)
    {
        if (isBlocked || isMoving) return;
        if (GraphManager.instance == null || GraphManager.instance.nodes.Count <= currentNodeIndex) return;
        
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
        if (valueText != null) { valueText.text = currentValue.ToString(); }
    }

    void Update()
    {
        if (isMoving && targetNode != null)
        {
            if (shouldRotateToDirection)
            {
                Vector3 direction = targetNode.position - transform.position;
                if (direction != Vector3.zero)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 0f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }
            
            transform.position = Vector3.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetNode.position) < 0.001f)
            {
                isMoving = false;
                currentNodeIndex = GraphManager.instance.nodes.IndexOf(targetNode);
                targetNode = null;
                Node arrivedNode = GraphManager.instance.nodes[currentNodeIndex];
                if (arrivedNode.isFinalNode)
                {
                    arrivedNode.isFinalNode = false;
                    GraphManager.instance.lastKnownPawnNodeIndex = currentNodeIndex;
                    InventoryManager.instance.PawnDepleted();
                    if (GraphManager.instance.onFinalNodeReached != null) { GraphManager.instance.onFinalNodeReached.Invoke(); }
                    if (this.gameObject != InventoryManager.instance.fixedPawn?.gameObject) { Destroy(this.gameObject); }
                    return; 
                }
                if (currentValue <= 0)
                {
                    GraphManager.instance.lastKnownPawnNodeIndex = currentNodeIndex;
                    InventoryManager.instance.PawnDepleted();
                    if (this.gameObject != InventoryManager.instance.fixedPawn?.gameObject) { Destroy(this.gameObject); }
                }
            }
        }
    }
}