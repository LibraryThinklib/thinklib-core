// PathFollower.cs
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    public float speed = 5.0f;
    public int currentNodeIndex = 0; // The index of the node where the pawn currently is.

    private Node targetNode = null;
    private bool isMoving = false;

    // Call this to set the initial position without moving.
    public void SetCurrentNode(int nodeIndex)
    {
        if (GraphManager.instance == null) return;
        currentNodeIndex = nodeIndex;
        transform.position = GraphManager.instance.nodes[currentNodeIndex].position;
        isMoving = false;
        targetNode = null;
    }

    // Call this to command the pawn to move to a new node.
    public void MoveToNode(int targetNodeIndex)
    {
        if (isMoving) return; // Don't accept new commands while already moving.

        // Get the data for the current and target nodes.
        Node startNode = GraphManager.instance.nodes[currentNodeIndex];
        Node destinationNode = GraphManager.instance.nodes[targetNodeIndex];

        // Check if there is a valid edge from the current node to the target node.
        bool pathExists = false;
        foreach (Edge edge in startNode.edges)
        {
            if (edge.targetNodeIndex == targetNodeIndex)
            {
                pathExists = true;
                break;
            }
        }

        if (pathExists)
        {
            Debug.Log($"Valid path found! Moving from node {currentNodeIndex} to {targetNodeIndex}");
            targetNode = destinationNode;
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
            // Move our position a step closer to the target.
            transform.position = Vector3.MoveTowards(transform.position, targetNode.position, speed * Time.deltaTime);

            // Check if the position of the pawn and target node are approximately equal.
            if (Vector3.Distance(transform.position, targetNode.position) < 0.001f)
            {
                // We've reached the destination.
                currentNodeIndex = GraphManager.instance.nodes.IndexOf(targetNode);
                isMoving = false;
                targetNode = null;
                Debug.Log($"Arrived at node {currentNodeIndex}");
            }
        }
    }
}