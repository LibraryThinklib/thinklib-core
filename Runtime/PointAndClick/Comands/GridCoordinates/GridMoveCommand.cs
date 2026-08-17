// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System.Collections;
using UnityEngine;

public class GridMoveCommand : IGridCommand
{
    private int row;
    private int col;
    
    public string CommandName => $"Go to (L:{row}, C:{col})";

    public GridMoveCommand(int targetRow, int targetCol)
    {
        this.row = targetRow;
        this.col = targetCol;
    }

    public IEnumerator Execute(GridAgent agent)
    {
        yield return agent.StartCoroutine(agent.GoToCoordinates(row, col));
    }
}