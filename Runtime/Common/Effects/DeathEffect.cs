using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Common/Effects/Death Effect", -100)]
[RequireComponent(typeof(SpriteRenderer))]
public class DeathEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float blinkDuration = 1.2f;
    public float blinkInterval = 0.1f;

    [Header("UI to Hide on Death (Optional)")]
    public GameObject[] uiElementsToHide;

    private SpriteRenderer spriteRenderer;

    // Telemetry
    private const string MechanicName = "Common/Effects/DeathEffect";
    private bool _sentUsed = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(DeathEffect),
            new Dictionary<string, object> {
                { "blinkDuration", blinkDuration },
                { "blinkInterval", blinkInterval },
                { "uiElementsCount", uiElementsToHide != null ? uiElementsToHide.Length : 0 }
            }
        );
    }

    /// <summary>
    /// Starts the death effect with blinking and destroys the object.
    /// </summary>
    public void PlayDeathEffect()
    {
        try
        {
            HideUIElements();

            // mechanic_used (primeira execução do efeito)
            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(DeathEffect),
                    new Dictionary<string, object> {
                        { "blinkDuration", blinkDuration },
                        { "blinkInterval", blinkInterval }
                    }
                );
            }

            StartCoroutine(BlinkAndDestroy());
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(DeathEffect),
                new Dictionary<string, object> {
                    { "where", "PlayDeathEffect" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void HideUIElements()
    {
        if (uiElementsToHide == null) return;

        foreach (var ui in uiElementsToHide)
        {
            if (ui != null)
                ui.SetActive(false);
        }
    }

    // >>> Não usar try/catch em iteradores. Use helpers sem yield para telemetria de erro e try/finally aqui.
    private IEnumerator BlinkAndDestroy()
    {
        float timer = 0f;

        try
        {
            while (timer < blinkDuration)
            {
                // Operação sem yield -> pode ter try/catch interno com telemetria
                BlinkStepSafe();

                // O yield permanece limpo (sem catch no iterador)
                yield return new WaitForSeconds(blinkInterval);
                timer += blinkInterval;
            }
        }
        finally
        {
            // Limpeza garantida
            SafeDestroy();
        }
    }

    // Helper sem yield: aqui podemos capturar exceções e enviar telemetria
    private void BlinkStepSafe()
    {
        try
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(DeathEffect),
                new Dictionary<string, object> {
                    { "where", "BlinkStepSafe" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void SafeDestroy()
    {
        try
        {
            Destroy(gameObject);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(DeathEffect),
                new Dictionary<string, object> {
                    { "where", "SafeDestroy" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
        }
    }
}
