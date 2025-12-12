using System.Collections;
using UnityEngine;

public class GridMoveCommand : IGridCommand
{
    private int row;
    private int col;
    
    public string CommandName => $"Ir para (L:{row}, C:{col})";

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