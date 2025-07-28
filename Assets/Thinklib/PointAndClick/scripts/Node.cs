using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Node
{
    public string name;
    public Vector3 position;

    public List<Edge> edges = new List<Edge>();
    public bool isFinalNode = false;
}

[System.Serializable]
public class Edge
{
    public int targetNodeIndex;
    public float weight;
}