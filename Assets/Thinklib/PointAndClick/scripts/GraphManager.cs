// GraphManager.cs
using UnityEngine;
using System.Collections.Generic;

public class GraphManager : MonoBehaviour
{
    // This list will hold all the nodes in our graph.
    // We will manage it from the Inspector and our custom editor.
    public List<Node> nodes = new List<Node>();
}