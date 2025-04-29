using UnityEngine;
using UnityEngine.UI;

public class PlatformerProjectileAttackController : MonoBehaviour
{
    [Header("Configurações de Projétil")]
    public GameObject projectilePrefab; // Prefab do projétil
    public Transform launchPosition; // Posição inicial do disparo
    public float projectileSpeed = 10f; // Velocidade do projétil
    public float maxProjectileLifetime = 5f; // Tempo máximo de vida do projétil
    public Button shootButton; // Botão para disparar projétil

    [Header("Key Bindings and Animation")]
    public KeyCode shootKey = KeyCode.E; // Tecla padrão para lançar o projétil
    private Animator animator; // Referência ao Animator do personagem

    [Header("Debug Direção (Somente Leitura)")]
    public bool isFacingRight = true; // Checkbox no inspetor para verificar se está olhando para a direita
    public bool isFacingLeft = false; // Checkbox no inspetor para verificar se está olhando para a esquerda

    private int direction = 1; // Direção padrão (1 para direita, -1 para esquerda)

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ConfigureShootButton();
    }

    private void Update()
    {
        // Verifica se a tecla de disparo foi pressionada
        if (Input.GetKeyDown(shootKey))
        {
            Debug.Log("Método ShootProjectile será chamado.");
            ShootProjectile(); // Chama o método de disparo do projétil
        }

        // Atualiza os checkboxes de direção para depuração (se necessário)
        UpdateDirectionDebug();
    }

    /// <summary>
    /// Configura o botão de disparo para chamar o método ShootProjectile().
    /// </summary>
    private void ConfigureShootButton()
    {
        if (shootButton != null)
        {
            shootButton.onClick.RemoveAllListeners();
            shootButton.onClick.AddListener(ShootProjectile);
        }
    }

    /// <summary>
    /// Define a direção do projétil.
    /// </summary>
    /// <param name="newDirection">1 para direita, -1 para esquerda.</param>
    public void SetDirection(int newDirection)
    {
        direction = Mathf.Clamp(newDirection, -1, 1);
        Debug.Log($"Direção do projétil configurada para: {(direction == 1 ? "Direita" : "Esquerda")} ({direction})");

        // Atualiza os checkboxes de depuração
        UpdateDirectionDebug();
    }

    /// <summary>
    /// Atualiza os checkboxes de direção para depuração.
    /// </summary>
    private void UpdateDirectionDebug()
    {
        isFacingRight = direction == 1;
        isFacingLeft = direction == -1;
    }

    /// <summary>
    /// Dispara o projétil na direção definida.
    /// </summary>
    public void ShootProjectile()
    {
        if (animator != null)
        {
            Debug.Log($"Antes: IsShooting = {animator.GetBool("IsShooting")}");
            animator.SetBool("IsShooting", true); // Ativa o parâmetro bool
            Debug.Log($"Depois: IsShooting = {animator.GetBool("IsShooting")}");
            StartCoroutine(ResetShootingState());
        }

        // Garante que o prefab e a posição de lançamento estão configurados
        if (projectilePrefab == null || launchPosition == null)
        {
            Debug.LogWarning("Projectile prefab ou launch position não configurados!");
            return;
        }

        // Instancia o projétil
        GameObject projectile = Instantiate(projectilePrefab, launchPosition.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Define a velocidade do projétil com base na direção
            Vector2 velocity = new Vector2(projectileSpeed * direction, 0);
            rb.velocity = velocity;
            Debug.Log($"Velocidade do projétil: {velocity}");
        }

        // Ajusta a escala do projétil para corresponder à direção
        Vector3 scale = projectile.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        projectile.transform.localScale = scale;

        // Destroi o projétil após o tempo máximo de vida
        Destroy(projectile, maxProjectileLifetime);
    }

    private System.Collections.IEnumerator ResetShootingState()
    {
        yield return new WaitForSeconds(0.1f); // Tempo suficiente para a animação
        animator.SetBool("IsShooting", false);
        Debug.Log("Parâmetro 'IsShooting' desativado.");
    }
}
