using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Thinklib/TowerDefense/Defeat System/Player Health", -100)]
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;  // Quantidade inicial de vidas
    private int currentHealth;
    public Text healthText;  // Refer�ncia ao componente Text para exibir as vidas
    public Text gameOverText;  // Refer�ncia ao componente Text para exibir "GAME OVER"

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        gameOverText.gameObject.SetActive(false);  // Assegura que "GAME OVER" esteja desativado inicialmente
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void UpdateHealthUI()
    {
        healthText.text = "Lifes: " + currentHealth.ToString();  // Atualiza o texto de vidas
    }

    void GameOver()
    {
        gameOverText.gameObject.SetActive(true);  // Exibe o texto "GAME OVER"
        Time.timeScale = 0f;  // Pausa o jogo (para tudo, inclusive anima��es e f�sica)
    }
}
