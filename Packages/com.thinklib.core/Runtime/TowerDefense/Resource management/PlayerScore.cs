using UnityEngine;
using UnityEngine.UI;  // Para utilizar o Text da UI

public class PlayerScore : MonoBehaviour
{
    public int currentScore = 0;  // Pontuação atual
    public Text scoreText;        // Para exibir a pontuação na UI

    // Método para adicionar pontos
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    // Atualiza a UI com a pontuação atual
    void UpdateScoreUI()
    {
        scoreText.text = "Points: " + currentScore.ToString();
    }
}
