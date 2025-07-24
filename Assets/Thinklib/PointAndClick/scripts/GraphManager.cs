// GraphManager.cs
using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    // NEW: Singleton pattern to create a static reference.
    public static GraphManager instance;

    void Awake()
    {
        // Check if an instance already exists.
        if (instance != null && instance != this)
        {
            // If so, destroy this new one and return.
            Destroy(this.gameObject);
            return;
        }
        // Otherwise, set the instance to this component.
        instance = this;
    }
    // --- End of Singleton ---

    // This list will hold all the nodes in our graph.
    public List<Node> nodes = new List<Node>();

    // This function draws debug visuals in the Scene view, even during Play mode.
    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        // Draw all nodes and edges
        for (int i = 0; i < nodes.Count; i++)
        {
            Node node = nodes[i];
            
            // Draw the node as a sphere
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(node.position, 0.3f);
            
            // Draw all edges originating from this node
            foreach (Edge edge in node.edges)
            {
                if (edge.targetNodeIndex >= 0 && edge.targetNodeIndex < nodes.Count)
                {
                    Node targetNode = nodes[edge.targetNodeIndex];

                    // Draw the line for the edge
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(node.position, targetNode.position);
                }
            }
        }
    }
}