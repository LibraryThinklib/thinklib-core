using System.Collections;
using UnityEngine;

public class TurnLeftCommand : ICommand
{
    public string CommandName => "Virar Esquerda";

    public IEnumerator Execute(PlayerAgent agent)
    {
        yield return agent.StartCoroutine(agent.TurnLeft());
    }
}