// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingItem : MonoBehaviour
{
    [Header("Configuração de Movimento")]
    public float fallSpeed = 5.0f;

    [Header("Referências")]
    [SerializeField] private SpriteRenderer itemSpriteRenderer;

    // Guarda os dados do item (ScriptableObject)
    public Item itemData { get; private set; }
    
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Garante que o Rigidbody não seja afetado pela gravidade padrão (vamos controlar a velocidade)
        // e que não colida fisicamente com outros itens
        rb.isKinematic = true;
        
        // Garante que o Collider seja um 'trigger' para detecção, não colisão física
        GetComponent<Collider2D>().isTrigger = true;

        if (itemSpriteRenderer == null)
        {
            // Tenta encontrar o SpriteRenderer no objeto filho, se não foi atribuído
            itemSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Chamado pelo ItemSpawner para configurar este item
    public void Initialize(Item item)
    {
        this.itemData = item;
        
        // Atualiza o sprite para ser o ícone do item
        if (itemSpriteRenderer != null && item.icon != null)
        {
            itemSpriteRenderer.sprite = item.icon;
        }
    }

    void Update()
    {
        // Move o item para baixo em velocidade constante
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
    }

    // Detecta se o item saiu da tela (se você tiver uma "DespawnZone" na parte inferior)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DespawnZone"))
        {
            Destroy(gameObject);
        }
    }
}