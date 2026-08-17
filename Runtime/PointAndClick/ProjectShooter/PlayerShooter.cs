// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;

[AddComponentMenu("Thinklib/Game/PlayerShooter")]
public class PlayerShooter : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The projectile prefab that will be fired.")]
    public GameObject projectilePrefab;

    [Tooltip("The point the projectile is fired from.")]
    public Transform firePoint;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (firePoint == null)
        {
            Debug.LogError("The 'firePoint' was not assigned on PlayerShooter!");
            firePoint = transform;
        }
    }

    void Update()
    {
        Aim();

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Aim()
    {
        if (mainCamera == null) return;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        Vector2 lookDirection = (Vector2)mouseWorldPos - (Vector2)transform.position;

        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;

        GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        Projectile projectileScript = projectileObj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.Initialize(firePoint.right);
        }
    }
}