// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Grid/GridCommandManager")]
public class GridCommandManager : MonoBehaviour
{
    [Header("Main References")]
    public GridAgent gridAgent;
    public TextMeshProUGUI commandListText;

    [Header("Input UI References")]
    public TMP_InputField rowInput;
    public TMP_InputField colInput;

    private const string MechanicName = "PointAndClick/GridCoordinates/GridCommandManager";

    private List<IGridCommand> commandQueue = new List<IGridCommand>();
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(GridCommandManager),
            new Dictionary<string, object>
            {
                { "hasGridAgent", gridAgent != null },
                { "hasCommandListText", commandListText != null }
            });
    }

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
        if (gridAgent == null) return;

        try
        {
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(GridCommandManager),
                    new Dictionary<string, object>
                    {
                        { "action", "run_queue" },
                        { "commandCount", commandQueue.Count }
                    });
            }

            gridAgent.RunCommandQueue(commandQueue);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track("mechanic_error", MechanicName, nameof(GridCommandManager),
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
