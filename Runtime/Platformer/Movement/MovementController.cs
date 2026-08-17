// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;

public class MovementController : MonoBehaviour
{
    protected float currentSpeed;
    protected Vector2 direction;

    public virtual void Move(Vector2 inputDirection, float speed)
    {
        direction = inputDirection.normalized;
        currentSpeed = speed;
        transform.Translate(direction * currentSpeed * Time.deltaTime);
    }
}
