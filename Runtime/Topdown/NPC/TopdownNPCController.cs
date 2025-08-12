using UnityEngine;
using System.Collections.Generic;

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

    [Header("Configura��es de intera��o")]
    public KeyCode interactionKey = KeyCode.E;
    public GameObject dialogueBubblePrefab;
    public Transform bubbleAnchor;

    [Header("Camadas que bloqueiam o caminho")]
    public LayerMask obstructionLayers;

    [Header("Refer�ncia do jogador")]
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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
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
                    ShowDialogue();
                }
            }
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
            animator.SetBool("IsMoving", true);
            SetAnimatorDirection(direction);
            transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
        }
        else
        {
            animator.SetBool("IsMoving", false);
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
        animator.SetFloat("Vertical", lastDirection.y);
    }

    private void ShowDialogue()
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
            dialogueBubbleComponent.SetText(dialogues[currentDialogueIndex], useTypewriterEffect, typeSpeed);
        }
    }

    private void EndDialogue()
    {
        isTalking = false;
        currentDialogueIndex = 0;
        if (activeBubble != null) Destroy(activeBubble);
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
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
            animator.SetFloat("Vertical", lastDirection.y);
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
