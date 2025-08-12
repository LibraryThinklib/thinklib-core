using UnityEngine;

[AddComponentMenu("Thinklib/Platformer/Environment/Moving Platform", -100)]
public class MovingPlatform : MonoBehaviour
{
    [Header("Configura��o de Movimenta��o")]
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private bool startActive = false;

    [Header("Configura��o de Ativa��o")]
    [SerializeField] private bool requirePlayerInput = false;
    [SerializeField] private KeyCode activationKey = KeyCode.E;

    private Transform targetPoint;
    private bool isWaiting = false;
    private bool isActive = false;
    private bool playerOnPlatform = false;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("PontoA e PontoB devem ser posicionados.");
            enabled = false;
            return;
        }

        transform.position = pointA.position;
        targetPoint = pointB;
        isActive = startActive;
    }

    private void Update()
    {
        // Ativa a plataforma se precisar de input e o jogador estiver nela
        if (requirePlayerInput && playerOnPlatform && Input.GetKeyDown(activationKey))
        {
            isActive = true;
        }

        if (!isActive || isWaiting) return;

        MoveToTarget();
    }

    private void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Ao chegar no destino, inicia a troca ap�s espera
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            StartCoroutine(WaitAndSwitch());
        }
    }

    private System.Collections.IEnumerator WaitAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        targetPoint = (targetPoint == pointA) ? pointB : pointA;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
            collision.transform.SetParent(transform); // Faz o jogador se mover junto com a plataforma
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
            collision.transform.SetParent(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
