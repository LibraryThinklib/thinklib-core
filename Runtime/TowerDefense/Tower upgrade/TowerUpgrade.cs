using UnityEngine;

[AddComponentMenu("Thinklib/TowerDefense/Tower Upgrade/Tower Shop", -99)]
public class TowerUpgrade : MonoBehaviour
{
    public int currentLevel = 1;  // N�vel atual da torre
    public int maxLevel = 3;      // M�ximo de n�veis que a torre pode alcan�ar

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

    // M�todo para melhorar a torre
    public void UpgradeTower()
    {
        if (playerScore.currentScore >= upgradeCost && currentLevel < maxLevel)
        {
            playerScore.AddScore(-upgradeCost);  // Subtrai os pontos
            currentLevel++;  // Aumenta o n�vel da torre

            // Atualiza os atributos da torre conforme o n�vel
            switch (currentLevel)
            {
                case 2:
                    damage = 3;  // Novo valor para o dano
                    fireRate = 1.5f;  // Novo valor para a taxa de disparo
                    range = 4f;  // Novo valor para o alcance
                    upgradeCost = 20;  // Aumenta o custo para o pr�ximo n�vel
                    break;
                case 3:
                    damage = 5;  // Novo valor para o dano
                    fireRate = 2f;  // Novo valor para a taxa de disparo
                    range = 5f;  // Novo valor para o alcance
                    upgradeCost = 30;  // Aumenta o custo para o pr�ximo n�vel
                    break;
                default:
                    break;
            }

            Debug.Log("Torre evolu�da para o n�vel " + currentLevel);
        }
        else
        {
            Debug.Log("Pontos insuficientes ou m�ximo de n�vel atingido.");
        }
    }

    // Exibindo as informa��es da torre (opcional)
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "N�vel da Torre: " + currentLevel);
        GUI.Label(new Rect(10, 30, 200, 20), "Dano: " + damage);
        GUI.Label(new Rect(10, 50, 200, 20), "Alcance: " + range);
        GUI.Label(new Rect(10, 70, 200, 20), "Taxa de Disparo: " + fireRate);
    }
}
