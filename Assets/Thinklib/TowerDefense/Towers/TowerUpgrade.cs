using UnityEngine;

public class TowerUpgrade : MonoBehaviour
{
    public int currentLevel = 1;  // Nível atual da torre
    public int maxLevel = 3;      // Máximo de níveis que a torre pode alcançar

    // Atributos da torre
    public int damage = 1;
    public float fireRate = 1f;
    public float range = 3f;

    public int upgradeCost = 10;  // Custo para evoluir a torre

    private PlayerScore playerScore;

    void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();  // Encontra o script PlayerScore
    }

    // Método para melhorar a torre
    public void UpgradeTower()
    {
        if (playerScore.currentScore >= upgradeCost && currentLevel < maxLevel)
        {
            playerScore.AddScore(-upgradeCost);  // Subtrai os pontos
            currentLevel++;  // Aumenta o nível da torre

            // Atualiza os atributos da torre conforme o nível
            switch (currentLevel)
            {
                case 2:
                    damage = 3;  // Novo valor para o dano
                    fireRate = 1.5f;  // Novo valor para a taxa de disparo
                    range = 4f;  // Novo valor para o alcance
                    upgradeCost = 20;  // Aumenta o custo para o próximo nível
                    break;
                case 3:
                    damage = 5;  // Novo valor para o dano
                    fireRate = 2f;  // Novo valor para a taxa de disparo
                    range = 5f;  // Novo valor para o alcance
                    upgradeCost = 30;  // Aumenta o custo para o próximo nível
                    break;
                default:
                    break;
            }

            Debug.Log("Torre evoluída para o nível " + currentLevel);
        }
        else
        {
            Debug.Log("Pontos insuficientes ou máximo de nível atingido.");
        }
    }

    // Exibindo as informações da torre (opcional)
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Nível da Torre: " + currentLevel);
        GUI.Label(new Rect(10, 30, 200, 20), "Dano: " + damage);
        GUI.Label(new Rect(10, 50, 200, 20), "Alcance: " + range);
        GUI.Label(new Rect(10, 70, 200, 20), "Taxa de Disparo: " + fireRate);
    }
}
