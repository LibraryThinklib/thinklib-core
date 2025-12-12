using System.Collections;
using UnityEngine;

public class MoveForwardCommand : ICommand
{
    public string CommandName => "Mover";

    public IEnumerator Execute(PlayerAgent agent)
    {
        yield return agent.StartCoroutine(agent.MoveForward());
    }
}