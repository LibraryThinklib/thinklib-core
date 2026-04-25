using UnityEngine;
using UnityEngine.UI;                  // Para utilizar o Button da UI
using System.Collections.Generic;      // Para o dicionário do extra
using Thinklib.Telemetry;

[AddComponentMenu("Thinklib/TowerDefense/Resource Management/Tower Shop", -99)]
public class TowerShop : MonoBehaviour
{
    public int towerCost = 5;          // Custo da torre
    public Button buyButton;           // Botão de compra
    private PlayerScore playerScore;
    private bool isPlacingTower = false;  // Modo de construção

    // Telemetry
    private const string MechanicName = "TowerDefense/ResourceManagement/TowerShop";
    private bool _sentInstantiated = false;
    private bool _sentFirstAttempt = false;
    private bool _sentFirstSuccess = false;

    void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();

        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(TryBuyTower);
        }

        if (!_sentInstantiated)
        {
            _sentInstantiated = true;
            ThinklibTelemetry.Track(
                "mechanic_instantiated",
                MechanicName,
                nameof(TowerShop),
                new Dictionary<string, object>
                {
                    { "towerCost", towerCost },
                    { "hasBuyButton", buyButton != null },
                    { "hasPlayerScore", playerScore != null }
                }
            );
        }
    }

    void TryBuyTower()
    {
        try
        {
            if (!_sentFirstAttempt)
            {
                _sentFirstAttempt = true;
                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerShop),
                    new Dictionary<string, object>
                    {
                        { "action", "buy_attempt_first_time" },
                        { "towerCost", towerCost },
                        { "currentScore", playerScore != null ? playerScore.currentScore : -1 }
                    }
                );
            }

            if (playerScore == null || buyButton == null)
            {
                ThinklibTelemetry.Track(
                    "mechanic_error",
                    MechanicName,
                    nameof(TowerShop),
                    new Dictionary<string, object>
                    {
                        { "where", "TryBuyTower" },
                        { "message", "Missing playerScore or buyButton reference" }
                    }
                );
                return;
            }

            if (playerScore.currentScore >= towerCost)
            {
                playerScore.AddScore(-towerCost);
                isPlacingTower = true;

                if (!_sentFirstSuccess)
                {
                    _sentFirstSuccess = true;
                    ThinklibTelemetry.Track(
                        "mechanic_used",
                        MechanicName,
                        nameof(TowerShop),
                        new Dictionary<string, object>
                        {
                            { "action", "enter_build_mode_first_time" },
                            { "towerCost", towerCost },
                            { "newScore", playerScore.currentScore }
                        }
                    );
                }
                Debug.Log("Modo de construção ativado! Escolha onde colocar a torre.");
            }
            else
            {

                ThinklibTelemetry.Track(
                    "mechanic_used",
                    MechanicName,
                    nameof(TowerShop),
                    new Dictionary<string, object>
                    {
                        { "action", "insufficient_funds" },
                        { "towerCost", towerCost },
                        { "currentScore", playerScore.currentScore }
                    }
                );

                Debug.Log("Pontos insuficientes!");
            }
        }
        catch (System.Exception ex)
        {
            ThinklibTelemetry.Track(
                "mechanic_error",
                MechanicName,
                nameof(TowerShop),
                new Dictionary<string, object>
                {
                    { "where", "TryBuyTower" },
                    { "towerCost", towerCost },
                    { "message", ex.Message },
                    { "stack", ex.StackTrace }
                }
            );
            throw;
        }
    }

    public bool IsPlacingTower()
    {
        return isPlacingTower;
    }

    public void StopPlacingTower()
    {
        isPlacingTower = false;

        ThinklibTelemetry.Track(
            "mechanic_used",
            MechanicName,
            nameof(TowerShop),
            new Dictionary<string, object>
            {
                { "action", "exit_build_mode" }
            }
        );
    }
}
