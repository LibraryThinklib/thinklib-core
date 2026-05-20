// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Thinklib/Game/ItemSpawner")]
public class ItemSpawner : MonoBehaviour
{
    [Header("Configuração de Spawn")]
    [Tooltip("O prefab do objeto que 'cai'. Deve ter o script FallingItem e um Collider2D.")]
    public GameObject fallingItemPrefab;

    [Tooltip("A lista de possíveis itens (ScriptableObjects) que podem ser 'dropados'.")]
    public List<Item> itemsToSpawn;

    [Tooltip("Intervalo, em segundos, entre cada item 'dropado'.")]
    public float spawnInterval = 2.0f;

    [Tooltip("Largura da área de spawn. O item surgirá em uma posição X aleatória dentro desta largura.")]
    public float spawnAreaWidth = 10f;

    [Tooltip("Atraso inicial antes de começar a 'dropar' itens.")]
    public float initialDelay = 1.0f;

    void Start()
    {
        if (fallingItemPrefab == null)
        {
            Debug.LogError("O prefab 'fallingItemPrefab' não foi atribuído no ItemSpawner!");
            return;
        }

        if (itemsToSpawn.Count == 0)
        {
            Debug.LogError("A lista 'itemsToSpawn' está vazia no ItemSpawner!");
            return;
        }
        
        // Começa a 'dropar' itens repetidamente
        InvokeRepeating("SpawnItem", initialDelay, spawnInterval);
    }

    void SpawnItem()
    {
        // 1. Escolhe um item aleatório da lista
        Item itemToSpawn = itemsToSpawn[Random.Range(0, itemsToSpawn.Count)];

        // 2. Calcula uma posição X aleatória
        float randomX = Random.Range(-spawnAreaWidth / 2, spawnAreaWidth / 2);
        Vector3 spawnPosition = transform.position + new Vector3(randomX, 0, 0);

        // 3. Instancia o prefab
        GameObject itemObject = Instantiate(fallingItemPrefab, spawnPosition, Quaternion.identity);

        // 4. Inicializa o item 'dropado' com os dados do ScriptableObject
        FallingItem fallingItem = itemObject.GetComponent<FallingItem>();
        if (fallingItem != null)
        {
            fallingItem.Initialize(itemToSpawn);
        }
        else
        {
            Debug.LogError($"O prefab {fallingItemPrefab.name} não contém o script FallingItem.cs!");
            Destroy(itemObject); // Destroi o objeto se estiver configurado incorretamente
        }
    }

    // Ajuda visual para ver a área de spawn no Editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 leftPoint = transform.position - new Vector3(spawnAreaWidth / 2, 0, 0);
        Vector3 rightPoint = transform.position + new Vector3(spawnAreaWidth / 2, 0, 0);
        Gizmos.DrawLine(leftPoint, rightPoint);
        Gizmos.DrawSphere(leftPoint, 0.2f);
        Gizmos.DrawSphere(rightPoint, 0.2f);
    }
}