// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/TowerDefense/Defense/Tower Shooter", -98)]
public class TowerShooter : MonoBehaviour
{
    public float range = 3f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;

    private float fireCountdown = 0f;
    private Transform target;
    private TowerUpgrade towerUpgrade;  // Referência para o script TowerUpgrade

    // === Telemetry ===
    private const string MechanicName = "TowerDefense/Defense/TowerShooter";
    private bool _sentInstantiated = false;
    private bool _sentFirstShot   = false;

    private void Start()
    {
        towerUpgrade = GetComponent<TowerUpgrade>();  // Encontra o script TowerUpgrade

        // mechanic_instantiated (1x)
        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(TowerShooter),
                new Dictionary<string, object> {
                    { "range", range },
                    { "fireRate", fireRate },
                    { "hasBulletPrefab", bulletPrefab != null },
                    { "hasFirePoint", firePoint != null },
                    { "hasUpgrade", towerUpgrade != null },
                    { "upgradeDamage", towerUpgrade != null ? towerUpgrade.damage : 0 }
                }
            );
        }
    }

    private void Update()
    {
        try
        {
            UpdateTarget();
            if (target == null) return;

            if (fireCountdown <= 0f)
            {
                Shoot();
                fireCountdown = 1f / Mathf.Max(0.0001f, fireRate);
            }

            fireCountdown -= Time.deltaTime;
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TowerShooter),
                new Dictionary<string, object> {
                    { "where", "Update" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < shortestDistance && distance <= range)
            {
                shortestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        target = nearestEnemy != null ? nearestEnemy.transform : null;
    }

    private void Shoot()
    {
        try
        {
            if (bulletPrefab == null || firePoint == null) return;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetTarget(target);
                if (towerUpgrade != null)
                    bulletScript.damage = towerUpgrade.damage;  // Dano baseado no nível da torre
            }

            // mechanic_used: primeira vez que a torre atira
            if (!_sentFirstShot)
            {
                _sentFirstShot = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerShooter),
                    new Dictionary<string, object> {
                        { "action", "shoot" },
                        { "range", range },
                        { "fireRate", fireRate },
                        { "hasUpgrade", towerUpgrade != null },
                        { "damageNow", towerUpgrade != null ? towerUpgrade.damage : (bulletScript != null ? bulletScript.damage : 0) }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TowerShooter),
                new Dictionary<string, object> {
                    { "where", "Shoot" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
