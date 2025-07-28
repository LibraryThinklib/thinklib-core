using UnityEngine;
using UnityEngine.UI;  // Para utilizar o Button da UI

public class TowerShop : MonoBehaviour
{
    public int towerCost = 5;  // Custo da torre
    public Button buyButton;   // Botão de compra
    private PlayerScore playerScore;
    private bool isPlacingTower = false;  // Para verificar se está no modo de construção

    void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();  // Encontra o script PlayerScore
        buyButton.onClick.AddListener(TryBuyTower);  // Adiciona o evento de clique no botão
    }

    void TryBuyTower()
    {
        if (playerScore.currentScore >= towerCost)  // Verifica se o jogador tem pontos suficientes
        {
            playerScore.AddScore(-towerCost);  // Subtrai os pontos para comprar
            isPlacingTower = true;  // Ativa o modo de colocar a torre
            Debug.Log("Modo de construção ativado! Escolha onde colocar a torre.");
        }
        else
        {
            Debug.Log("Pontos insuficientes!");
        }
    }

    public bool IsPlacingTower()  // Método para permitir que outros scripts verifiquem o estado
    {
        return isPlacingTower;
    }

    public void StopPlacingTower()  // Desativa o modo de construção
    {
        isPlacingTower = false;
    }
}
