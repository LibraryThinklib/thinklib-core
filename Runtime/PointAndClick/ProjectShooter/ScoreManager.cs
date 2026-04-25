using UnityEngine;
using TMPro;

[AddComponentMenu("Thinklib/Game/ScoreManager")]
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    [Header("UI")]
    [Tooltip("O texto da UI para mostrar a pontuação.")]
    public TextMeshProUGUI scoreText;

    [Header("Estado")]
    private int currentScore = 0;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int amount)
    {
        if (amount <= 0) return;
        
        currentScore += amount;
        UpdateScoreUI();
        Debug.Log($"Pontuação: {currentScore}");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    public int GetScore()
    {
        return currentScore;
    }
}