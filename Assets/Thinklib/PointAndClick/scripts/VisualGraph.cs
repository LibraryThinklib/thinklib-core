using UnityEngine;
using System.Collections.Generic;

public class VisualGraph : MonoBehaviour
{
    public GraphManager graphManager;
    public GameObject nodePrefab;
    public GameObject edgePrefab;

    private Dictionary<Node, GameObject> nodeObjectMap = new Dictionary<Node, GameObject>();

    void Start()
    {
        // 1. Check if all required components are assigned in the Inspector.
        if (graphManager == null || nodePrefab == null || edgePrefab == null)
        {
            Debug.LogError("VisualGraph: GraphManager, Node Prefab, or Edge Prefab not assigned in Inspector!", this);
            return;
        }

        // 2. Instantiate all visual nodes.
        foreach (Node node in graphManager.nodes)
        {
            // Safeguard: Check if the node is null or already added to our map.
            if (node != null && !nodeObjectMap.ContainsKey(node))
            {
                // Create the visual object for the node.
                GameObject nodeObject = Instantiate(nodePrefab, node.position, Quaternion.identity, transform);
                int index = graphManager.nodes.IndexOf(node);
                nodeObject.name = "VisualNode_" + index;
                
                // Add the node and its visual object to our map for later reference.
                nodeObjectMap.Add(node, nodeObject);

                // Add the click handler component to make the node interactive.
                VisualNodeClickHandler clickHandler = nodeObject.AddComponent<VisualNodeClickHandler>();
                clickHandler.nodeIndex = index;
            }
            else
            {
                // If the node is a duplicate or null, log a warning and skip it.
                Debug.LogWarning("Duplicate or null node found in GraphManager's list. Skipping.", this);
            }
        }

        // 3. Instantiate all visual edges.
        for (int i = 0; i < graphManager.nodes.Count; i++)
        {
            Node sourceNode = graphManager.nodes[i];
            
            // Check if the source node is valid (i.e., it wasn't a skipped duplicate).
            if (sourceNode != null && nodeObjectMap.ContainsKey(sourceNode))
            {
                foreach (Edge edge in sourceNode.edges)
                {
                    // Check if the edge's target index is valid.
                    if (edge.targetNodeIndex >= 0 && edge.targetNodeIndex < graphManager.nodes.Count)
                    {
                        Node targetNode = graphManager.nodes[edge.targetNodeIndex];
                        
                        // Check if the target node is also valid.
                        if (targetNode != null && nodeObjectMap.ContainsKey(targetNode))
                        {
                            // Create the visual object for the edge.
                            GameObject edgeObject = Instantiate(edgePrefab, transform);
                            edgeObject.name = "VisualEdge_" + i + "-" + edge.targetNodeIndex;

                            LineRenderer lineRenderer = edgeObject.GetComponent<LineRenderer>();
                            if (lineRenderer != null)
                            {
                                // Set the line's start and end positions.
                                lineRenderer.SetPosition(0, sourceNode.position);
                                lineRenderer.SetPosition(1, targetNode.position);
                            }
                        }
                    }
                }
            }
        }
    }
}