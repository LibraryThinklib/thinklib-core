// Node.cs
using UnityEngine;
using System.Collections.Generic;

// We use [System.Serializable] so Unity knows how to save and show this class in the Inspector.
[System.Serializable]
public class Node
{
    public string name; // Optional: A name for the node like 'A', 'B', 'C'
    public Vector3 position;

    // Each node holds a list of its outgoing connections (edges)
    public List<Edge> edges = new List<Edge>();
    public bool isFinalNode = false;
}

// An Edge defines a connection to another node with a specific weight
[System.Serializable]
public class Edge
{
    // The node this edge connects to. We use an integer index to avoid complex object reference issues.
    public int targetNodeIndex;
    public float weight;
}