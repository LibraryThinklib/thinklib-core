using UnityEngine;
using UnityEngine.UI;  // Para utilizar o Button da UI

[AddComponentMenu("Thinklib/TowerDefense/Resource Management/Tower Shop", -99)]
public class TowerShop : MonoBehaviour
{
    public int towerCost = 5;  // Custo da torre
    public Button buyButton;   // Bot�o de compra
    private PlayerScore playerScore;
    private bool isPlacingTower = false;  // Para verificar se est� no modo de constru��o

    void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();  // Encontra o script PlayerScore
        buyButton.onClick.AddListener(TryBuyTower);  // Adiciona o evento de clique no bot�o
    }

    void TryBuyTower()
    {
        if (playerScore.currentScore >= towerCost)  // Verifica se o jogador tem pontos suficientes
        {
            playerScore.AddScore(-towerCost);  // Subtrai os pontos para comprar
            isPlacingTower = true;  // Ativa o modo de colocar a torre
            Debug.Log("Modo de constru��o ativado! Escolha onde colocar a torre.");
        }
        else
        {
            Debug.Log("Pontos insuficientes!");
        }
    }

    public bool IsPlacingTower()  // M�todo para permitir que outros scripts verifiquem o estado
    {
        return isPlacingTower;
    }

    public void StopPlacingTower()  // Desativa o modo de constru��o
    {
        isPlacingTower = false;
    }
}
