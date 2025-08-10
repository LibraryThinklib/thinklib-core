// GraphManagerEditor.cs
using UnityEngine;
using UnityEditor; // We need this namespace for editor scripts

// This tells Unity that this script is a custom editor for the GraphManager component.
[CustomEditor(typeof(GraphManager))]
public class GraphManagerEditor : Editor
{
    private GraphManager graphManager;

    // This function is called whenever the scene view is redrawn.
    private void OnSceneGUI()
    {
        // Get the target object this editor is inspecting
        graphManager = (GraphManager)target;

        if (graphManager.nodes == null) return;

        // Draw all the edges first
        for (int i = 0; i < graphManager.nodes.Count; i++)
        {
            Node node = graphManager.nodes[i];
            
            foreach (Edge edge in node.edges)
            {
                // Ensure the target index is valid
                if (edge.targetNodeIndex >= 0 && edge.targetNodeIndex < graphManager.nodes.Count)
                {
                    Node targetNode = graphManager.nodes[edge.targetNodeIndex];

                    // Draw the line for the edge
                    Handles.color = Color.yellow;
                    Handles.DrawLine(node.position, targetNode.position, 2f);

                    // Draw the label with the weight
                    Vector3 labelPosition = (node.position + targetNode.position) * 0.5f;
                    GUIStyle style = new GUIStyle();
                    style.normal.textColor = Color.white;
                    style.fontSize = 14;
                    style.fontStyle = FontStyle.Bold;
                    Handles.Label(labelPosition, edge.weight.ToString(), style);
                }
            }
        }

        // Draw all the nodes and their handles
        for (int i = 0; i < graphManager.nodes.Count; i++)
        {
            Node node = graphManager.nodes[i];

            // Make the node's position editable with a handle in the scene view
            Handles.color = Color.cyan;
            EditorGUI.BeginChangeCheck(); // Start checking if a value is about to change
            
            // Create a handle that the user can move
            var fmh_56_73_638889046497525919 = Quaternion.identity; Vector3 newPosition = Handles.FreeMoveHandle(node.position, 0.5f, Vector3.zero, Handles.SphereHandleCap);
            
            // Draw a label for the node's name
            Handles.Label(node.position + Vector3.up * 0.7f, node.name ?? $"Node {i}");

            if (EditorGUI.EndChangeCheck()) // If the handle was moved...
            {
                // Record the change for undo functionality
                Undo.RecordObject(graphManager, "Move Node");
                
                // Update the node's position
                node.position = newPosition;
            }
        }
    }
}