using System;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/TowerDefense/Enemy Progression/Enemy Health", -100)]
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public int pointsOnDeath = 5;  // Pontos dados ao jogador ao matar esse inimigo

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/EnemyProgression/EnemyHealth";
    private bool _sentInstantiated = false;
    private bool _sentFirstHit     = false;
    private bool _sentDeath        = false;

    private void Start()
    {
        currentHealth = maxHealth;

        // mechanic_instantiated (1x)
        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(EnemyHealth),
                new Dictionary<string, object> {
                    { "maxHealth", maxHealth },
                    { "pointsOnDeath", pointsOnDeath }
                }
            );
        }
    }

    public void TakeDamage(int amount)
    {
        try
        {
            currentHealth -= amount;

            // mechanic_used: primeiro dano recebido
            if (!_sentFirstHit)
            {
                _sentFirstHit = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(EnemyHealth),
                    new Dictionary<string, object> {
                        { "action", "take_damage_first" },
                        { "amount", amount },
                        { "remainingHealth", currentHealth }
                    }
                );
            }

            if (currentHealth <= 0)
            {
                // mechanic_used: morte do inimigo
                if (!_sentDeath)
                {
                    _sentDeath = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(EnemyHealth),
                        new Dictionary<string, object> {
                            { "action", "death" },
                            { "pointsOnDeath", pointsOnDeath }
                        }
                    );
                }

                // Adiciona pontos ao jogador
                PlayerScore playerScore = FindObjectOfType<PlayerScore>();
                if (playerScore != null)
                {
                    playerScore.AddScore(pointsOnDeath);
                }

                Destroy(gameObject);
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(EnemyHealth),
                new Dictionary<string, object> {
                    { "where", "TakeDamage" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
