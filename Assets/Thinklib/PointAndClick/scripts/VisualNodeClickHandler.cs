// VisualNodeClickHandler.cs
using UnityEngine;

public class VisualNodeClickHandler : MonoBehaviour
{
    public int nodeIndex; // This will be set by the VisualGraph script.
    
    // This function is called when the user has pressed the mouse button while over the Collider.
    private void OnMouseDown()
    {
        // Find the active pawn in the scene and tell it to move to this node.
        PathFollower activePawn = FindObjectOfType<PathFollower>();
        if (activePawn != null)
        {
            activePawn.MoveToNode(nodeIndex);
        }
        else
        {
            Debug.LogWarning("Clicked on a node, but no active PathFollower was found in the scene!");
        }
    }
}