using System.Collections;
using UnityEngine;

public interface IGridCommand
{
    string CommandName { get; }
    
    IEnumerator Execute(GridAgent agent);
}