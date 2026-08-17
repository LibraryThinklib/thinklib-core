// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Grid/GridManager")]
public class GridManager : MonoBehaviour
{
    public static GridManager instance;

    [Header("Grid Configuration")]
    [Tooltip("The world position (X, Y) of your (0, 0) cell, in the top-left corner.")]
    public Vector2 gridOrigin;

    [Tooltip("The size of each grid cell (in Unity units).")]
    public float cellSize = 1.0f;

    private const string MechanicName = "PointAndClick/GridCoordinates/GridManager";

    void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); return; }
        instance = this;

        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(GridManager),
            new Dictionary<string, object>
            {
                { "cellSize", cellSize },
                { "gridOriginX", gridOrigin.x },
                { "gridOriginY", gridOrigin.y }
            });
    }

    public Vector3 GetWorldPosition(int row, int col)
    {
        float x = gridOrigin.x + (col * cellSize);
        float y = gridOrigin.y - (row * cellSize);
        return new Vector3(x, y, 0);
    }
}
