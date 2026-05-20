// Copyright (c) 2026 Thinkted Lab
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections; // (não usamos yield aqui, mas mantive por consistência)
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Topdown/NPC/NPC Controller", -99)]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider2D))]
public class TopdownNPCController : MonoBehaviour
{
    public enum NPCType { Static, Patroller }

    [Header("Tipo do NPC")]
    public NPCType npcType = NPCType.Static;

    [Header("Pontos de patrulha")]
    public List<Transform> patrolPoints = new List<Transform>();
    public float patrolSpeed = 2f;
    public float patrolTolerance = 0.1f;
    private int currentPointIndex = 0;

    [Header("Falas")]
    public bool hasDialogues = false;
    [TextArea(2, 5)]
    public List<string> dialogues = new List<string>();
    public bool useTypewriterEffect = false;
    public float typeSpeed = 0.05f;

    [Header("Configurações de interação")]
    public KeyCode interactionKey = KeyCode.E;
    public GameObject dialogueBubblePrefab;
    public Transform bubbleAnchor;

    [Header("Camadas que bloqueiam o caminho")]
    public LayerMask obstructionLayers;

    [Header("Referência do jogador")]
    public Transform player;
    public MonoBehaviour playerMovementScript; // Ex: PlayerMovement (deve ser um MonoBehaviour com .enabled)

    private Animator animator;
    private bool isTouchingPlayer = false;
    private bool isTalking = false;
    private int currentDialogueIndex = 0;
    private GameObject activeBubble;
    private DialogueBubble dialogueBubbleComponent;
    private Vector2 lastDirection = Vector2.down;
    private bool pathBlocked = false;
    private bool playerNearby = false;

