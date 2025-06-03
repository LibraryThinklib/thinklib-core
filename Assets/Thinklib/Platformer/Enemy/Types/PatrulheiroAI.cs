using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PatrulheiroAI : MonoBehaviour
{
    [Header("Pontos de Patrulha (fora do inimigo!)")]
    [SerializeField] private Transform pontoA;
    [SerializeField] private Transform pontoB;

    [Header("Configurações")]
    [SerializeField] private float velocidade = 2f;
    [SerializeField] private float tolerancia = 0.5f;

    private Animator animator;
    private Transform destinoAtual;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        destinoAtual = pontoB;
        Flip();
    }

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        animator.SetBool("IsWalking", true);

        // Move em direção ao destino
        transform.position = Vector2.MoveTowards(transform.position, destinoAtual.position, velocidade * Time.deltaTime);

        // Chegou ao destino?
        if (Vector2.Distance(transform.position, destinoAtual.position) <= tolerancia)
        {
            AlternarDestino();
        }
    }

    private void AlternarDestino()
    {
        // Alterna entre ponto A e B
        destinoAtual = destinoAtual == pontoA ? pontoB : pontoA;
        Flip();
    }

    private void Flip()
    {
        Vector3 escala = transform.localScale;
        float direcao = destinoAtual.position.x - transform.position.x;

        if (direcao > 0f)
            escala.x = Mathf.Abs(escala.x); // olhando para a direita
        else if (direcao < 0f)
            escala.x = -Mathf.Abs(escala.x); // olhando para a esquerda

        transform.localScale = escala;
    }
}
