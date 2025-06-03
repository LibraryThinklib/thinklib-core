using UnityEngine;
using UnityEngine.UI;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        ConfigureJumpButton();
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
        if (isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            if (enableDoubleJump) canDoubleJump = true;
        }
        else if (enableDoubleJump && canDoubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetBool("IsJumping", true);
            canDoubleJump = false;
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
