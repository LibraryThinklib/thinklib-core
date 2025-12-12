using System;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Platformer/Enemy/Shooter/Projectile Damage Dealer", -89)]
[RequireComponent(typeof(Collider2D))]
public class ProjectileDamageDealer : MonoBehaviour
{
    [Header("Dano")]
    public int damage = 1;

    [Header("Configuração de colisão")]
    public string targetTag = "Player";

    // Telemetry
    private const string MechanicName = "Platformer/Enemy/Projectile";
    private bool _sentUsed = false;

    private void Awake()
    {
        // Evento de instância do projétil (um por spawn)
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(ProjectileDamageDealer),
            new Dictionary<string, object> {
                { "damage", damage },
                { "targetTag", targetTag }
            }
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        try
        {
            if (!other.CompareTag(targetTag)) return;

            var life = other.GetComponent<LifeSystemController>();
            var hurtEffect = other.GetComponent<PlayerHurtEffect>();

            if (life != null && (hurtEffect == null || !hurtEffect.IsInvulnerable))
            {
                life.TakeDamage(damage);

                if (hurtEffect != null)
                    hurtEffect.TriggerInvulnerability();

                // Primeiro uso efetivo do projétil (acerto)
                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(ProjectileDamageDealer),
                        new Dictionary<string, object> {
                            { "hitTag", targetTag },
                            { "damage", damage }
                        }
                    );
                }
            }

            Destroy(gameObject);
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(ProjectileDamageDealer),
                new Dictionary<string, object> {
                    { "where", "OnTriggerEnter2D" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
