using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public int pointsOnDeath = 5;  // Pontos dados ao jogador ao matar esse inimigo

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            // Adiciona pontos ao jogador
            PlayerScore playerScore = FindObjectOfType<PlayerScore>(); // Encontra o objeto PlayerScore
            if (playerScore != null)
            {
                playerScore.AddScore(pointsOnDeath);  // Adiciona pontos ao jogador
            }

            Destroy(gameObject); // Destrói o inimigo
        }
    }
}
