// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Topdown/NPC/Dialogue Bubble", -100)]
public class DialogueBubble : MonoBehaviour
{
    [Header("Text Field")]
    public Text textField;

    private Coroutine typeCoroutine;

    /// <summary>Whether the text is still being revealed letter by letter.</summary>
    public bool IsTyping { get; private set; } = false;

    private const string MechanicName = "Topdown/NPC/DialogueBubble";
    private bool _sentUsedSet   = false; // first time SetText is called
    private bool _sentUsedType  = false; // first time the typewriter effect is used

    private void Awake()
    {
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(DialogueBubble),
            new Dictionary<string, object> {
                { "hasTextField", textField != null }
            }
        );
    }

    /// <summary>
    /// Sets the text to display in the bubble, with or without the typing effect.
    /// </summary>
    /// <param name="text">Full line of dialogue.</param>
    /// <param name="useTypewriter">If true, enables the letter-by-letter typing effect.</param>
    /// <param name="speed">Typing speed (time between letters).</param>
    public void SetText(string text, bool useTypewriter = false, float speed = 0.05f)
    {
        try
        {
            if (textField == null)
            {
                Debug.LogWarning("The dialogue bubble's 'textField' is not assigned!");
                return;
            }

            if (typeCoroutine != null)
            {
                StopCoroutine(typeCoroutine);
                typeCoroutine = null;
            }

            if (useTypewriter)
            {
                typeCoroutine = StartCoroutine(TypeText(text, speed));

                // mechanic_used: first time the typewriter is activated
                if (!_sentUsedType)
                {
                    _sentUsedType = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(DialogueBubble),
                        new Dictionary<string, object> {
                            { "action", "typewriter_start" },
                            { "length", text?.Length ?? 0 },
                            { "speed", speed }
                        }
                    );
                }
            }
            else
            {
                textField.text = text ?? string.Empty;
                IsTyping = false;
            }

            // mechanic_used: first time text is set (regardless of mode)
            if (!_sentUsedSet)
            {
                _sentUsedSet = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(DialogueBubble),
                    new Dictionary<string, object> {
                        { "action", "set_text" },
                        { "length", text?.Length ?? 0 },
                        { "typewriter", useTypewriter }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(DialogueBubble),
                new Dictionary<string, object> {
                    { "where", "SetText" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private IEnumerator TypeText(string text, float speed)
    {
        // Important: don't wrap yield in try/catch.
        IsTyping = true;
        textField.text = "";
        string safeText = text ?? string.Empty;

        for (int i = 0; i < safeText.Length; i++)
        {
            textField.text += safeText[i];
            yield return new WaitForSeconds(speed);
        }

        IsTyping = false;

        // (Optional) We could mark the typewriter's completion only once.
        if (_sentUsedType)
        {
            ThinklibTelemetry.Track(
                "mechanic_used",
                MechanicName,
                nameof(DialogueBubble),
                new Dictionary<string, object> {
                    { "action", "typewriter_done" },
                    { "length", safeText.Length }
                }
            );
        }
    }
}
