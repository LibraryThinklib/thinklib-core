using System.Collections.Generic;
using UnityEngine;
using TMPro;

[AddComponentMenu("Thinklib/Grid/GridCommandManager")]
public class GridCommandManager : MonoBehaviour
{
    [Header("Referências Principais")]
    public GridAgent gridAgent;
    public TextMeshProUGUI commandListText;

    [Header("Referências da UI de Input")]
    public TMP_InputField rowInput;
    public TMP_InputField colInput;

    private List<IGridCommand> commandQueue = new List<IGridCommand>();

    public void AddCommandFromUI()
    {
        if (rowInput == null || colInput == null) return;

        if (int.TryParse(rowInput.text, out int row) && 
            int.TryParse(colInput.text, out int col))
        {
            commandQueue.Add(new GridMoveCommand(row, col));
            UpdateCommandListUI();
        }
    }

    public void RunQueue()
    {
        if (gridAgent != null)
        {
            gridAgent.RunCommandQueue(commandQueue);
        }
    }

    public void ClearQueue()
    {
        commandQueue.Clear();
        UpdateCommandListUI();
        
        if (gridAgent != null)
        {
            gridAgent.ResetAgent();
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
        
        foreach (IGridCommand command in commandQueue)
        {
            commandListText.text += command.CommandName + "\n";
        }
    }
}