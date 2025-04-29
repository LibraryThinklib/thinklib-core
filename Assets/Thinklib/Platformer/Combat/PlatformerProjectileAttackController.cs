using UnityEngine;
using UnityEngine.UI;

public class PlatformerProjectileAttackController : MonoBehaviour
{
    [Header("Configura��es de Proj�til")]
    public GameObject projectilePrefab; // Prefab do proj�til
    public Transform launchPosition; // Posi��o inicial do disparo
    public float projectileSpeed = 10f; // Velocidade do proj�til
    public float maxProjectileLifetime = 5f; // Tempo m�ximo de vida do proj�til
    public Button shootButton; // Bot�o para disparar proj�til

    [Header("Key Bindings and Animation")]
    public KeyCode shootKey = KeyCode.E; // Tecla padr�o para lan�ar o proj�til
    private Animator animator; // Refer�ncia ao Animator do personagem

    [Header("Debug Dire��o (Somente Leitura)")]
    public bool isFacingRight = true; // Checkbox no inspetor para verificar se est� olhando para a direita
    public bool isFacingLeft = false; // Checkbox no inspetor para verificar se est� olhando para a esquerda

    private int direction = 1; // Dire��o padr�o (1 para direita, -1 para esquerda)

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
            Debug.Log("M�todo ShootProjectile ser� chamado.");
            ShootProjectile(); // Chama o m�todo de disparo do proj�til
        }

        // Atualiza os checkboxes de dire��o para depura��o (se necess�rio)
        UpdateDirectionDebug();
    }

    /// <summary>
    /// Configura o bot�o de disparo para chamar o m�todo ShootProjectile().
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
    /// Define a dire��o do proj�til.
    /// </summary>
    /// <param name="newDirection">1 para direita, -1 para esquerda.</param>
    public void SetDirection(int newDirection)
    {
        direction = Mathf.Clamp(newDirection, -1, 1);
        Debug.Log($"Dire��o do proj�til configurada para: {(direction == 1 ? "Direita" : "Esquerda")} ({direction})");

        // Atualiza os checkboxes de depura��o
        UpdateDirectionDebug();
    }

    /// <summary>
    /// Atualiza os checkboxes de dire��o para depura��o.
    /// </summary>
    private void UpdateDirectionDebug()
    {
        isFacingRight = direction == 1;
        isFacingLeft = direction == -1;
    }

    /// <summary>
    /// Dispara o proj�til na dire��o definida.
    /// </summary>
    public void ShootProjectile()
    {
        if (animator != null)
        {
            Debug.Log($"Antes: IsShooting = {animator.GetBool("IsShooting")}");
            animator.SetBool("IsShooting", true); // Ativa o par�metro bool
            Debug.Log($"Depois: IsShooting = {animator.GetBool("IsShooting")}");
            StartCoroutine(ResetShootingState());
        }

        // Garante que o prefab e a posi��o de lan�amento est�o configurados
        if (projectilePrefab == null || launchPosition == null)
        {
            Debug.LogWarning("Projectile prefab ou launch position n�o configurados!");
            return;
        }

        // Instancia o proj�til
        GameObject projectile = Instantiate(projectilePrefab, launchPosition.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Define a velocidade do proj�til com base na dire��o
            Vector2 velocity = new Vector2(projectileSpeed * direction, 0);
            rb.velocity = velocity;
            Debug.Log($"Velocidade do proj�til: {velocity}");
        }

        // Ajusta a escala do proj�til para corresponder � dire��o
        Vector3 scale = projectile.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        projectile.transform.localScale = scale;

        // Destroi o proj�til ap�s o tempo m�ximo de vida
        Destroy(projectile, maxProjectileLifetime);
    }

    private System.Collections.IEnumerator ResetShootingState()
    {
        yield return new WaitForSeconds(0.1f); // Tempo suficiente para a anima��o
        animator.SetBool("IsShooting", false);
        Debug.Log("Par�metro 'IsShooting' desativado.");
    }
}
