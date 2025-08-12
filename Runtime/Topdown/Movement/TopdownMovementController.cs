using UnityEngine;

[AddComponentMenu("Thinklib/Topdown/Movement/Topdown Movement Controller", -100)]
[RequireComponent(typeof(Animator))]
public class TopdownMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Joystick joystick;

    [Header("Input Settings")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.S;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;

    private Animator animator;
    private InputHandler inputHandler;

    // Salva a �ltima dire��o v�lida (para idle direcional)
    private Vector2 lastMoveDirection = Vector2.down;

    public Vector2 GetLastMoveDirection()
    {
        return lastMoveDirection;
    }


    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputHandler = GetComponent<InputHandler>() ?? gameObject.AddComponent<InputHandler>();
    }

    private void Update()
    {
        Vector2 input = GetInput();

        // Movement
        Move(input);

        // Animator
        UpdateAnimator(input);
    }

    private Vector2 GetInput()
    {
        if (joystick != null)
        {
            return new Vector2(joystick.Horizontal, joystick.Vertical);
        }
        else
        {
            float h = 0f;
            float v = 0f;

            if (Input.GetKey(leftKey)) h -= 1f;
            if (Input.GetKey(rightKey)) h += 1f;
            if (Input.GetKey(downKey)) v -= 1f;
            if (Input.GetKey(upKey)) v += 1f;

            return new Vector2(h, v).normalized;
        }
    }

    private void Move(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

            // Atualiza a �ltima dire��o
            lastMoveDirection = direction;
        }
    }

    private void UpdateAnimator(Vector2 direction)
    {
        bool isMoving = direction.sqrMagnitude > 0.01f;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // Atualiza o blend para a dire��o atual
            animator.SetFloat("Horizontal", direction.x);
            animator.SetFloat("Vertical", direction.y);
        }
        else
        {
            // Continua usando a �ltima dire��o salva para o Idle
            animator.SetFloat("Horizontal", lastMoveDirection.x);
            animator.SetFloat("Vertical", lastMoveDirection.y);
        }
    }
}
