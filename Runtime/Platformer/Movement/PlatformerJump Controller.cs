using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/Platformer/Movement/Platformer Jump Controller", -98)]
public class PlatformerJumpController : MonoBehaviour
{
    [Header("Jump Settings")]
    public KeyCode jumpKey = KeyCode.Space;
    public Button jumpButton;
    public bool enableDoubleJump = false;
    public float jumpForce = 10f;
    public string groundTag = "Ground";

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = false;
    private bool canDoubleJump = false;

    // Telemetry
    private const string MechanicName = "Platformer/Jump";
    private bool _sentUsed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ConfigureJumpButton();

        // telemetry: instanciado
        ThinklibTelemetry.Track("mechanic_instantiated", MechanicName, nameof(PlatformerJumpController));
    }

    private void Update()
    {
        if (Input.GetKeyDown(jumpKey))
        {
            Jump();
        }

        // Update falling state in animator
        animator.SetBool("IsFalling", rb.velocity.y < 0 && !isGrounded);
    }

    private void ConfigureJumpButton()
    {
        if (jumpButton != null)
        {
            jumpButton.onClick.RemoveAllListeners();
            jumpButton.onClick.AddListener(Jump);
        }
    }

    public void Jump()
    {
        try
        {
            bool jumped = false;

            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetBool("IsJumping", true);
                if (enableDoubleJump) canDoubleJump = true;
                jumped = true;
            }
            else if (enableDoubleJump && canDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetBool("IsJumping", true);
                canDoubleJump = false;
                jumped = true;
            }

            // telemetry: primeiro uso real (primeiro pulo efetivo)
            if (jumped && !_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track("mechanic_used", MechanicName, nameof(PlatformerJumpController));
            }
        }
        catch (Exception ex)
        {
            // telemetry: erro
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlatformerJumpController),
                new Dictionary<string, object> {
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw; // mantém comportamento padrão
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            animator.SetBool("IsFalling", false);
            canDoubleJump = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(groundTag))
        {
            isGrounded = false;
        }
    }
}
