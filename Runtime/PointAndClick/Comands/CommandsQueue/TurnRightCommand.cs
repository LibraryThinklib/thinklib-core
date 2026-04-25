using System.Collections;
using UnityEngine;

public class TurnRightCommand : ICommand
{
    public string CommandName => "Virar Direita";

    public IEnumerator Execute(PlayerAgent agent)
    {
        yield return agent.StartCoroutine(agent.TurnRight());
    }
}