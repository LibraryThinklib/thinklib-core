using UnityEngine;
using UnityEngine.UI;                // Para utilizar o Text da UI
using System.Collections.Generic;    // Para o dicionário do extra
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/TowerDefense/Resource Management/Player Score", -100)]
public class PlayerScore : MonoBehaviour
{
    public int currentScore = 0;     // Pontuação atual
    public Text scoreText;           // Para exibir a pontuação na UI

    // Telemetry
    private const string MechanicName = "TowerDefense/ResourceManagement/PlayerScore";
    private bool _sentInstantiated = false;
    private bool _sentUsed = false;

    private void Awake()
    {
        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(PlayerScore),
                new Dictionary<string, object>
                {
                    { "initialScore", currentScore },
                    { "hasScoreText", scoreText != null }
                }
            );
        }

        // Garante que a UI inicial esteja coerente
        SafeUpdateScoreUI();
    }

    // Método para adicionar pontos
    public void AddScore(int points)
    {
        try
        {
            currentScore += points;

            if (!_sentUsed)
            {
                _sentUsed = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(PlayerScore),
                    new Dictionary<string, object>
                    {
                        { "action", "add_score_first_time" },
                        { "added", points },
                        { "newScore", currentScore }
                    }
                );
            }
            else
            {
                // (Opcional) Caso queira registrar todo incremento:
                // ThinklibTelemetry.Track(
                //     "mechanic_used",
                //     MechanicName,
                //     nameof(PlayerScore),
                //     new Dictionary<string, object>
                //     {
                //         { "action", "add_score" },
                //         { "added", points },
                //         { "newScore", currentScore }
                //     }
                // );
            }

            SafeUpdateScoreUI();
        }
        catch (System.Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerScore),
                new Dictionary<string, object>
                {
                    { "where", "AddScore" },
                    { "points", points },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    // Atualiza a UI com a pontuação atual (com proteção e telemetria de erro)
    private void SafeUpdateScoreUI()
    {
        try
        {
            if (scoreText != null)
            {
                scoreText.text = "Points: " + currentScore.ToString();
            }
            else
            {
                // Se quiser registrar ausência de UI em uso real (opcional)
                // ThinklibTelemetry.Track(
                //     "mechanic_error",
                //     MechanicName,
                //     nameof(PlayerScore),
                //     new Dictionary<string, object>
                //     {
                //         { "where", "SafeUpdateScoreUI" },
                //         { "message", "scoreText is null" }
                //     }
                // );
            }
        }
        catch (System.Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(PlayerScore),
                new Dictionary<string, object>
                {
                    { "where", "SafeUpdateScoreUI" },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }
}
