using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[AddComponentMenu("Thinklib/Point and Click/Graph/GraphManager", -100)]
public class GraphManager : MonoBehaviour
{
    public static GraphManager instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    [Header("Game Events")]
    public UnityEvent onFinalNodeReached;

    [Header("Graph Data")]
    public List<Node> nodes = new List<Node>();

    [Header("Runtime State")]
    [Tooltip("The index of the node where the last pawn was. This is updated automatically.")]
    public int lastKnownPawnNodeIndex = 0;

    private void OnDrawGizmos()
    {
        if (nodes == null) return;

        for (int i = 0; i < nodes.Count; i++)
        {
            Node node = nodes[i];

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(node.position, 0.3f);

            foreach (Edge edge in node.edges)
            {
                if (edge.targetNodeIndex >= 0 && edge.targetNodeIndex < nodes.Count)
                {
                    Node targetNode = nodes[edge.targetNodeIndex];

                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(node.position, targetNode.position);
                }
            }
        }
    }
}