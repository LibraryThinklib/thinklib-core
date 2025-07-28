using UnityEngine;

public class VisualNodeClickHandler : MonoBehaviour
{
    public int nodeIndex;
    
    private void OnMouseDown()
    {
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