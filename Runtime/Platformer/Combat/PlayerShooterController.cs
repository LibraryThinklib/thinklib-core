using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thinklib.Platformer.Enemy.Core;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Platformer/Combat/Player Shooter Controller", -98)]
[RequireComponent(typeof(ProjectileShooterBase))]
public class PlayerShooterController : MonoBehaviour
{
    [Header("Input Settings")]
    public KeyCode shootKey = KeyCode.E;
    public Button shootButton;

    [Header("Shooting Settings")]
    public float timeBetweenShots = 1.5f;
    private float shotCooldownTimer = 0f;

    [Header("Damage Settings")]
    public int projectileDamage = 1;

    [Header("Debug Direction (Read Only)")]
    public bool isFacingRight = true;
    public bool isFacingLeft = false;

    private int direction = 1;
    private ProjectileShooterBase shooter;
    private Animator animator;

    // Telemetry
    private const string MechanicName = "Platformer/Combat/PlayerShooter";
    private bool _sentUsed = false;
    private string _lastInput = "unknown";

    private void Awake()
    {
        shooter  = GetComponent<ProjectileShooterBase>();
        animator = GetComponent<Animator>();
        ConfigureShootButton();

        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(PlayerShooterController),
            new Dictionary<string, object> {
                { "timeBetweenShots", timeBetweenShots },
                { "projectileDamage", projectileDamage },
                { "hasButton", shootButton != null }
            }
        );
    }

    private void Update()
    {
        shotCooldownTimer -= Time.deltaTime;

        if (Input.GetKeyDown(shootKey) && shotCooldownTimer <= 0f)
        {
            _lastInput = "keyboard";
            Shoot();
        }

        UpdateDirectionFromScale();
    }

    private void ConfigureShootButton()
    {
        if (shootButton != null)
        {
            shootButton.onClick.RemoveAllListeners();
            shootButton.onClick.AddListener(() =>
            {
                if (shotCooldownTimer <= 0f)
                {
                    _lastInput = "button";
                    Shoot();
                }
            });
        }
    }

    private void Shoot()
    {
        try
        {
            if (animator != null)
                animator.SetBool("IsShooting", true);

            GameObject proj = shooter.ShootProjectile(new Vector2(direction, 0));

            if (proj != null)
            {
                var damageDealer = proj.GetComponent<ProjectileDamageDealer>();
                if (damageDealer != null)
                {
                    damageDealer.damage = projectileDamage;
                }

                // mechanic_used (primeiro disparo efetivo)
                if (!_sentUsed)
                {
                    _sentUsed = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(PlayerShooterController),
                        new Dictionary<string, object> {
                            { "input", _lastInput },
                            { "direction", direction },
                            { "timeBetweenShots", timeBetweenShots },
                            { "projectileDamage", projectileDamage }
                        }
                    );
                }
            }

            shotCooldownTimer = timeBetweenShots;
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerShooterController),
                new Dictionary<string, object> {
                    { "where", "Shoot" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void UpdateDirectionFromScale()
    {
        direction     = transform.localScale.x >= 0 ? 1 : -1;
        isFacingRight = direction == 1;
        isFacingLeft  = direction == -1;
    }
}
