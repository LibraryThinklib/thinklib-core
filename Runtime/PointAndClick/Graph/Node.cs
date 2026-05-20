// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[AddComponentMenu("Thinklib/Point and Click/Graph/Node", -99)]
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