using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Topdown/NPC/Dialogue Bubble", -100)]
public class DialogueBubble : MonoBehaviour
{
    [Header("Campo de texto")]
    public Text textField;

    private Coroutine typeCoroutine;

    /// <summary>Indica se o texto ainda está sendo exibido letra por letra.</summary>
    public bool IsTyping { get; private set; } = false;

    // Telemetry
    private const string MechanicName = "Topdown/NPC/DialogueBubble";
    private bool _sentUsedSet   = false; // primeira vez que SetText é chamado
    private bool _sentUsedType  = false; // primeira vez que efeito typewriter é usado

    private void Awake()
    {
        // mechanic_instantiated
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
    /// Define o texto a ser exibido no balão, com ou sem efeito de digitação.
    /// </summary>
    /// <param name="text">Texto completo da fala.</param>
    /// <param name="useTypewriter">Se verdadeiro, ativa o efeito de digitação letra por letra.</param>
    /// <param name="speed">Velocidade da digitação (tempo entre letras).</param>
    public void SetText(string text, bool useTypewriter = false, float speed = 0.05f)
    {
        try
        {
            if (textField == null)
            {
                Debug.LogWarning("Campo 'textField' do balão de fala não está atribuído!");
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

                // mechanic_used: primeira vez que ativa typewriter
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

            // mechanic_used: primeira vez que define texto (independente do modo)
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
        // Importante: não envolver yield com try/catch.
        IsTyping = true;
        textField.text = "";
        string safeText = text ?? string.Empty;

        for (int i = 0; i < safeText.Length; i++)
        {
            textField.text += safeText[i];
            yield return new WaitForSeconds(speed);
        }

        IsTyping = false;

        // (Opcional) Podemos marcar a conclusão do typewriter apenas uma vez.
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
