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
    if (graphManager == null || nodePrefab == null || edgePrefab == null)
    {
        Debug.LogError("VisualGraph: GraphManager, Node Prefab, ou Edge Prefab não foram atribuídos no Inspector!");
        return;
    }

    // Instantiate all nodes
    foreach (Node node in graphManager.nodes)
    {
        GameObject nodeObject = Instantiate(nodePrefab, node.position, Quaternion.identity, transform);
        nodeObject.name = "VisualNode_" + (graphManager.nodes.IndexOf(node));
        nodeObjectMap.Add(node, nodeObject);
    }

    // Instantiate all edges
    for (int i = 0; i < graphManager.nodes.Count; i++)
    {
        Node sourceNode = graphManager.nodes[i];
        foreach (Edge edge in sourceNode.edges)
        {
            if (edge.targetNodeIndex >= 0 && edge.targetNodeIndex < graphManager.nodes.Count)
            {
                Node targetNode = graphManager.nodes[edge.targetNodeIndex];
                GameObject edgeObject = Instantiate(edgePrefab, transform);
                edgeObject.name = "VisualEdge_" + i + "-" + edge.targetNodeIndex;

                LineRenderer lineRenderer = edgeObject.GetComponent<LineRenderer>();
                if (lineRenderer != null)
                {
                    // Set the two positions for the line renderer
                    lineRenderer.SetPosition(0, sourceNode.position);
                    lineRenderer.SetPosition(1, targetNode.position);
                }
                else
                {
                    Debug.LogError("VisualGraph: Edge Prefab não tem um LineRenderer!");
                }
            }
        }
    }
}
}