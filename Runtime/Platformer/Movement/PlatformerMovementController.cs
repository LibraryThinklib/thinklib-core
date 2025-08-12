using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Thinklib/Platformer/Movement/Platformer Movement Controller", -97)]
[RequireComponent(typeof(Animator))]
public class PlatformerMovementController : MovementController
{
    [Header("Movement Settings")]
    public List<KeyCode> rightKeys = new List<KeyCode> { KeyCode.D, KeyCode.RightArrow };
    public List<KeyCode> leftKeys = new List<KeyCode> { KeyCode.A, KeyCode.LeftArrow };
    public Joystick joystick;

    [Header("Speed Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Player State")]
    public bool isJumping = false; // Updated by the jump script
    public bool isFalling = false; // Updated by the jump script

    [Header("Attack Settings")]
    public PlatformerProjectileAttackController projectileAttackController; // Reference to the projectile controller

    private InputHandler inputHandler;
    private Animator animator;
    private bool isFacingRight = true;

    private void Awake()
    {
        inputHandler = GetComponent<InputHandler>() ?? gameObject.AddComponent<InputHandler>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Get input direction
        Vector2 inputDirection = joystick != null
            ? inputHandler.GetJoystickInput(joystick)
            : inputHandler.GetKeyboardInput(rightKeys, leftKeys);

        // Determine movement speed
        float speed = Input.GetKey(runKey) ? runSpeed : walkSpeed;

        // Move the character
        Move(inputDirection, speed);

        // Update animator
        UpdateAnimator(inputDirection, speed);

        // Flip character sprite
        FlipSprite(inputDirection.x);
    }

    private void UpdateAnimator(Vector2 inputDirection, float speed)
    {
        animator.SetBool("IsMoving", inputDirection.magnitude > 0);
        animator.SetFloat("MoveSpeed", Mathf.Abs(speed));

        isJumping = animator.GetBool("IsJumping");
        isFalling = animator.GetBool("IsFalling");
    }

    private void FlipSprite(float horizontalInput)
    {
        if (horizontalInput > 0 && !isFacingRight)
        {
            Debug.Log("Flipping to the right");
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Debug.Log("Flipping to the left");
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Invert X axis to flip the sprite
        transform.localScale = localScale;

        // Update projectile direction accordingly
        if (projectileAttackController != null)
        {
            int newDirection = isFacingRight ? 1 : -1;
            projectileAttackController.SetDirection(newDirection); // Pass direction to the projectile controller
        }
    }
}