    // === TELEMETRIA ===
    private const string MechanicName = "Topdown/NPC/TopdownNPCController";
    private bool _sentUsedPatrolStart = false;
    private bool _sentUsedDialogueOpen = false;
    private bool _sentUsedDialogueAdvance = false;
    private bool _sentUsedDialogueEnd = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        // mechanic_instantiated
        ThinklibTelemetry.Track(
            "mechanic_instantiated",
            MechanicName,
            nameof(TopdownNPCController),
            new Dictionary<string, object> {
                { "npcType", npcType.ToString() },
                { "patrolPoints", patrolPoints?.Count ?? 0 },
                { "patrolSpeed", patrolSpeed },
                { "hasDialogues", hasDialogues },
                { "dialogueCount", dialogues?.Count ?? 0 },
                { "useTypewriter", useTypewriterEffect },
                { "typeSpeed", typeSpeed },
                { "interactionKey", interactionKey.ToString() },
                { "hasBubblePrefab", dialogueBubblePrefab != null },
                { "hasBubbleAnchor", bubbleAnchor != null }
            }
        );
    }

    private void Update()
    {
        try
        {
            if (npcType == NPCType.Patroller && !isTalking && !pathBlocked && patrolPoints.Count > 0 && !isTouchingPlayer)
            {
                Patrol();
            }
            else
            {
                animator.SetBool("IsMoving", false);
                if (isTouchingPlayer)
                {
                    FacePlayerDirection();
                }
            }

            if (hasDialogues && Input.GetKeyDown(interactionKey) && playerNearby)
            {
                if (!isTalking)
                {
                    currentDialogueIndex = 0;
                    ShowDialogue();
                }
                else if (dialogueBubbleComponent != null && !dialogueBubbleComponent.IsTyping)
                {
                    currentDialogueIndex++;
                    if (currentDialogueIndex >= dialogues.Count)
                    {
                        EndDialogue();
                    }
                    else
                    {
                        ShowDialogue(advance:true);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TopdownNPCController),
                new Dictionary<string, object> {
                    { "where", "Update" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void Patrol()
    {
        Transform target = patrolPoints[currentPointIndex];
        Vector2 direction = (target.position - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, target.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, obstructionLayers);
        pathBlocked = hit.collider != null;

        if (!pathBlocked)
        {
            if (!_sentUsedPatrolStart)
            {
                _sentUsedPatrolStart = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TopdownNPCController),
                    new Dictionary<string, object> {
                        { "action", "patrol_start" },
                        { "points", patrolPoints?.Count ?? 0 },
                        { "speed", patrolSpeed }
                    }
                );
            }

            animator.SetBool("IsMoving", true);
            SetAnimatorDirection(direction);
            transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);
            // (opcional) reportar primeira vez que bloqueia caminho
            // Manter simples: sem evento extra para não poluir.
        }

        if (Vector2.Distance(transform.position, target.position) < patrolTolerance)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Count;
        }
    }

    private void SetAnimatorDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            lastDirection = direction;
        }

        animator.SetFloat("Horizontal", lastDirection.x);
        animator.SetFloat("Vertical",   lastDirection.y);
    }

    private void ShowDialogue(bool advance = false)
    {
        try
        {
            isTalking = true;
            animator.SetBool("IsMoving", false);
            if (playerMovementScript != null)
                playerMovementScript.enabled = false;

            if (activeBubble != null) Destroy(activeBubble);

            activeBubble = Instantiate(dialogueBubblePrefab, bubbleAnchor.position, Quaternion.identity, bubbleAnchor);
            dialogueBubbleComponent = activeBubble.GetComponent<DialogueBubble>();
            if (dialogueBubbleComponent != null)
            {
                string line = (currentDialogueIndex >= 0 && currentDialogueIndex < dialogues.Count) ? dialogues[currentDialogueIndex] : string.Empty;
                dialogueBubbleComponent.SetText(line, useTypewriterEffect, typeSpeed);
            }

            // Telemetry: primeira abertura e primeiras avanços
            if (!advance && !_sentUsedDialogueOpen)
            {
                _sentUsedDialogueOpen = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TopdownNPCController),
                    new Dictionary<string, object> {
                        { "action", "dialogue_open" },
                        { "index", currentDialogueIndex },
                        { "total", dialogues?.Count ?? 0 },
                        { "typewriter", useTypewriterEffect }
                    }
                );
            }
            else if (advance && !_sentUsedDialogueAdvance)
            {
                _sentUsedDialogueAdvance = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TopdownNPCController),
                    new Dictionary<string, object> {
                        { "action", "dialogue_advance" },
                        { "index", currentDialogueIndex },
                        { "total", dialogues?.Count ?? 0 }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TopdownNPCController),
                new Dictionary<string, object> {
                    { "where", "ShowDialogue" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void EndDialogue()
    {
        try
        {
            isTalking = false;
            currentDialogueIndex = 0;
            if (activeBubble != null) Destroy(activeBubble);
            if (playerMovementScript != null)
                playerMovementScript.enabled = true;

            if (!_sentUsedDialogueEnd)
            {
                _sentUsedDialogueEnd = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TopdownNPCController),
                    new Dictionary<string, object> {
                        { "action", "dialogue_end" }
                    }
                );
            }
        }
        catch (Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TopdownNPCController),
                new Dictionary<string, object> {
                    { "where", "EndDialogue" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        foreach (var point in patrolPoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, 0.1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            playerNearby = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            playerNearby = false;
        }
    }

    private void FacePlayerDirection()
    {
        if (player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        if (directionToPlayer.sqrMagnitude > 0.01f)
        {
            lastDirection = directionToPlayer;
            animator.SetFloat("Horizontal", lastDirection.x);
            animator.SetFloat("Vertical",   lastDirection.y);
        }
    }

    private void OnDrawGizmos()
    {
        if (npcType == NPCType.Patroller && patrolPoints.Count > 0)
        {
            Vector2 direction = (patrolPoints[currentPointIndex].position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, patrolPoints[currentPointIndex].position);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, direction * distance);
        }
    }
}
