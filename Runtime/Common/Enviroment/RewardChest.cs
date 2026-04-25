using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Thinklib/Game/RewardChest")]
public class RewardChest : MonoBehaviour
{
    [Header("Configuração da Recompensa")]
    [Tooltip("Quantidade de vida a ser curada.")]
    public int healthToGive = 1;

    [Header("Visuais")]
    [Tooltip("Sprite opcional do baú 'aberto' para mostrar que foi usado.")]
    public Sprite openSprite;

    private const string MechanicName = "Common/Enviroment/RewardChest";

    private SpriteRenderer spriteRenderer;
    private bool isUsed = false;
    private bool _sentUsed = false;

    private void Awake()
    {
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(RewardChest),
            new Dictionary<string, object>
            {
                { "healthToGive", healthToGive },
                { "hasOpenSprite", openSprite != null }
            });
    }

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isUsed || !other.CompareTag("Player")) return;

        try
        {
            LifeSystemController lifeSystem = other.GetComponent<LifeSystemController>();

            if (lifeSystem != null)
            {
                lifeSystem.Heal(healthToGive);
                Debug.Log($"Jogador curou {healthToGive} de vida!");

                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(RewardChest),
                        new Dictionary<string, object>
                        {
                            { "action", "heal" },
                            { "healthToGive", healthToGive }
                        });
                }

                MarkAsUsed();
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track("mechanic_error", MechanicName, nameof(RewardChest),
                new Dictionary<string, object>
                {
                    { "where", "OnTriggerEnter2D" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                });
            throw;
        }
    }

    private void MarkAsUsed()
    {
        isUsed = true;
        GetComponent<Collider2D>().enabled = false;

        if (spriteRenderer != null && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }
    }
}
