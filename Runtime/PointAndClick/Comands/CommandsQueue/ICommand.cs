using System.Collections;
using UnityEngine;

public interface ICommand
{
    string CommandName { get; }
    
    IEnumerator Execute(PlayerAgent agent);
}