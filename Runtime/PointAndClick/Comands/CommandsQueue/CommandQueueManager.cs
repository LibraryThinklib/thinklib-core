using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;
using TMPro;

[AddComponentMenu("Thinklib/Game/CommandQueueManager")]
public class CommandQueueManager : MonoBehaviour
{
    [Header("Referências")]
    public PlayerAgent playerAgent;
    public TextMeshProUGUI commandListText;

    private const string MechanicName = "PointAndClick/CommandsQueue/CommandQueueManager";

    private List<ICommand> commandQueue = new List<ICommand>();
    private int lastExecutedIndex = 0;
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(CommandQueueManager),
            new Dictionary<string, object>
            {
                { "hasPlayerAgent", playerAgent != null },
                { "hasCommandListText", commandListText != null }
            });
    }

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

        try
        {
            if (lastExecutedIndex < commandQueue.Count)
            {
                int count = commandQueue.Count - lastExecutedIndex;
                List<ICommand> newCommands = commandQueue.GetRange(lastExecutedIndex, count);

                Debug.Log($"Executando {count} novos comandos...");

                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(CommandQueueManager),
                        new Dictionary<string, object>
                        {
                            { "action", "run_queue" },
                            { "commandCount", count }
                        });
                }

                playerAgent.RunCommandQueue(newCommands);
                lastExecutedIndex = commandQueue.Count;
                UpdateCommandListUI();
            }
            else
            {
                Debug.Log("Nenhum comando novo para executar.");
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track("mechanic_error", MechanicName, nameof(CommandQueueManager),
                new Dictionary<string, object>
                {
                    { "where", "RunQueue" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                });
            throw;
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
                commandListText.text += $"<color=grey>OK {cmdName}</color>\n";
            else
                commandListText.text += cmdName + "\n";
        }
    }
}
