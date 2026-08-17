// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Thinklib/Game/Target")]
public class Target : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("The 'n' value this target adds to the score.")]
    public int scoreValue = 10;

    public void OnHit()
    {
        Debug.Log($"Target '{gameObject.name}' hit, +{scoreValue} points!");
        Destroy(gameObject);
    }
}