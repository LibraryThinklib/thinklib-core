using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlatformerMovementController : MovementController
{
    [Header("Configura��o de Movimenta��o")]
    public List<KeyCode> rightKeys = new List<KeyCode> { KeyCode.D, KeyCode.RightArrow };
    public List<KeyCode> leftKeys = new List<KeyCode> { KeyCode.A, KeyCode.LeftArrow };
    public Joystick joystick;

    [Header("Configura��es de Velocidade")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public KeyCode runKey = KeyCode.LeftShift;

    [Header("Estado do Jogador")]
    public bool isJumping = false; // Atualizado pelo script de pulo
    public bool isFalling = false; // Atualizado pelo script de pulo

    [Header("Configura��o de Ataque")]
    public PlatformerProjectileAttackController projectileAttackController; // Refer�ncia ao controlador de proj�teis

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
        // Obter a dire��o de entrada
        Vector2 inputDirection = joystick != null
            ? inputHandler.GetJoystickInput(joystick)
            : inputHandler.GetKeyboardInput(rightKeys, leftKeys);

        // Determinar a velocidade
        float speed = Input.GetKey(runKey) ? runSpeed : walkSpeed;

        // Movimentar o personagem
        Move(inputDirection, speed);

        // Atualizar o Animator
        UpdateAnimator(inputDirection, speed);

        // Girar o sprite do personagem
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
            Debug.Log("Virando para a direita");
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Debug.Log("Virando para a esquerda");
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1; // Inverte o eixo X para virar o sprite
        transform.localScale = localScale;

        // Atualiza a dire��o do proj�til de forma consistente
        if (projectileAttackController != null)
        {
            int newDirection = isFacingRight ? 1 : -1;
            projectileAttackController.SetDirection(newDirection); // Passa a dire��o para o controlador de proj�teis
        }
    }
}
