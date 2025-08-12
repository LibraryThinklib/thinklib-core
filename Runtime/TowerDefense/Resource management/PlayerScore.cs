using UnityEngine;
using UnityEngine.UI;  // Para utilizar o Text da UI

[AddComponentMenu("Thinklib/TowerDefense/Resource Management/Player Score", -100)]
public class PlayerScore : MonoBehaviour
{
    public int currentScore = 0;  // Pontua��o atual
    public Text scoreText;        // Para exibir a pontua��o na UI

    // M�todo para adicionar pontos
    public void AddScore(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }

    // Atualiza a UI com a pontua��o atual
    void UpdateScoreUI()
    {
        scoreText.text = "Points: " + currentScore.ToString();
    }
}
