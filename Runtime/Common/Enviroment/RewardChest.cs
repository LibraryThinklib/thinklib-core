using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[AddComponentMenu("Thinklib/Game/RewardChest")]
public class RewardChest : MonoBehaviour
{
    [Header("Configuração da Recompensa")]
    [Tooltip("Quantidade de vida a ser curada.")]
    public int healthToGive = 1;

    [Header("Visuais")]
    [Tooltip("Sprite opcional do baú 'aberto' para mostrar que foi usado.")]
    public Sprite openSprite;

    private SpriteRenderer spriteRenderer;
    private bool isUsed = false;

    void Start()
    {
        // Garante que o collider é um trigger
        GetComponent<Collider2D>().isTrigger = true;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Chamado quando o jogador toca o baú
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Se já foi usado ou não for o jogador, não faz nada
        if (isUsed || !other.CompareTag("Player"))
        {
            return;
        }

        // --- LÓGICA DE DAR VIDA ---
        LifeSystemController lifeSystem = other.GetComponent<LifeSystemController>();
        
        if (lifeSystem != null)
        {
            lifeSystem.Heal(healthToGive);
            Debug.Log($"Jogador curou {healthToGive} de vida!");
            
            // Marca o baú como usado
            MarkAsUsed();
        }
    }

    private void MarkAsUsed()
    {
        isUsed = true;

        // Desativa o collider para não ser pego de novo
        GetComponent<Collider2D>().enabled = false;

        // Muda o sprite para o sprite de "aberto", se houver
        if (spriteRenderer != null && openSprite != null)
        {
            spriteRenderer.sprite = openSprite;
        }

        // (Opcional) Tocar um som de "baú abrindo"
        // AudioSource.PlayClipAtPoint(openSound, transform.position);
    }
}