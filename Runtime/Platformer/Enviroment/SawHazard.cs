using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Thinklib/Game/SawHazard")]
public class SawHazard : MonoBehaviour
{
    [Header("Configuração de Dano")]
    [Tooltip("Quanto de dano esta serra causa ao tocar o jogador.")]
    public int damageAmount = 1; // Ajustei para 1, já que seu LifeSystem parece usar 5 de vida

    [Header("Movimento (Patrulha)")]
    [Tooltip("Marque se esta serra deve se mover.")]
    public bool shouldMove = false;
    
    [Tooltip("Ponto A da patrulha (posição de início).")]
    public Vector2 pointA;
    
    [Tooltip("Ponto B da patrulha (posição final).")]
    public Vector2 pointB;
    
    [Tooltip("Velocidade do movimento de patrulha.")]
    public float moveSpeed = 3.0f;
    
    private Vector2 targetPosition;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (shouldMove)
        {
            transform.position = pointA;
            targetPosition = pointB;
        }
    }

    void Update()
    {
        if (shouldMove)
        {
            Move();
        }
    }

    private void Move()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector2.Distance((Vector2)transform.position, targetPosition) < 0.1f)
        {
            targetPosition = (targetPosition == pointA) ? pointB : pointA;
        }
    }

    // Chamado quando outro collider entra no trigger da serra
    void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // MODIFICADO: Procura pelo seu script 'LifeSystemController'
            LifeSystemController lifeSystem = other.GetComponent<LifeSystemController>();
            
            if (lifeSystem != null)
            {
                // MODIFICADO: Chama a função 'TakeDamage' do seu script
                lifeSystem.TakeDamage(damageAmount);
            }
            else
            {
                Debug.LogWarning("Serra tocou o 'Player', mas não encontrou o script 'LifeSystemController'.");
            }
        }
    }

    // (Opcional) Ajuda visual para configurar os pontos de patrulha no Editor
    private void OnDrawGizmosSelected()
    {
        if (shouldMove)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(pointA, 0.3f);
            Gizmos.DrawWireSphere(pointB, 0.3f);
            Gizmos.DrawLine(pointA, pointB);
        }
    }
}