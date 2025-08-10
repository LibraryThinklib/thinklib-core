using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class VisualGraph : MonoBehaviour
{
    public GraphManager graphManager;
    public GameObject nodePrefab;
    public GameObject edgePrefab;

    [Header("Display Options")]
    [Tooltip("If checked, the weight of each edge will be displayed in-game.")]
    public bool showEdgeWeights = true;

    private Dictionary<Node, GameObject> nodeObjectMap = new Dictionary<Node, GameObject>();

    void Start()
    {
        if (graphManager == null || nodePrefab == null || edgePrefab == null)
        {
            Debug.LogError("VisualGraph: GraphManager, Node Prefab, or Edge Prefab not assigned in Inspector!", this);
            return;
        }

        foreach (Node node in graphManager.nodes)
        {
            if (node != null && !nodeObjectMap.ContainsKey(node))
            {
                GameObject nodeObject = Instantiate(nodePrefab, node.position, Quaternion.identity, transform);
                int index = graphManager.nodes.IndexOf(node);
                nodeObject.name = "VisualNode_" + index;
                nodeObjectMap.Add(node, nodeObject);

                VisualNodeClickHandler clickHandler = nodeObject.AddComponent<VisualNodeClickHandler>();
                clickHandler.nodeIndex = index;
            }
            else
            {
                Debug.LogWarning($"Duplicate or null node found in GraphManager's list. Skipping.", this);
            }
        }

        for (int i = 0; i < graphManager.nodes.Count; i++)
        {
            Node sourceNode = graphManager.nodes[i];
            
            if (sourceNode != null && nodeObjectMap.ContainsKey(sourceNode))
            {
                foreach (Edge edge in sourceNode.edges)
                {
                    if (edge.targetNodeIndex >= 0 && edge.targetNodeIndex < graphManager.nodes.Count)
                    {
                        Node targetNode = graphManager.nodes[edge.targetNodeIndex];
                        
                        if (targetNode != null && nodeObjectMap.ContainsKey(targetNode))
                        {
                            GameObject edgeObject = Instantiate(edgePrefab, transform);
                            edgeObject.name = "VisualEdge_" + i + "-" + edge.targetNodeIndex;

                            LineRenderer lineRenderer = edgeObject.GetComponent<LineRenderer>();
                            if (lineRenderer != null)
                            {
                                lineRenderer.SetPosition(0, sourceNode.position);
                                lineRenderer.SetPosition(1, targetNode.position);
                            }
                            
                            TextMeshPro textComponent = edgeObject.GetComponentInChildren<TextMeshPro>();

                            if (textComponent != null)
                            {
                                if (showEdgeWeights)
                                {
                                    Vector3 midpoint = (sourceNode.position + targetNode.position) / 2f;
                                    textComponent.transform.position = midpoint;
                                    
                                    textComponent.text = edge.weight.ToString();
                                }
                                else
                                {
                                    textComponent.gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}