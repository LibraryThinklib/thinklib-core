using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[AddComponentMenu("Thinklib/Game/CommandQueueManager")]
public class CommandQueueManager : MonoBehaviour
{
    [Header("Referências")]
    public PlayerAgent playerAgent;
    public TextMeshProUGUI commandListText;

    private List<ICommand> commandQueue = new List<ICommand>();

    private int lastExecutedIndex = 0;

    public void AddMoveCommand()
    {
        commandQueue.Add(new MoveForwardCommand());
        UpdateCommandListUI();
    }

    public void AddTurnLeftCommand()
    {
        commandQueue.Add(new TurnLeftCommand());
        UpdateCommandListUI();
    }

    public void AddTurnRightCommand()
    {
        commandQueue.Add(new TurnRightCommand());
        UpdateCommandListUI();
    }

    public void RunQueue()
    {
        if (playerAgent == null) return;

        if (lastExecutedIndex < commandQueue.Count)
        {
            int count = commandQueue.Count - lastExecutedIndex;

            List<ICommand> newCommands = commandQueue.GetRange(lastExecutedIndex, count);

            Debug.Log($"Executando {count} novos comandos...");

            playerAgent.RunCommandQueue(newCommands);

            lastExecutedIndex = commandQueue.Count;
            
            UpdateCommandListUI();
        }
        else
        {
            Debug.Log("Nenhum comando novo para executar.");
        }
    }

    public void ClearQueue()
    {
        commandQueue.Clear();
        
        lastExecutedIndex = 0;
        
        UpdateCommandListUI();
        
        if (playerAgent != null)
        {
            playerAgent.ResetAgent();
        }
    }

    private void Start()
    {
        UpdateCommandListUI();
    }

    private void UpdateCommandListUI()
    {
        if (commandListText == null) return;

        commandListText.text = "";
        
        for (int i = 0; i < commandQueue.Count; i++)
        {
            string cmdName = commandQueue[i].CommandName;

            if (i < lastExecutedIndex)
            {
                commandListText.text += $"<color=grey>OK {cmdName}</color>\n";
            }
            else
            {
                commandListText.text += cmdName + "\n";
            }
        }
    }
}